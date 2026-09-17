using Microsoft.AspNetCore.Mvc;
using NMC.ApiService.DTO;
using NMC.DataAccess;
using NMC.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace NMC.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly NmcDbContext _context;

        public AnalysisController(NmcDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAndAnalyze([FromQuery] int userId, [FromQuery] string title, [FromQuery] string artist, [FromQuery] string filePath)
        {
            var track = await _context.Tracks.FirstOrDefaultAsync(t => t.Title == title && t.Artist == artist);

            if (track == null)
            {
                track = new Track
                {
                    Title = title,
                    Artist = artist
                };

                _context.Tracks.Add(track);
                await _context.SaveChangesAsync();

                await GenerateDummyScoresForTrackAsync(track.Id);
            }

            var userTrack = new UserTrack
            {
                UserId = userId,
                TrackId = track.Id,
                FilePath = filePath
            };

            _context.UserTracks.Add(userTrack);
            await _context.SaveChangesAsync();

            var genreScores = await GetGenreScoresForTrackAsync(track.Id);
            var acousticScores = await GetAcousticScoresForTrackAsync(track.Id);

            var response = new AnalysisUploadResponseDto(
                track.Id,
                userTrack.Id,
                track.Title,
                track.Artist,
                genreScores,
                acousticScores
            );

            return Ok(response);
        }

        #region Support Methods
        private async Task GenerateDummyScoresForTrackAsync(int trackId)
        {
            var genres = await _context.Genres.ToListAsync();
            var attributes = await _context.AcousticAttributes.ToListAsync();
            var random = new Random();

            foreach (var genre in genres)
            {
                var genreScore = new GenreScore
                {
                    TrackId = trackId,
                    GenreId = genre.Id,
                    Score = Math.Round(random.NextDouble(), 2)
                };
                _context.GenreScores.Add(genreScore);
            }

            foreach (var attribute in attributes)
            {
                var acousticScore = new AcousticScore
                {
                    TrackId = trackId,
                    AttributeId = attribute.Id,
                    Score = Math.Round(random.NextDouble(), 2)
                };
                _context.AcousticScores.Add(acousticScore);
            }

            await _context.SaveChangesAsync();
        }

        private async Task<List<GenreScoreDto>> GetGenreScoresForTrackAsync(int trackId)
        {
            return await _context.GenreScores.Where(gs => gs.TrackId == trackId)
                .Select(gs => new GenreScoreDto(
                    gs.GenreId,
                    gs.Genre.Name,
                    gs.Score
                )).ToListAsync();
        }

        private async Task<List<AcousticScoreDto>> GetAcousticScoresForTrackAsync(int trackId)
        {
            return await _context.AcousticScores.Where(acs => acs.TrackId == trackId)
                .Select(acs => new AcousticScoreDto(
                    acs.AttributeId,
                    acs.Attribute.Name,
                    acs.Score
                )).ToListAsync();
        }
        #endregion
    }
}