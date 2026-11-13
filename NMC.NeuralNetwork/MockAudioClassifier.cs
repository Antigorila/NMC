using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.NeuralNetwork
{
    public class MockAudioClassifier : IAudioClassifier
    {
        public Task<ClassificationResult> ClassifyAsync(string audioFilePath, IReadOnlyCollection<string> genreNames, IReadOnlyCollection<string> attributeNames, CancellationToken cancellationToken)
        {
            var result = new ClassificationResult();

            AddNormalizedGenreScores(result, genreNames);
            AddAcousticScores(result, attributeNames);

            return Task.FromResult(result);
        }

        #region Support Methods
        private static void AddNormalizedGenreScores(ClassificationResult result, IReadOnlyCollection<string> genreNames)
        {
            var rawScores = new Dictionary<string, double>();
            double sum = 0;

            foreach (string genreName in genreNames)
            {
                double value = Random.Shared.NextDouble();
                rawScores[genreName] = value;
                sum += value;
            }

            foreach (KeyValuePair<string, double> rawScore in rawScores)
            {
                double normalized = 0;
                if (sum > 0)
                {
                    normalized = rawScore.Value / sum;
                }

                result.GenreScores[rawScore.Key] = Math.Round(normalized, 2);
            }
        }

        private static void AddAcousticScores(ClassificationResult result, IReadOnlyCollection<string> attributeNames)
        {
            foreach (string attributeName in attributeNames)
            {
                result.AcousticScores[attributeName] = Math.Round(Random.Shared.NextDouble(), 2);
            }
        }
        #endregion
    }
}