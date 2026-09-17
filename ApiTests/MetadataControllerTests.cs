using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMC.ApiService.Controllers;
using NMC.ApiService.DTO;
using NMC.DataAccess;
using NMC.DataAccess.Models;

namespace ApiTests
{
    [TestFixture]
    public class MetadataControllerTests
    {
        private DbContextOptions<NmcDbContext> _dbOptions = null!;

        [SetUp]
        public void SetUp()
        {
            _dbOptions = new DbContextOptionsBuilder<NmcDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        }

        private NmcDbContext CreateContext() => new NmcDbContext(_dbOptions);

        #region GetGenres Tests

        [Test]
        public async Task GetGenres_EmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new MetadataController(testContext);

            // Act
            var result = await controller.GetGenres();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var genres = okResult!.Value as List<GenreScoreDto>;
            Assert.That(genres, Is.Not.Null);
            Assert.That(genres!.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetGenres_ExistingGenres_ReturnsAllGenresWithZeroScore()
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                setupContext.Genres.AddRange(
                    new Genre { Id = 1, Name = "Rock" },
                    new Genre { Id = 2, Name = "Pop" },
                    new Genre { Id = 3, Name = "Jazz" }
                );
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new MetadataController(testContext);

            // Act
            var result = await controller.GetGenres();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var genres = okResult!.Value as List<GenreScoreDto>;
            Assert.That(genres, Is.Not.Null);
            Assert.That(genres!.Count, Is.EqualTo(3));
            Assert.That(genres[0].GenreName, Is.EqualTo("Rock"));
            Assert.That(genres[0].Score, Is.EqualTo(0));
            Assert.That(genres[1].GenreName, Is.EqualTo("Pop"));
            Assert.That(genres[2].GenreName, Is.EqualTo("Jazz"));
        }

        #endregion

        #region GetAcousticAttributes Tests

        [Test]
        public async Task GetAcousticAttributes_EmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new MetadataController(testContext);

            // Act
            var result = await controller.GetAcousticAttributes();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var attributes = okResult!.Value as List<AcousticScoreDto>;
            Assert.That(attributes, Is.Not.Null);
            Assert.That(attributes!.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAcousticAttributes_ExistingAttributes_ReturnsAllAttributesWithZeroScore()
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                setupContext.AcousticAttributes.AddRange(
                    new AcousticAttribute { Id = 1, Name = "Acousticness" },
                    new AcousticAttribute { Id = 2, Name = "Danceability" }
                );
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new MetadataController(testContext);

            // Act
            var result = await controller.GetAcousticAttributes();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var attributes = okResult!.Value as List<AcousticScoreDto>;
            Assert.That(attributes, Is.Not.Null);
            Assert.That(attributes!.Count, Is.EqualTo(2));
            Assert.That(attributes[0].AttributeName, Is.EqualTo("Acousticness"));
            Assert.That(attributes[0].Score, Is.EqualTo(0));
            Assert.That(attributes[1].AttributeName, Is.EqualTo("Danceability"));
        }

        #endregion
    }
}