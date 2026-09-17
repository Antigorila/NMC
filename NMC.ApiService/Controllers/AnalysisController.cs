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
        public async Task<IActionResult> UploadAndAnalyze([FromQuery] int userId, [FromQuery] string title, [FromQuery] string artist, [FromQuery] string filePath, CancellationToken cancellationToken)
        {
            var track = await _context.Tracks.FirstOrDefaultAsync(t => t.Title == title && t.Artist == artist, cancellationToken);

            if (track == null)
            {
                track = new Track
                {
                    Title = title,
                    Artist = artist
                };

                var genres = await _context.Genres.ToListAsync(cancellationToken);
                var attributes = await _context.AcousticAttributes.ToListAsync(cancellationToken);

                foreach (var genre in genres)
                {
                    track.GenreScores.Add(new GenreScore
                    {
                        GenreId = genre.Id,
                        Score = Math.Round(Random.Shared.NextDouble(), 2)
                    });
                }

                foreach (var attr in attributes)
                {
                    track.AcousticScores.Add(new AcousticScore
                    {
                        AttributeId = attr.Id,
                        Score = Math.Round(Random.Shared.NextDouble(), 2)
                    });
                }
            }

            var userTrack = new UserTrack
            {
                UserId = userId,
                Track = track,
                FilePath = filePath
            };

            _context.UserTracks.Add(userTrack);
            await _context.SaveChangesAsync(cancellationToken);

            var response = await FetchAnalysisResponseAsync(userTrack.Id, cancellationToken);
            return Ok(response);
        }

        #region Support Methods

        private async Task<AnalysisUploadResponseDto> FetchAnalysisResponseAsync(int userTrackId, CancellationToken cancellationToken)
        {
            var userTrack = await _context.UserTracks.Where(ut => ut.Id == userTrackId)
                .Select(ut => new AnalysisUploadResponseDto(
                    ut.TrackId,
                    ut.Id,
                    ut.Track.Title,
                    ut.Track.Artist,
                    ut.Track.GenreScores.Select(gs => new GenreScoreDto(gs.GenreId, gs.Genre.Name, gs.Score)).ToList(),
                    ut.Track.AcousticScores.Select(acs => new AcousticScoreDto(acs.AttributeId, acs.Attribute.Name, acs.Score)).ToList()
                )).FirstAsync(cancellationToken);

            return userTrack;
        }
        #endregion
    }
}