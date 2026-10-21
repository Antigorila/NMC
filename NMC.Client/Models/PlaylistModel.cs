using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NMC.Client.Models
{
    public partial class PlaylistModel : ObservableObject
    {
        public int Id { get; init; }

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CoverSource))]
        private string? coverPath;

        [ObservableProperty]
        private int trackCount;

        public ObservableCollection<TrackModel> Tracks { get; } = new();

        public string CoverSource => string.IsNullOrWhiteSpace(CoverPath) ? TrackModel.PlaceholderCover : CoverPath;
    }
}
