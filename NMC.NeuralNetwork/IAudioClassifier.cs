using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.NeuralNetwork
{
    public interface IAudioClassifier
    {
        Task<ClassificationResult> ClassifyAsync(string audioFilePath, IReadOnlyCollection<string> genreNames, IReadOnlyCollection<string> attributeNames, CancellationToken cancellationToken);
    }
}