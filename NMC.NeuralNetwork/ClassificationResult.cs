using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.NeuralNetwork
{
    public class ClassificationResult
    {
        public Dictionary<string, double> GenreScores { get; } = new();

        public Dictionary<string, double> AcousticScores { get; } = new();
    }
}