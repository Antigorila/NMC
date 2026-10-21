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
        public async Task<IActionResult> GetPlaylistById(int id, CancellationToken cancellationToken)
        {
            var playlist = await FetchPlaylistDetailByIdAsync(id, cancellationToken);

            if (playlist == null)
            {
                return NotFound("Sorry, there is no such playlist.");
            }

            return Ok(playlist);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlaylist([FromQuery] int userId, [FromBody] CreatePlaylistDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(dto.PlaylistName))
            {
                return BadRequest("And how should we call the playlist?");
            }

            var newPlaylist = new Playlist
            {
                UserId = userId,
                PlaylistName = dto.PlaylistName.Trim(),
                CoverPath = dto.CoverPath
            };

            _context.Playlists.Add(newPlaylist);
            await _context.SaveChangesAsync(cancellationToken);

            var resultDto = new PlaylistDto(
                newPlaylist.Id,
                newPlaylist.UserId,
                newPlaylist.PlaylistName,
                newPlaylist.CoverPath,
                0
            );

            return CreatedAtAction(nameof(GetPlaylistById), new { id = newPlaylist.Id }, resultDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePlaylist(int id, [FromBody] UpdatePlaylistDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(dto.PlaylistName))
            {
                return BadRequest("And how should we call the playlist?");
            }

            var playlist = await _context.Playlists.FindAsync(new object[] { id }, cancellationToken);

            if (playlist == null)
            {
                return NotFound("Sorry, there is no such playlist.");
            }

            playlist.PlaylistName = dto.PlaylistName.Trim();
            playlist.CoverPath = dto.CoverPath;
            await _context.SaveChangesAsync(cancellationToken);

            int trackCount = await _context.PlaylistTracks.CountAsync(pt => pt.PlaylistId == id, cancellationToken);

            var resultDto = new PlaylistDto(
                playlist.Id,
                playlist.UserId,
                playlist.PlaylistName,
                playlist.CoverPath,
                trackCount
            );

            return Ok(resultDto);
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

        private async Task<PlaylistDetailDto?> FetchPlaylistDetailByIdAsync(int playlistId, CancellationToken cancellationToken)
        {
            return await _context.Playlists
                .Where(p => p.Id == playlistId)
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
                        pt.UserTrack.Track.GenreScores.Select(gs => new GenreScoreDto(gs.GenreId, gs.Genre.Name, gs.Score)).ToList(),
                        pt.UserTrack.Track.AcousticScores.Select(acs => new AcousticScoreDto(acs.AttributeId, acs.Attribute.Name, acs.Score)).ToList()
                    )).ToList()
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        private async Task<bool> CheckTrackInPlaylistAsync(int playlistId, int userTrackId)
        {
            return await _context.PlaylistTracks.AnyAsync(pt => pt.PlaylistId == playlistId && pt.UserTrackId == userTrackId);
        }

        #endregion
    }
}
