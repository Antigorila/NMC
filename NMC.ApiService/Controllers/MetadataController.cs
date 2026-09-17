using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMC.ApiService.DTO;
using NMC.DataAccess;

namespace NMC.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetadataController : ControllerBase
    {
        private readonly NmcDbContext _context;

        public MetadataController(NmcDbContext context)
        {
            _context = context;
        }

        [HttpGet("genres")]
        public async Task<IActionResult> GetGenres()
        {
            var genres = await FetchAllGenresAsync();
            return Ok(genres);
        }

        [HttpGet("acoustic-attributes")]
        public async Task<IActionResult> GetAcousticAttributes()
        {
            var attributes = await FetchAllAcousticAttributesAsync();
            return Ok(attributes);
        }

        #region Support Methods
        private async Task<List<GenreScoreDto>> FetchAllGenresAsync()
        {
            return await _context.Genres.Select(g => new GenreScoreDto(g.Id, g.Name, 0)).ToListAsync();
        }

        private async Task<List<AcousticScoreDto>> FetchAllAcousticAttributesAsync()
        {
            return await _context.AcousticAttributes.Select(a => new AcousticScoreDto(a.Id, a.Name, 0)).ToListAsync();
        }
        #endregion
    }
}