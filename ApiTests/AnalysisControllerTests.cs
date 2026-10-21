using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMC.ApiService.Controllers;
using NMC.ApiService.DTO;
using NMC.DataAccess;
using NMC.DataAccess.Models;

namespace ApiTests
{
    [TestFixture]
    public class AnalysisControllerTests
    {
        private DbContextOptions<NmcDbContext> _dbOptions = null!;
        private FakeAudioClassifier _classifier = null!;

        [SetUp]
        public void SetUp()
        {
            _dbOptions = new DbContextOptionsBuilder<NmcDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _classifier = new FakeAudioClassifier();
        }

        private NmcDbContext CreateContext() => new NmcDbContext(_dbOptions);

        private static IFormFile CreateFile(string fileName)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("fake audio content");
            return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "File", fileName);
        }

        private static UploadTrackFormDto CreateForm(int userId, string title, string artist, string filePath, string? coverPath, IFormFile? file)
        {
            return new UploadTrackFormDto
            {
                UserId = userId,
                Title = title,
                Artist = artist,
                FilePath = filePath,
                CoverPath = coverPath,
                File = file
            };
        }

        #region CheckTrack Tests

        [Test]
        public async Task CheckTrack_ExistingTrackDifferentCasing_ReturnsExistsTrue()
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                setupContext.Tracks.Add(new Track { Id = 1, Title = "Look To Windward", Artist = "Sleep Token" });
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new AnalysisController(testContext, _classifier);

            // Act
            var result = await controller.CheckTrack("  look to windward ", "SLEEP TOKEN", CancellationToken.None);

            // Assert
            var checkDto = (result as OkObjectResult)!.Value as TrackCheckDto;
            Assert.That(checkDto, Is.Not.Null);
            Assert.That(checkDto!.Exists, Is.True);
        }

        [Test]
        public async Task CheckTrack_UnknownTrack_ReturnsExistsFalse()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new AnalysisController(testContext, _classifier);

            // Act
            var result = await controller.CheckTrack("New Song", "New Artist", CancellationToken.None);

            // Assert
            var checkDto = (result as OkObjectResult)!.Value as TrackCheckDto;
            Assert.That(checkDto, Is.Not.Null);
            Assert.That(checkDto!.Exists, Is.False);
        }

        [Test]
        public async Task CheckTrack_MissingTitle_ReturnsBadRequest()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new AnalysisController(testContext, _classifier);

            // Act
            var result = await controller.CheckTrack("", "Artist", CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        #endregion

        #region UploadAndAnalyze Validation Tests

        [Test]
        public async Task UploadAndAnalyze_MissingTitleOrArtist_ReturnsBadRequest()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new AnalysisController(testContext, _classifier);
            var form = CreateForm(1, "", "Artist", "/song.mp3", null, CreateFile("song.mp3"));

            // Act
            var result = await controller.UploadAndAnalyze(form, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            Assert.That((result as BadRequestObjectResult)!.Value, Is.EqualTo("Title and Artist are strictly required."));
        }

        [Test]
        public async Task UploadAndAnalyze_GenericInvalidValues_ReturnsBadRequest()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new AnalysisController(testContext, _classifier);
            var form = CreateForm(1, "Unknown", "Artist", "/song.mp3", null, CreateFile("song.mp3"));

            // Act
            var result = await controller.UploadAndAnalyze(form, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            Assert.That((result as BadRequestObjectResult)!.Value, Is.EqualTo("Generic names like 'unknown' or 'none' are not allowed."));
        }

        [Test]
        public async Task UploadAndAnalyze_NewTrackWithoutFile_ReturnsBadRequest()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new AnalysisController(testContext, _classifier);
            var form = CreateForm(1, "New Song", "New Artist", "/song.mp3", null, null);

            // Act
            var result = await controller.UploadAndAnalyze(form, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            Assert.That(await testContext.Tracks.CountAsync(), Is.EqualTo(0));
        }

        [Test]
        public async Task UploadAndAnalyze_NewTrackWithNonMp3File_ReturnsBadRequest()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new AnalysisController(testContext, _classifier);
            var form = CreateForm(1, "New Song", "New Artist", "/song.wav", null, CreateFile("song.wav"));

            // Act
            var result = await controller.UploadAndAnalyze(form, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            Assert.That(_classifier.CallCount, Is.EqualTo(0));
        }

        #endregion

        #region UploadAndAnalyze Tests

        [Test]
        public async Task UploadAndAnalyze_NewTrack_ClassifiesAndCreatesTrackWithScoresAndUserTrack()
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                setupContext.Genres.Add(new Genre { Id = 1, Name = "Rock" });
                setupContext.AcousticAttributes.Add(new AcousticAttribute { Id = 1, Name = "Energy" });
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new AnalysisController(testContext, _classifier);
            var form = CreateForm(1, "Hypnosis", "Sleep Token", "/music/hypnosis.mp3", "/covers/hypnosis.jpg", CreateFile("hypnosis.mp3"));

            // Act
            var result = await controller.UploadAndAnalyze(form, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var userTrackDto = (result as OkObjectResult)!.Value as UserTrackDto;
            Assert.That(userTrackDto, Is.Not.Null);
            Assert.That(userTrackDto!.Title, Is.EqualTo("Hypnosis"));
            Assert.That(userTrackDto.FilePath, Is.EqualTo("/music/hypnosis.mp3"));
            Assert.That(userTrackDto.CoverPath, Is.EqualTo("/covers/hypnosis.jpg"));
            Assert.That(userTrackDto.GenreScores.Count, Is.EqualTo(1));
            Assert.That(userTrackDto.GenreScores[0].Score, Is.EqualTo(FakeAudioClassifier.FixedScore));
            Assert.That(userTrackDto.AcousticScores.Count, Is.EqualTo(1));
            Assert.That(_classifier.CallCount, Is.EqualTo(1));
            Assert.That(await testContext.UserTracks.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task UploadAndAnalyze_ExistingTrackWithoutFile_ReusesTrackAndSkipsClassification()
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                setupContext.Genres.Add(new Genre { Id = 1, Name = "Metal" });
                setupContext.Tracks.Add(new Track { Id = 200, Title = "Existing Song", Artist = "Existing Artist" });
                setupContext.GenreScores.Add(new GenreScore { TrackId = 200, GenreId = 1, Score = 0.85 });
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new AnalysisController(testContext, _classifier);
            var form = CreateForm(2, "existing song", "EXISTING ARTIST", "/music/existing.mp3", null, null);

            // Act
            var result = await controller.UploadAndAnalyze(form, CancellationToken.None);

            // Assert
            var userTrackDto = (result as OkObjectResult)!.Value as UserTrackDto;
            Assert.That(userTrackDto, Is.Not.Null);
            Assert.That(userTrackDto!.TrackId, Is.EqualTo(200));
            Assert.That(userTrackDto.GenreScores[0].Score, Is.EqualTo(0.85));
            Assert.That(_classifier.CallCount, Is.EqualTo(0));
            Assert.That(await testContext.Tracks.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task UploadAndAnalyze_ExistingUserTrack_UpdatesFilePathAndCoverPath()
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                setupContext.Tracks.Add(new Track { Id = 1, Title = "Past Self", Artist = "Sleep Token" });
                setupContext.UserTracks.Add(new UserTrack { Id = 10, UserId = 1, TrackId = 1, FilePath = "/old/path.mp3", CoverPath = "/old/cover.jpg" });
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new AnalysisController(testContext, _classifier);
            var form = CreateForm(1, "Past Self", "Sleep Token", "/new/path.mp3", "/new/cover.jpg", null);

            // Act
            var result = await controller.UploadAndAnalyze(form, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(await testContext.UserTracks.CountAsync(), Is.EqualTo(1));

            var dbUserTrack = await testContext.UserTracks.FindAsync(10);
            Assert.That(dbUserTrack!.FilePath, Is.EqualTo("/new/path.mp3"));
            Assert.That(dbUserTrack.CoverPath, Is.EqualTo("/new/cover.jpg"));
        }

        [TestCase("MOTIONLESS IN WHITE")]
        [TestCase("motionless in white")]
        [TestCase("MoTiOnLeSs In WhItE")]
        public async Task UploadAndAnalyze_ExistingArtistDifferentCasing_ReusesExistingArtistName(string inputArtistVariant)
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                setupContext.Tracks.Add(new Track { Id = 100, Title = "Another Life", Artist = "Motionless In White" });
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new AnalysisController(testContext, _classifier);
            var form = CreateForm(1, "Masterpiece", inputArtistVariant, "/music/masterpiece.mp3", null, CreateFile("masterpiece.mp3"));

            // Act
            var result = await controller.UploadAndAnalyze(form, CancellationToken.None);

            // Assert
            var userTrackDto = (result as OkObjectResult)!.Value as UserTrackDto;
            Assert.That(userTrackDto, Is.Not.Null);
            Assert.That(userTrackDto!.Artist, Is.EqualTo("Motionless In White"));
        }

        #endregion
    }
}
