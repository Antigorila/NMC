using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMC.ApiService.Controllers;
using NMC.ApiService.DTO;
using NMC.DataAccess;
using NMC.DataAccess.Models;

namespace ApiTests
{
    [TestFixture]
    public class PlaylistsControllerTests
    {
        private DbContextOptions<NmcDbContext> _dbOptions = null!;

        [SetUp]
        public void SetUp()
        {
            _dbOptions = new DbContextOptionsBuilder<NmcDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        }

        private NmcDbContext CreateContext() => new NmcDbContext(_dbOptions);

        #region GetPlaylists Tests

        [Test]
        public async Task GetPlaylists_ReturnsOnlyPlaylistsForSpecifiedUser()
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                setupContext.Playlists.AddRange(
                    new Playlist { Id = 1, UserId = 10, PlaylistName = "Alternative" },
                    new Playlist { Id = 2, UserId = 10, PlaylistName = "Ethereal" },
                    new Playlist { Id = 3, UserId = 20, PlaylistName = "User20 List" }
                );
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new PlaylistsController(testContext);

            // Act
            var result = await controller.GetPlaylists(userId: 10);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var playlists = okResult!.Value as List<PlaylistDto>;
            Assert.That(playlists, Is.Not.Null);
            Assert.That(playlists!.Count, Is.EqualTo(2));
            Assert.That(playlists.All(p => p.UserId == 10), Is.True);
        }

        #endregion

        #region GetPlaylistById Tests

        [Test]
        public async Task GetPlaylistById_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new PlaylistsController(testContext);

            // Act
            var result = await controller.GetPlaylistById(999, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFound = result as NotFoundObjectResult;
            Assert.That(notFound, Is.Not.Null);
            Assert.That(notFound!.Value, Is.EqualTo("Sorry, there is no such playlist."));
        }

        [Test]
        public async Task GetPlaylistById_ExistingPlaylist_ReturnsPlaylistDetail()
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                var track = new Track { Id = 1, Title = "Song A", Artist = "Artist A" };
                var userTrack = new UserTrack { Id = 1, Track = track, UserId = 1, FilePath = "/path/a.mp3" };
                var playlist = new Playlist { Id = 10, UserId = 1, PlaylistName = "My List" };
                var playlistTrack = new PlaylistTrack { Playlist = playlist, UserTrack = userTrack };

                setupContext.Tracks.Add(track);
                setupContext.UserTracks.Add(userTrack);
                setupContext.Playlists.Add(playlist);
                setupContext.PlaylistTracks.Add(playlistTrack);
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new PlaylistsController(testContext);

            // Act
            var result = await controller.GetPlaylistById(10, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var playlistDetail = okResult!.Value as PlaylistDetailDto;
            Assert.That(playlistDetail, Is.Not.Null);
            Assert.That(playlistDetail!.Id, Is.EqualTo(10));
            Assert.That(playlistDetail.PlaylistName, Is.EqualTo("My List"));
            Assert.That(playlistDetail.Tracks.Count, Is.EqualTo(1));
            Assert.That(playlistDetail.Tracks[0].Title, Is.EqualTo("Song A"));
        }

        #endregion

        #region CreatePlaylist Tests

        [Test]
        public async Task CreatePlaylist_EmptyName_ReturnsBadRequest()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new PlaylistsController(testContext);
            var dto = new CreatePlaylistDto("", null);

            // Act
            var result = await controller.CreatePlaylist(userId: 1, dto, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null);
            Assert.That(badRequest!.Value, Is.EqualTo("And how should we call the playlist?"));
        }

        [Test]
        public async Task CreatePlaylist_ValidDto_CreatesAndReturnsCreatedAtAction()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new PlaylistsController(testContext);
            var dto = new CreatePlaylistDto("New Playlist", "/covers/cover.jpg");

            // Act
            var result = await controller.CreatePlaylist(userId: 5, dto, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result as CreatedAtActionResult;
            Assert.That(createdResult, Is.Not.Null);

            var playlistDto = createdResult!.Value as PlaylistDto;
            Assert.That(playlistDto, Is.Not.Null);
            Assert.That(playlistDto!.PlaylistName, Is.EqualTo("New Playlist"));
            Assert.That(playlistDto.UserId, Is.EqualTo(5));
            Assert.That(playlistDto.CoverPath, Is.EqualTo("/covers/cover.jpg"));

            // Database verification
            Assert.That(await testContext.Playlists.CountAsync(), Is.EqualTo(1));
        }

        #endregion

        #region AddTrackToPlaylist Tests

        [Test]
        public async Task AddTrackToPlaylist_TrackAlreadyInPlaylist_ReturnsBadRequest()
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                setupContext.PlaylistTracks.Add(new PlaylistTrack { PlaylistId = 1, UserTrackId = 10 });
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new PlaylistsController(testContext);
            var dto = new AddTrackToPlaylistDto(UserTrackId: 10);

            // Act
            var result = await controller.AddTrackToPlaylist(id: 1, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null);
            Assert.That(badRequest!.Value, Is.EqualTo("You already have that track in your playlist."));
        }

        [Test]
        public async Task AddTrackToPlaylist_NewTrack_AddsTrackAndReturnsOk()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new PlaylistsController(testContext);
            var dto = new AddTrackToPlaylistDto(UserTrackId: 20);

            // Act
            var result = await controller.AddTrackToPlaylist(id: 1, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());

            // Database verification
            var dbPlaylistTrack = await testContext.PlaylistTracks.FirstOrDefaultAsync(pt => pt.PlaylistId == 1 && pt.UserTrackId == 20);
            Assert.That(dbPlaylistTrack, Is.Not.Null);
        }

        [Test]
        public async Task AddTrackToPlaylist_WithExistingParents_AddsTrackAndReturnsOk()
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                var user = new User { Id = 1, Username = "testuser", PasswordHash = "hash" };
                var track = new Track { Id = 1, Title = "Song", Artist = "Artist" };
                var userTrack = new UserTrack { Id = 20, UserId = 1, Track = track, FilePath = "/path.mp3" };
                var playlist = new Playlist { Id = 1, UserId = 1, PlaylistName = "My Playlist" };

                setupContext.Users.Add(user);
                setupContext.Tracks.Add(track);
                setupContext.UserTracks.Add(userTrack);
                setupContext.Playlists.Add(playlist);
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new PlaylistsController(testContext);
            var dto = new AddTrackToPlaylistDto(UserTrackId: 20);

            // Act
            var result = await controller.AddTrackToPlaylist(id: 1, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());
            var dbPlaylistTrack = await testContext.PlaylistTracks
                .FirstOrDefaultAsync(pt => pt.PlaylistId == 1 && pt.UserTrackId == 20);
            Assert.That(dbPlaylistTrack, Is.Not.Null);
        }
        #endregion

        #region RemoveTrackFromPlaylist Tests

        [Test]
        public async Task RemoveTrackFromPlaylist_TrackNotInPlaylist_ReturnsNotFound()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new PlaylistsController(testContext);

            // Act
            var result = await controller.RemoveTrackFromPlaylist(id: 1, userTrackId: 99);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFound = result as NotFoundObjectResult;
            Assert.That(notFound, Is.Not.Null);
            Assert.That(notFound!.Value, Is.EqualTo("Seems like the track is not in this playlist."));
        }

        [Test]
        public async Task RemoveTrackFromPlaylist_TrackExists_RemovesAndReturnsNoContent()
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                setupContext.PlaylistTracks.Add(new PlaylistTrack { PlaylistId = 1, UserTrackId = 5 });
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new PlaylistsController(testContext);

            // Act
            var result = await controller.RemoveTrackFromPlaylist(id: 1, userTrackId: 5);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());

            // Database verification
            var dbPlaylistTrack = await testContext.PlaylistTracks.FirstOrDefaultAsync(pt => pt.PlaylistId == 1 && pt.UserTrackId == 5);
            Assert.That(dbPlaylistTrack, Is.Null);
        }

        #endregion

        #region DeletePlaylist Tests

        [Test]
        public async Task DeletePlaylist_NonExistentPlaylist_ReturnsNotFound()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new PlaylistsController(testContext);

            // Act
            var result = await controller.DeletePlaylist(id: 404);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFound = result as NotFoundObjectResult;
            Assert.That(notFound, Is.Not.Null);
            Assert.That(notFound!.Value, Is.EqualTo("Sorry, but there is no such playlist. Perhaps you have already deleted it."));
        }

        [Test]
        public async Task DeletePlaylist_ExistingPlaylist_RemovesAndReturnsNoContent()
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                setupContext.Playlists.Add(new Playlist { Id = 10, UserId = 1, PlaylistName = "To Delete" });
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new PlaylistsController(testContext);

            // Act
            var result = await controller.DeletePlaylist(id: 10);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());

            // Database verification
            var dbPlaylist = await testContext.Playlists.FindAsync(10);
            Assert.That(dbPlaylist, Is.Null);
        }

        #endregion
    }
}