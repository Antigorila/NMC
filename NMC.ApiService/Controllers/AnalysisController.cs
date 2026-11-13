using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMC.ApiService.DTO;
using NMC.DataAccess;
using NMC.NeuralNetwork;
using NMC.DataAccess.Models;

namespace NMC.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private const long MaxUploadBytes = 50 * 1024 * 1024;
        private static readonly string[] InvalidNames = { "unknown", "none", "null", "undefined" };

        private readonly NmcDbContext _context;
        private readonly IAudioClassifier _classifier;

        public AnalysisController(NmcDbContext context, IAudioClassifier classifier)
        {
            _context = context;
            _classifier = classifier;
        }

        [HttpGet("check")]
        public async Task<IActionResult> CheckTrack([FromQuery] string? title, [FromQuery] string? artist, CancellationToken cancellationToken)
        {
            string? validationError = ValidateTitleAndArtist(title, artist);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            Track? track = await FindTrackAsync(title!.Trim(), artist!.Trim(), cancellationToken);

            return Ok(new TrackCheckDto(track != null));
        }

        [HttpPost("upload")]
        [RequestSizeLimit(MaxUploadBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxUploadBytes)]
        public async Task<IActionResult> UploadAndAnalyze([FromForm] UploadTrackFormDto form, CancellationToken cancellationToken)
        {
            string? validationError = ValidateTitleAndArtist(form.Title, form.Artist);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            string title = form.Title.Trim();
            string artist = form.Artist.Trim();

            Track? track = await FindTrackAsync(title, artist, cancellationToken);

            if (track == null)
            {
                if (form.File == null || form.File.Length == 0)
                {
                    return BadRequest("The audio file is required to analyse a new track.");
                }

                if (!IsMp3(form.File))
                {
                    return BadRequest("Only MP3 files can be analysed.");
                }

                track = await CreateAnalysedTrackAsync(title, artist, form.File, cancellationToken);
            }

            UserTrack userTrack = await AddOrUpdateUserTrackAsync(form.UserId, track.Id, form.FilePath, form.CoverPath, cancellationToken);

            UserTrackDto? response = await FetchUserTrackByIdAsync(userTrack.Id, cancellationToken);
            return Ok(response);
        }

        #region Support Methods
        private static string? ValidateTitleAndArtist(string? title, string? artist)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(artist))
            {
                return "Title and Artist are strictly required.";
            }

            if (InvalidNames.Contains(title.Trim().ToLowerInvariant()) || InvalidNames.Contains(artist.Trim().ToLowerInvariant()))
            {
                return "Generic names like 'unknown' or 'none' are not allowed.";
            }

            return null;
        }

        private static bool IsMp3(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName);
            return extension.Equals(".mp3", StringComparison.OrdinalIgnoreCase);
        }

        private async Task<Track?> FindTrackAsync(string title, string artist, CancellationToken cancellationToken)
        {
            string lowerTitle = title.ToLowerInvariant();
            string lowerArtist = artist.ToLowerInvariant();

            return await _context.Tracks.FirstOrDefaultAsync(t => t.Title.ToLower() == lowerTitle && t.Artist.ToLower() == lowerArtist, cancellationToken);
        }

        private async Task<string> ResolveArtistNameAsync(string artist, CancellationToken cancellationToken)
        {
            string lowerArtist = artist.ToLowerInvariant();
            Track? trackWithSameArtist = await _context.Tracks.FirstOrDefaultAsync(t => t.Artist.ToLower() == lowerArtist, cancellationToken);

            if (trackWithSameArtist == null)
            {
                return artist;
            }

            return trackWithSameArtist.Artist;
        }

        private async Task<Track> CreateAnalysedTrackAsync(string title, string artist, IFormFile file, CancellationToken cancellationToken)
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp3");

            try
            {
                using (FileStream tempFile = System.IO.File.Create(tempFilePath))
                {
                    await file.CopyToAsync(tempFile, cancellationToken);
                }

                List<Genre> genres = await _context.Genres.ToListAsync(cancellationToken);
                List<AcousticAttribute> attributes = await _context.AcousticAttributes.ToListAsync(cancellationToken);

                var genreNames = new List<string>();
                foreach (Genre genre in genres)
                {
                    genreNames.Add(genre.Name);
                }

                var attributeNames = new List<string>();
                foreach (AcousticAttribute attribute in attributes)
                {
                    attributeNames.Add(attribute.Name);
                }

                ClassificationResult classification = await _classifier.ClassifyAsync(tempFilePath, genreNames, attributeNames, cancellationToken);

                var track = new Track
                {
                    Title = title,
                    Artist = await ResolveArtistNameAsync(artist, cancellationToken)
                };

                foreach (Genre genre in genres)
                {
                    if (classification.GenreScores.TryGetValue(genre.Name, out double score))
                    {
                        track.GenreScores.Add(new GenreScore { GenreId = genre.Id, Score = score });
                    }
                }

                foreach (AcousticAttribute attribute in attributes)
                {
                    if (classification.AcousticScores.TryGetValue(attribute.Name, out double score))
                    {
                        track.AcousticScores.Add(new AcousticScore { AttributeId = attribute.Id, Score = score });
                    }
                }

                _context.Tracks.Add(track);
                await _context.SaveChangesAsync(cancellationToken);

                return track;
            }
            finally
            {
                if (System.IO.File.Exists(tempFilePath))
                {
                    System.IO.File.Delete(tempFilePath);
                }
            }
        }

        private async Task<UserTrack> AddOrUpdateUserTrackAsync(int userId, int trackId, string filePath, string? coverPath, CancellationToken cancellationToken)
        {
            UserTrack? userTrack = await _context.UserTracks.FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TrackId == trackId, cancellationToken);

            if (userTrack == null)
            {
                userTrack = new UserTrack
                {
                    UserId = userId,
                    TrackId = trackId
                };

                _context.UserTracks.Add(userTrack);
            }

            userTrack.FilePath = filePath;
            userTrack.CoverPath = coverPath;

            await _context.SaveChangesAsync(cancellationToken);

            return userTrack;
        }

        private async Task<UserTrackDto?> FetchUserTrackByIdAsync(int userTrackId, CancellationToken cancellationToken)
        {
            return await _context.UserTracks.Where(ut => ut.Id == userTrackId)
                .Select(ut => new UserTrackDto(
                    ut.Id,
                    ut.TrackId,
                    ut.Track.Title,
                    ut.Track.Artist,
                    ut.FilePath,
                    ut.CoverPath,
                    ut.Track.GenreScores.Select(gs => new GenreScoreDto(gs.GenreId, gs.Genre.Name, gs.Score)).ToList(),
                    ut.Track.AcousticScores.Select(acs => new AcousticScoreDto(acs.AttributeId, acs.Attribute.Name, acs.Score)).ToList()
                )).FirstOrDefaultAsync(cancellationToken);
        }
        #endregion
    }
}
