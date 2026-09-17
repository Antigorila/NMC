using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMC.ApiService.Controllers;
using NMC.ApiService.DTO;
using NMC.DataAccess;
using NMC.DataAccess.Models;
using NUnit.Framework;

namespace ApiTests
{
    [TestFixture]
    public class AnalysisControllerTests
    {
        private DbContextOptions<NmcDbContext> _dbOptions = null!;

        [SetUp]
        public void SetUp()
        {
            _dbOptions = new DbContextOptionsBuilder<NmcDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        }

        private NmcDbContext CreateContext() => new NmcDbContext(_dbOptions);

        [Test]
        public async Task UploadAndAnalyze_NewTrack_CreatesTrackAndScoresAndUserTrack()
        {
            // Arrange
            using (var context = CreateContext())
            {
                context.Genres.Add(new Genre { Id = 1, Name = "Rock" });
                context.AcousticAttributes.Add(new AcousticAttribute { Id = 1, Name = "Acousticness" });
                await context.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new AnalysisController(testContext);

            // Act
            var result = await controller.UploadAndAnalyze(
                userId: 10,
                title: "Even In Arcadia",
                artist: "Sleep Token",
                filePath: "/music/SleepToken.mp3",
                CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var response = okResult!.Value as AnalysisUploadResponseDto;
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Title, Is.EqualTo("Even In Arcadia"));
            Assert.That(response.Artist, Is.EqualTo("Sleep Token"));
            Assert.That(response.GenreScores.Count, Is.EqualTo(1));
            Assert.That(response.GenreScores[0].GenreName, Is.EqualTo("Rock"));
            Assert.That(response.AcousticScores.Count, Is.EqualTo(1));
            Assert.That(response.AcousticScores[0].AttributeName, Is.EqualTo("Acousticness"));

            Assert.That(await testContext.Tracks.CountAsync(), Is.EqualTo(1));
            Assert.That(await testContext.UserTracks.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task UploadAndAnalyze_ExistingTrack_ReusesTrackAndDoesNotDuplicate()
        {
            // Arrange
            using (var context = CreateContext())
            {
                var genre = new Genre { Id = 1, Name = "Pop" };
                var attribute = new AcousticAttribute { Id = 1, Name = "Danceability" };

                var existingTrack = new Track
                {
                    Title = "Existing Song",
                    Artist = "Existing Artist",
                    GenreScores = new List<GenreScore> { new GenreScore { Genre = genre, Score = 0.85 } },
                    AcousticScores = new List<AcousticScore> { new AcousticScore { Attribute = attribute, Score = 0.42 } }
                };

                context.Genres.Add(genre);
                context.AcousticAttributes.Add(attribute);
                context.Tracks.Add(existingTrack);
                await context.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new AnalysisController(testContext);

            // Act
            var result = await controller.UploadAndAnalyze(
                userId: 20,
                title: "Existing Song",
                artist: "Existing Artist",
                filePath: "/music/existing.mp3",
                CancellationToken.None);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var response = okResult!.Value as AnalysisUploadResponseDto;
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Title, Is.EqualTo("Existing Song"));
            Assert.That(response.GenreScores.Count, Is.EqualTo(1));
            Assert.That(response.GenreScores[0].Score, Is.EqualTo(0.85));

            Assert.That(await testContext.Tracks.CountAsync(), Is.EqualTo(1));
            Assert.That(await testContext.UserTracks.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public void UploadAndAnalyze_CancelledToken_ThrowsOperationCanceledException()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new AnalysisController(testContext);
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await controller.UploadAndAnalyze(
                    userId: 1,
                    title: "Title",
                    artist: "Artist",
                    filePath: "/path.mp3",
                    cancellationToken: cts.Token);
            });
        }

        [Test]
        public async Task UploadAndAnalyze_SameUserUploadsSameTrackTwice_CreatesTwoUserTrackEntriesForSameTrack()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new AnalysisController(testContext);

            // Act 1
            await controller.UploadAndAnalyze(1, "Repeat Song", "Artist", "/path1.mp3", CancellationToken.None);

            // Act 2
            await controller.UploadAndAnalyze(1, "Repeat Song", "Artist", "/path2.mp3", CancellationToken.None);

            // Assert
            Assert.That(await testContext.Tracks.CountAsync(), Is.EqualTo(1));
            Assert.That(await testContext.UserTracks.CountAsync(), Is.EqualTo(2));
        }

        [Test]
        public async Task UploadAndAnalyze_EmptyMetadataTables_ReturnsResponseWithEmptyScoresList()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new AnalysisController(testContext);

            // Act
            var result = await controller.UploadAndAnalyze(1, "Solo Song", "Solo Artist", "/path.mp3", CancellationToken.None);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var response = okResult!.Value as AnalysisUploadResponseDto;
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.GenreScores.Count, Is.EqualTo(0));
            Assert.That(response.AcousticScores.Count, Is.EqualTo(0));
        }
    }
}