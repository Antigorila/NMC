using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMC.ApiService.Controllers;
using NMC.ApiService.DTO;
using NMC.DataAccess;
using NMC.DataAccess.Models;

namespace ApiTests
{
    [TestFixture]
    public class TracksControllerTests
    {
        private DbContextOptions<NmcDbContext> _dbOptions = null!;

        [SetUp]
        public void SetUp()
        {
            _dbOptions = new DbContextOptionsBuilder<NmcDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        }

        private NmcDbContext CreateContext() => new NmcDbContext(_dbOptions);

        #region GetUserTracks Tests

        [Test]
        public async Task GetUserTracks_ReturnsOnlyTracksForSpecifiedUser()
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                var track1 = new Track { Id = 1, Title = "Song 1", Artist = "Artist A" };
                var track2 = new Track { Id = 2, Title = "Song 2", Artist = "Artist B" };

                setupContext.Tracks.AddRange(track1, track2);
                setupContext.UserTracks.AddRange(
                    new UserTrack { Id = 10, UserId = 1, Track = track1, FilePath = "/p1.mp3" },
                    new UserTrack { Id = 11, UserId = 1, Track = track2, FilePath = "/p2.mp3" },
                    new UserTrack { Id = 12, UserId = 2, Track = track1, FilePath = "/p3.mp3" }
                );
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new TracksController(testContext);

            // Act
            var result = await controller.GetUserTracks(userId: 1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var tracks = okResult!.Value as List<UserTrackDto>;
            Assert.That(tracks, Is.Not.Null);
            Assert.That(tracks!.Count, Is.EqualTo(2));
            Assert.That(tracks[0].UserTrackId, Is.EqualTo(10));
            Assert.That(tracks[1].UserTrackId, Is.EqualTo(11));
        }

        #endregion

        #region GetTrackById Tests

        [Test]
        public async Task GetTrackById_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new TracksController(testContext);

            // Act
            var result = await controller.GetTrackById(id: 999);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFound = result as NotFoundObjectResult;
            Assert.That(notFound, Is.Not.Null);
            Assert.That(notFound!.Value, Is.EqualTo("We cant find the track you are looking for."));
        }

        [Test]
        public async Task GetTrackById_ExistingId_ReturnsUserTrackDto()
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                var genre = new Genre { Id = 1, Name = "Metalcore" };
                var attribute = new AcousticAttribute { Id = 1, Name = "Acousticness" };
                var track = new Track
                {
                    Id = 1,
                    Title = "Like A Villain",
                    Artist = "Bad Omens",
                    GenreScores = new List<GenreScore> { new GenreScore { Genre = genre, Score = 0.95 } },
                    AcousticScores = new List<AcousticScore> { new AcousticScore { Attribute = attribute, Score = 0.10 } }
                };

                setupContext.Genres.Add(genre);
                setupContext.AcousticAttributes.Add(attribute);
                setupContext.Tracks.Add(track);
                setupContext.UserTracks.Add(new UserTrack { Id = 5, UserId = 1, Track = track, FilePath = "/music/Bad Omens.mp3" });
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new TracksController(testContext);

            // Act
            var result = await controller.GetTrackById(id: 5);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var trackDto = okResult!.Value as UserTrackDto;
            Assert.That(trackDto, Is.Not.Null);
            Assert.That(trackDto!.UserTrackId, Is.EqualTo(5));
            Assert.That(trackDto.Title, Is.EqualTo("Like A Villain"));
            Assert.That(trackDto.Artist, Is.EqualTo("Bad Omens"));
            Assert.That(trackDto.GenreScores.Count, Is.EqualTo(1));
            Assert.That(trackDto.GenreScores[0].GenreName, Is.EqualTo("Metalcore"));
            Assert.That(trackDto.AcousticScores.Count, Is.EqualTo(1));
            Assert.That(trackDto.AcousticScores[0].AttributeName, Is.EqualTo("Acousticness"));
        }

        #endregion

        #region GetTracksByArtist Tests

        [Test]
        public async Task GetTracksByArtist_FiltersByUserIdAndArtistSubstring_HandlesSpacesAndMultipleMatches()
        {
            // Arrange
            const string searchTarget = "Motionless In White";

            using (var setupContext = CreateContext())
            {
                var track1 = new Track { Id = 1, Title = "Masterpiece", Artist = "Motionless In White" };
                var track2 = new Track { Id = 2, Title = "Slaughterhouse", Artist = "Motionless In White" };
                var track3 = new Track { Id = 3, Title = "In The End", Artist = "Black Veil Brides" };

                setupContext.Tracks.AddRange(track1, track2, track3);
                setupContext.UserTracks.AddRange(
                    new UserTrack { Id = 1, UserId = 10, Track = track1, FilePath = "/p1.mp3" },
                    new UserTrack { Id = 2, UserId = 10, Track = track2, FilePath = "/p2.mp3" },
                    new UserTrack { Id = 3, UserId = 10, Track = track3, FilePath = "/p3.mp3" },
                    new UserTrack { Id = 4, UserId = 20, Track = track1, FilePath = "/p4.mp3" }
                );
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new TracksController(testContext);

            // Act
            var result = await controller.GetTracksByArtist(userId: 10, artistName: searchTarget);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var tracks = okResult!.Value as List<UserTrackDto>;

            Assert.That(tracks, Is.Not.Null);
            Assert.That(tracks!.Count, Is.EqualTo(2));
            Assert.That(tracks.All(t => t.Artist.Contains(searchTarget, StringComparison.OrdinalIgnoreCase)), Is.True);
        }

        [Test]
        public async Task GetTracksByArtist_CaseInsensitiveAndSpacedSearch_ReturnsMatchingTracks()
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                var track1 = new Track { Id = 1, Title = "Masterpiece", Artist = "Motionless In White" };
                var track2 = new Track { Id = 2, Title = "Voices", Artist = "MOTIONLESS IN WHITE" };
                var track3 = new Track { Id = 3, Title = "Shadow Moses", Artist = "Bring Me The Horizon" };

                setupContext.Tracks.AddRange(track1, track2, track3);
                setupContext.UserTracks.AddRange(
                    new UserTrack { Id = 1, UserId = 10, Track = track1, FilePath = "/p1.mp3" },
                    new UserTrack { Id = 2, UserId = 10, Track = track2, FilePath = "/p2.mp3" },
                    new UserTrack { Id = 3, UserId = 10, Track = track3, FilePath = "/p3.mp3" }
                );
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new TracksController(testContext);

            // Act
            var result = await controller.GetTracksByArtist(userId: 10, artistName: "motionless in white");

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var tracks = okResult!.Value as List<UserTrackDto>;
            Assert.That(tracks, Is.Not.Null);
            Assert.That(tracks!.Count, Is.EqualTo(2));
        }

        #endregion

        #region UpdateCover Tests

        [Test]
        public async Task UpdateCover_NonExistentUserTrack_ReturnsNotFound()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new TracksController(testContext);
            var dto = new UpdateCoverDto("/covers/new.jpg");

            // Act
            var result = await controller.UpdateCover(userTrackId: 404, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFound = result as NotFoundObjectResult;
            Assert.That(notFound, Is.Not.Null);
            Assert.That(notFound!.Value, Is.EqualTo("We cant find the track you are looking for."));
        }

        [Test]
        public async Task UpdateCover_ExistingUserTrack_UpdatesCoverPathAndReturnsNoContent()
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                var track = new Track { Id = 1, Title = "Title", Artist = "Artist" };
                setupContext.Tracks.Add(track);
                setupContext.UserTracks.Add(new UserTrack { Id = 10, UserId = 1, Track = track, FilePath = "/path.mp3", CoverPath = "/old_cover.jpg" });
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new TracksController(testContext);
            var dto = new UpdateCoverDto("/new_cover.jpg");

            // Act
            var result = await controller.UpdateCover(userTrackId: 10, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());

            // Database verification
            var dbUserTrack = await testContext.UserTracks.FindAsync(10);
            Assert.That(dbUserTrack, Is.Not.Null);
            Assert.That(dbUserTrack!.CoverPath, Is.EqualTo("/new_cover.jpg"));
        }

        #endregion

        #region DeleteUserTrack Tests

        [Test]
        public async Task DeleteUserTrack_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new TracksController(testContext);

            // Act
            var result = await controller.DeleteUserTrack(999, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task DeleteUserTrack_ExistingId_RemovesUserTrackAndPlaylistEntriesButKeepsTrack()
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                setupContext.Tracks.Add(new Track { Id = 1, Title = "Song", Artist = "Artist" });
                setupContext.UserTracks.Add(new UserTrack { Id = 10, UserId = 5, TrackId = 1, FilePath = "/music/song.mp3" });
                setupContext.Playlists.Add(new Playlist { Id = 20, UserId = 5, PlaylistName = "List" });
                setupContext.PlaylistTracks.Add(new PlaylistTrack { PlaylistId = 20, UserTrackId = 10 });
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new TracksController(testContext);

            // Act
            var result = await controller.DeleteUserTrack(10, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
            Assert.That(await testContext.UserTracks.AnyAsync(ut => ut.Id == 10), Is.False);
            Assert.That(await testContext.PlaylistTracks.AnyAsync(pt => pt.UserTrackId == 10), Is.False);
            Assert.That(await testContext.Tracks.AnyAsync(t => t.Id == 1), Is.True);
        }

        #endregion

        #region FilterTracks Tests

        private async Task SeedFilterDataAsync()
        {
            using var setupContext = CreateContext();

            setupContext.Genres.AddRange(
                new Genre { Id = 1, Name = "Metal" },
                new Genre { Id = 2, Name = "Pop" }
            );

            setupContext.Tracks.AddRange(
                new Track { Id = 1, Title = "Heavy", Artist = "Band A" },
                new Track { Id = 2, Title = "Mixed", Artist = "Band B" },
                new Track { Id = 3, Title = "Light", Artist = "Band C" }
            );

            setupContext.GenreScores.AddRange(
                new GenreScore { TrackId = 1, GenreId = 1, Score = 0.9 },
                new GenreScore { TrackId = 1, GenreId = 2, Score = 0.1 },
                new GenreScore { TrackId = 2, GenreId = 1, Score = 0.4 },
                new GenreScore { TrackId = 2, GenreId = 2, Score = 0.6 },
                new GenreScore { TrackId = 3, GenreId = 1, Score = 0.2 },
                new GenreScore { TrackId = 3, GenreId = 2, Score = 0.8 }
            );

            setupContext.UserTracks.AddRange(
                new UserTrack { Id = 10, UserId = 5, TrackId = 1, FilePath = "/music/1.mp3" },
                new UserTrack { Id = 11, UserId = 5, TrackId = 2, FilePath = "/music/2.mp3" },
                new UserTrack { Id = 12, UserId = 5, TrackId = 3, FilePath = "/music/3.mp3" },
                new UserTrack { Id = 20, UserId = 6, TrackId = 1, FilePath = "/music/other.mp3" }
            );

            await setupContext.SaveChangesAsync();
        }

        [Test]
        public async Task FilterTracks_SoftFilter_ReturnsTracksAboveThresholdOrderedByScore()
        {
            // Arrange
            await SeedFilterDataAsync();
            using var testContext = CreateContext();
            var controller = new TracksController(testContext);

            // Act
            var result = await controller.FilterTracks(5, new TrackFilterDto(1, false, 0.3), CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var tracks = (result as OkObjectResult)!.Value as List<UserTrackDto>;
            Assert.That(tracks, Is.Not.Null);
            Assert.That(tracks!.Count, Is.EqualTo(2));
            Assert.That(tracks[0].UserTrackId, Is.EqualTo(10));
            Assert.That(tracks[1].UserTrackId, Is.EqualTo(11));
        }

        [Test]
        public async Task FilterTracks_HardFilter_ReturnsOnlyTracksWhereGenreIsTheTopScore()
        {
            // Arrange
            await SeedFilterDataAsync();
            using var testContext = CreateContext();
            var controller = new TracksController(testContext);

            // Act
            var result = await controller.FilterTracks(5, new TrackFilterDto(2, true, 0), CancellationToken.None);

            // Assert
            var tracks = (result as OkObjectResult)!.Value as List<UserTrackDto>;
            Assert.That(tracks, Is.Not.Null);
            Assert.That(tracks!.Count, Is.EqualTo(2));
            Assert.That(tracks[0].UserTrackId, Is.EqualTo(12));
            Assert.That(tracks[1].UserTrackId, Is.EqualTo(11));
        }

        [Test]
        public async Task FilterTracks_ReturnsOnlyTracksOfTheGivenUser()
        {
            // Arrange
            await SeedFilterDataAsync();
            using var testContext = CreateContext();
            var controller = new TracksController(testContext);

            // Act
            var result = await controller.FilterTracks(6, new TrackFilterDto(1, false, 0), CancellationToken.None);

            // Assert
            var tracks = (result as OkObjectResult)!.Value as List<UserTrackDto>;
            Assert.That(tracks, Is.Not.Null);
            Assert.That(tracks!.Count, Is.EqualTo(1));
            Assert.That(tracks[0].UserTrackId, Is.EqualTo(20));
        }

        [Test]
        public async Task FilterTracks_ThresholdOutOfRange_ReturnsBadRequest()
        {
            // Arrange
            await SeedFilterDataAsync();
            using var testContext = CreateContext();
            var controller = new TracksController(testContext);

            // Act
            var result = await controller.FilterTracks(5, new TrackFilterDto(1, false, 1.5), CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task FilterTracks_UnknownGenre_ReturnsBadRequest()
        {
            // Arrange
            await SeedFilterDataAsync();
            using var testContext = CreateContext();
            var controller = new TracksController(testContext);

            // Act
            var result = await controller.FilterTracks(5, new TrackFilterDto(99, false, 0.1), CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        #endregion
    }
}