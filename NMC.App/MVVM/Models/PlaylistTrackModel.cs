using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.App.MVVM.Models
{
    public partial class PlaylistTrackModel : ObservableObject
    {
        [ObservableProperty]
        private int userTrackId;

        [ObservableProperty]
        private UserTrackModel userTrack;

        [ObservableProperty]
        private int playlistId;

        [ObservableProperty]
        private PlaylistModel playlist;

        public PlaylistTrackModel(int userTrackId, UserTrackModel userTrack, int playlistId, PlaylistModel playlist)
        {
            this.userTrackId = userTrackId;
            this.userTrack = userTrack;
            this.playlistId = playlistId;
            this.playlist = playlist;
        }

        public PlaylistTrackModel(int userTrackId, int playlistId)
        {
            this.userTrackId = userTrackId;
            this.playlistId = playlistId;

            //todo: query
            this.userTrack = new UserTrackModel(0, 0, 0, "c-null", "c-null");
            this.playlist = new PlaylistModel(0, 0, "c-null", "c-null");
        }

        public PlaylistTrackModel(UserTrackModel userTrack, PlaylistModel playlist)
        {
            this.userTrackId = userTrack.Id;
            this.playlistId = playlist.Id;

            this.userTrack = userTrack;
            this.playlist = playlist;
        }
    }
}
