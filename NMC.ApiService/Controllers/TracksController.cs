using Microsoft.AspNetCore.Mvc;
using NMC.ApiService.DTO;
using NMC.DataAccess;
using Microsoft.EntityFrameworkCore;

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

        #endregion
    }
}