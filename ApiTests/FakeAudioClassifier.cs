using NMC.CNTK;

namespace ApiTests
{
    public class FakeAudioClassifier : IAudioClassifier
    {
        public const double FixedScore = 0.5;

        public int CallCount { get; private set; }

        public Task<ClassificationResult> ClassifyAsync(string audioFilePath, IReadOnlyCollection<string> genreNames, IReadOnlyCollection<string> attributeNames, CancellationToken cancellationToken)
        {
            CallCount++;

            var result = new ClassificationResult();

            foreach (string genreName in genreNames)
            {
                result.GenreScores[genreName] = FixedScore;
            }

            foreach (string attributeName in attributeNames)
            {
                result.AcousticScores[attributeName] = FixedScore;
            }

            return Task.FromResult(result);
        }
    }
}
