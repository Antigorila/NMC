using CommunityToolkit.Mvvm.ComponentModel;

namespace NMC.Client.Models
{
    public partial class TrackModel : ObservableObject
    {
        public const string PlaceholderCover = "music_placeholder.png";

        public int UserTrackId { get; init; }

        public int TrackId { get; init; }

        public string Title { get; init; } = string.Empty;

        public string Artist { get; init; } = string.Empty;

        public string FilePath { get; init; } = string.Empty;

        public IReadOnlyList<ScoreModel> GenreScores { get; init; } = [];

        public IReadOnlyList<ScoreModel> AcousticScores { get; init; } = [];


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CoverSource))]
        private string? coverPath;

        [ObservableProperty]
        private bool isPlaying;

        public string CoverSource => string.IsNullOrWhiteSpace(CoverPath) ? PlaceholderCover : CoverPath;

        public bool IsFileAvailable => !string.IsNullOrWhiteSpace(FilePath) && File.Exists(FilePath);

        public bool IsFileMissing => !IsFileAvailable;

        public ScoreModel? TopGenre => GenreScores.Count == 0 ? null : GenreScores.MaxBy(g => g.Score);

        public string DisplayName => $"{Artist} - {Title}";
    }
}
