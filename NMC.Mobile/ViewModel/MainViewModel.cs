using CommunityToolkit.Mvvm.ComponentModel;
using NMC.Mobile.Helper;
using NMC.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NMC.Mobile.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<PlaylistModel> Playlists { get; set; } = new();
        public AcousticGraphDrawable GraphDrawable { get; } = new();

        public MainViewModel()
        {
            //test dummy data
            Playlists.Add(new PlaylistModel { Name = "Chill Vibes", GenresText = "Lo-Fi, Ambient, Electronic" });
            Playlists.Add(new PlaylistModel { Name = "Workout Beats", GenresText = "Rock, Metal, Synthwave" });
            Playlists.Add(new PlaylistModel { Name = "Night Drive", GenresText = "Retrowave, Pop" });
        }
    }
}
