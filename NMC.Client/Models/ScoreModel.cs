namespace NMC.Client.Models
{
    public record ScoreModel(int Id, string Name, double Score)
    {
        public int Percent => (int)Math.Round(Math.Clamp(Score, 0d, 1d) * 100);
    }
}
