using Microsoft.AspNetCore.Mvc;
using NMC.ApiService.DTO;
using NMC.DataAccess;
using NMC.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace NMC.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistsController : ControllerBase
    {
        private readonly NmcDbContext _context;

        public PlaylistsController(NmcDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPlaylists([FromQuery] int userId)
        {
            var playlists = await FetchUserPlaylistsAsync(userId);
            return Ok(playlists);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPlaylistById(int id)
        {
            var playlist = await FetchPlaylistDetailByIdAsync(id);

            if (playlist == null)
            {
                return NotFound("Sorry, there is no such playlist.");
            }

            return Ok(playlist);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlaylist([FromQuery] int userId, [FromBody] CreatePlaylistDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.PlaylistName))
            {
                return BadRequest("Nice try, but the playlist name cannot be empty.");
            }

            var newPlaylist = new Playlist
            {
                UserId = userId,
                PlaylistName = dto.PlaylistName,
                CoverPath = dto.CoverPath
            };

            _context.Playlists.Add(newPlaylist);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlaylistById), new { id = newPlaylist.Id }, newPlaylist.Id);
        }

        [HttpPost("{id:int}/tracks")]
        public async Task<IActionResult> AddTrackToPlaylist(int id, [FromBody] AddTrackToPlaylistDto dto)
        {
            bool trackAlreadyInPlaylist = await CheckTrackInPlaylistAsync(id, dto.UserTrackId);

            if (trackAlreadyInPlaylist)
            {
                return BadRequest("You already have that track in your playlist.");
            }

            var playlistTrack = new PlaylistTrack
            {
                PlaylistId = id,
                UserTrackId = dto.UserTrackId
            };

            _context.PlaylistTracks.Add(playlistTrack);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id:int}/tracks/{userTrackId:int}")]
        public async Task<IActionResult> RemoveTrackFromPlaylist(int id, int userTrackId)
        {
            var playlistTrack = await _context.PlaylistTracks.FirstOrDefaultAsync(pt => pt.PlaylistId == id && pt.UserTrackId == userTrackId);

            if (playlistTrack == null)
            {
                return NotFound("Seems like the track is not in this playlist.");
            }

            _context.PlaylistTracks.Remove(playlistTrack);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePlaylist(int id)
        {
            var playlist = await _context.Playlists.FindAsync(id);

            if (playlist == null)
            {
                return NotFound("Sorry, but there is no such playlist. Perhaps you have already deleted it.");
            }

            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #region Support Methods

        private async Task<List<PlaylistDto>> FetchUserPlaylistsAsync(int userId)
        {
            return await _context.Playlists.Where(p => p.UserId == userId)
                .Select(p => new PlaylistDto(
                    p.Id,
                    p.UserId,
                    p.PlaylistName,
                    p.CoverPath,
                    p.PlaylistTracks.Count
                )).ToListAsync();
        }

        private async Task<PlaylistDetailDto?> FetchPlaylistDetailByIdAsync(int playlistId)
        {
            return await _context.Playlists.Where(p => p.Id == playlistId)
                .Select(p => new PlaylistDetailDto(
                    p.Id,
                    p.UserId,
                    p.PlaylistName,
                    p.CoverPath,
                    p.PlaylistTracks.Select(pt => new UserTrackDto(
                        pt.UserTrack.Id,
                        pt.UserTrack.TrackId,
                        pt.UserTrack.Track.Title,
                        pt.UserTrack.Track.Artist,
                        pt.UserTrack.FilePath,
                        pt.UserTrack.CoverPath,
                        new List<GenreScoreDto>(),
                        new List<AcousticScoreDto>()
                    )).ToList()
                )).FirstOrDefaultAsync();
        }

        private async Task<bool> CheckTrackInPlaylistAsync(int playlistId, int userTrackId)
        {
            return await _context.PlaylistTracks.AnyAsync(pt => pt.PlaylistId == playlistId && pt.UserTrackId == userTrackId);
        }

        #endregion
    }
}
