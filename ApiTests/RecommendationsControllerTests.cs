using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMC.ApiService.Controllers;
using NMC.ApiService.DTO;
using NMC.DataAccess;
using NMC.DataAccess.Models;

namespace ApiTests
{
    [TestFixture]
    public class RecommendationsControllerTests
    {
        private DbContextOptions<NmcDbContext> _dbOptions = null!;

        [SetUp]
        public void SetUp()
        {
            _dbOptions = new DbContextOptionsBuilder<NmcDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        }

        private NmcDbContext CreateContext() => new NmcDbContext(_dbOptions);

        #region Generate Tests

        [Test]
        public async Task Generate_ReturnsFirstTwoTracksOfTheUser()
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                setupContext.Tracks.AddRange(
                    new Track { Id = 1, Title = "One", Artist = "A" },
                    new Track { Id = 2, Title = "Two", Artist = "B" },
                    new Track { Id = 3, Title = "Three", Artist = "C" }
                );
                setupContext.UserTracks.AddRange(
                    new UserTrack { Id = 12, UserId = 5, TrackId = 3, FilePath = "/music/3.mp3" },
                    new UserTrack { Id = 10, UserId = 5, TrackId = 1, FilePath = "/music/1.mp3" },
                    new UserTrack { Id = 11, UserId = 5, TrackId = 2, FilePath = "/music/2.mp3" },
                    new UserTrack { Id = 9, UserId = 6, TrackId = 1, FilePath = "/music/other.mp3" }
                );
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new RecommendationsController(testContext);

            // Act
            var result = await controller.Generate(5, new GenerateRecommendationDto(1), CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var tracks = (result as OkObjectResult)!.Value as List<UserTrackDto>;
            Assert.That(tracks, Is.Not.Null);
            Assert.That(tracks!.Count, Is.EqualTo(2));
            Assert.That(tracks[0].UserTrackId, Is.EqualTo(10));
            Assert.That(tracks[1].UserTrackId, Is.EqualTo(11));
        }

        [Test]
        public async Task Generate_UserWithoutTracks_ReturnsEmptyList()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new RecommendationsController(testContext);

            // Act
            var result = await controller.Generate(42, new GenerateRecommendationDto(1), CancellationToken.None);

            // Assert
            var tracks = (result as OkObjectResult)!.Value as List<UserTrackDto>;
            Assert.That(tracks, Is.Not.Null);
            Assert.That(tracks!.Count, Is.EqualTo(0));
        }

        #endregion
    }
}
