using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMC.ApiService.DTO;
using NMC.DataAccess;

namespace NMC.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationsController : ControllerBase
    {
        private const int MockResultCount = 2;

        private readonly NmcDbContext _context;

        public RecommendationsController(NmcDbContext context)
        {
            _context = context;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromQuery] int userId, [FromBody] GenerateRecommendationDto dto, CancellationToken cancellationToken)
        {
            var tracks = await FetchMockRecommendationsAsync(userId, cancellationToken);
            return Ok(tracks);
        }

        #region Support Methods
        private async Task<List<UserTrackDto>> FetchMockRecommendationsAsync(int userId, CancellationToken cancellationToken)
        {
            return await _context.UserTracks
                .Where(ut => ut.UserId == userId)
                .OrderBy(ut => ut.Id)
                .Take(MockResultCount)
                .Select(ut => new UserTrackDto(
                    ut.Id,
                    ut.TrackId,
                    ut.Track.Title,
                    ut.Track.Artist,
                    ut.FilePath,
                    ut.CoverPath,
                    ut.Track.GenreScores.Select(gs => new GenreScoreDto(gs.GenreId, gs.Genre.Name, gs.Score)).ToList(),
                    ut.Track.AcousticScores.Select(acs => new AcousticScoreDto(acs.AttributeId, acs.Attribute.Name, acs.Score)).ToList()
                )).ToListAsync(cancellationToken);
        }
        #endregion
    }
}
