using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMC.ApiService.DTO;
using NMC.DataAccess;
using NMC.DataAccess.Models;

namespace NMC.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TracksController : ControllerBase
    {
        private readonly NmcDbContext _context;

        public TracksController(NmcDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserTracks([FromQuery] int userId)
        {
            var tracks = await FetchTracksByUserIdAsync(userId);
            return Ok(tracks);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTrackById(int id)
        {
            var track = await FetchUserTrackByIdAsync(id);

            if (track == null)
            {
                return NotFound("We cant find the track you are looking for.");
            }

            return Ok(track);
        }

        [HttpGet("artist/{artistName}")]
        public async Task<IActionResult> GetTracksByArtist([FromQuery] int userId, string artistName)
        {
            var tracks = await FetchTracksByArtistAsync(userId, artistName);
            return Ok(tracks);
        }

        [HttpPut("{userTrackId:int}/cover")]
        public async Task<IActionResult> UpdateCover(int userTrackId, [FromBody] UpdateCoverDto dto)
        {
            var userTrack = await _context.UserTracks.FindAsync(userTrackId);

            if (userTrack == null)
            {
                return NotFound("We cant find the track you are looking for.");
            }

            userTrack.CoverPath = dto.CoverPath;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadTrack([FromBody] CreateUserTrackRequestDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Artist))
            {
                return BadRequest("Title and Artist are strictly required.");
            }

            var normalizedTitle = dto.Title.Trim();
            var normalizedArtist = dto.Artist.Trim();

            var invalidValues = new[] { "unknown", "none", "null", "undefined", "unknown_1" };
            if (invalidValues.Contains(normalizedTitle.ToLowerInvariant()) ||
                invalidValues.Contains(normalizedArtist.ToLowerInvariant()))
            {
                return BadRequest("Generic names like 'unknown' or 'none' are not allowed.");
            }

            var track = await _context.Tracks.FirstOrDefaultAsync(t => t.Title.ToLower() == normalizedTitle.ToLower() && t.Artist.ToLower() == normalizedArtist.ToLower(), cancellationToken);

            if (track == null)
            {
                track = new Track
                {
                    Title = normalizedTitle,
                    Artist = normalizedArtist
                };

                _context.Tracks.Add(track);
                await _context.SaveChangesAsync(cancellationToken);

                await SeedDummyAnalysisScoresAsync(track.Id, cancellationToken);
            }

            var userTrack = await _context.UserTracks.FirstOrDefaultAsync(ut => ut.UserId == dto.UserId && ut.TrackId == track.Id, cancellationToken);

            if (userTrack != null)
            {
                userTrack.FilePath = dto.FilePath;
                userTrack.CoverPath = dto.CoverPath;
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                userTrack = new UserTrack
                {
                    UserId = dto.UserId,
                    TrackId = track.Id,
                    FilePath = dto.FilePath,
                    CoverPath = dto.CoverPath
                };

                _context.UserTracks.Add(userTrack);
                await _context.SaveChangesAsync(cancellationToken);
            }

            var responseDto = await FetchUserTrackByIdAsync(userTrack.Id);
            return Ok(responseDto);
        }

        #region Support Methods

        private async Task<List<UserTrackDto>> FetchTracksByUserIdAsync(int userId)
        {
            return await _context.UserTracks.Where(ut => ut.UserId == userId)
                .Select(ut => new UserTrackDto(
                    ut.Id,
                    ut.TrackId,
                    ut.Track.Title,
                    ut.Track.Artist,
                    ut.FilePath,
                    ut.CoverPath,
                    ut.Track.GenreScores.Select(gs => new GenreScoreDto(gs.GenreId, gs.Genre.Name, gs.Score)).ToList(),
                    ut.Track.AcousticScores.Select(acs => new AcousticScoreDto(acs.AttributeId, acs.Attribute.Name, acs.Score)).ToList()
                )).ToListAsync();
        }

        private async Task<UserTrackDto?> FetchUserTrackByIdAsync(int userTrackId)
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
                )).FirstOrDefaultAsync();
        }

        private async Task<List<UserTrackDto>> FetchTracksByArtistAsync(int userId, string artistName)
        {
            var lowerArtist = artistName.ToLower();

            return await _context.UserTracks.Where(ut => ut.UserId == userId && ut.Track.Artist.ToLower().Contains(lowerArtist))
                .Select(ut => new UserTrackDto(
                    ut.Id,
                    ut.TrackId,
                    ut.Track.Title,
                    ut.Track.Artist,
                    ut.FilePath,
                    ut.CoverPath,
                    ut.Track.GenreScores.Select(gs => new GenreScoreDto(gs.GenreId, gs.Genre.Name, gs.Score)).ToList(),
                    ut.Track.AcousticScores.Select(acs => new AcousticScoreDto(acs.AttributeId, acs.Attribute.Name, acs.Score)).ToList()
                )).ToListAsync();
        }

        private async Task SeedDummyAnalysisScoresAsync(int trackId, CancellationToken cancellationToken)
        {
            var genres = await _context.Genres.ToListAsync(cancellationToken);
            if (genres.Any())
            {
                var random = new Random();
                foreach (var genre in genres)
                {
                    _context.GenreScores.Add(new GenreScore
                    {
                        TrackId = trackId,
                        GenreId = genre.Id,
                        Score = (float)Math.Round(random.NextDouble(), 2)
                    });
                }
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        #endregion
    }
}