using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.App.MVVM.Models
{
    public partial class PlaylistModel : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private int userId;

        [ObservableProperty]
        private UserModel user;

        [ObservableProperty]
        private string playlistName;

        [ObservableProperty]
        private string coverPath;

        public PlaylistModel(int id, int userId, UserModel user, string playlistName, string coverPath)
        {
            this.id = id;
            this.userId = userId;
            this.user = user;
            this.playlistName = playlistName;
            this.coverPath = coverPath;
        }

        public PlaylistModel(int id, int userId, string playlistName, string coverPath)
        {
            this.id = id;
            this.userId = userId;
            this.playlistName = playlistName;
            this.coverPath = coverPath;

            //todo: query
            this.User = new UserModel(0, "c-null", "c-null");
        }

        public PlaylistModel(int id, UserModel user, string playlistName, string coverPath)
        {
            this.id = id;
            this.userId = user.Id;
            this.playlistName = playlistName;
            this.coverPath = coverPath;

            this.User = user;
        }
    }
}
