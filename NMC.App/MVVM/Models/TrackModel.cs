using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.App.MVVM.Models
{
    public partial class TrackModel : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string artist;

        public TrackModel(int id, string title, string artist)
        {
            this.id = id;
            this.title = title;
            this.artist = artist;
        }
    }
}
