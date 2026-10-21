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

        [HttpPost("filter")]
        public async Task<IActionResult> FilterTracks([FromQuery] int userId, [FromBody] TrackFilterDto dto, CancellationToken cancellationToken)
        {
            if (dto.Threshold < 0 || dto.Threshold > 1)
            {
                return BadRequest("The threshold must be between 0 and 1.");
            }

            bool genreExists = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId, cancellationToken);
            if (!genreExists)
            {
                return BadRequest("Please select a genre.");
            }

            var tracks = await FetchFilteredTracksAsync(userId, dto, cancellationToken);
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

        [HttpDelete("{userTrackId:int}")]
        public async Task<IActionResult> DeleteUserTrack(int userTrackId, CancellationToken cancellationToken)
        {
            var userTrack = await _context.UserTracks.FindAsync(new object[] { userTrackId }, cancellationToken);

            if (userTrack == null)
            {
                return NotFound("We cant find the track you are looking for.");
            }

            var playlistTracks = await _context.PlaylistTracks.Where(pt => pt.UserTrackId == userTrackId).ToListAsync(cancellationToken);

            _context.PlaylistTracks.RemoveRange(playlistTracks);
            _context.UserTracks.Remove(userTrack);
            await _context.SaveChangesAsync(cancellationToken);

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

        private async Task<List<UserTrackDto>> FetchFilteredTracksAsync(int userId, TrackFilterDto filter, CancellationToken cancellationToken)
        {
            IQueryable<UserTrack> query = _context.UserTracks.Where(ut => ut.UserId == userId);

            if (filter.IsHardFilter)
            {
                query = query.Where(ut => ut.Track.GenreScores
                    .OrderByDescending(gs => gs.Score)
                    .Select(gs => gs.GenreId)
                    .FirstOrDefault() == filter.GenreId);
            }
            else
            {
                query = query.Where(ut => ut.Track.GenreScores
                    .Any(gs => gs.GenreId == filter.GenreId && gs.Score > filter.Threshold));
            }

            return await query
                .OrderByDescending(ut => ut.Track.GenreScores
                    .Where(gs => gs.GenreId == filter.GenreId)
                    .Select(gs => gs.Score)
                    .FirstOrDefault())
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