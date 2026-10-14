using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.App.MVVM.Models
{
    public partial class UserTrackModel : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private int trackId;

        [ObservableProperty]
        private TrackModel track;

        [ObservableProperty]
        private int userId;

        [ObservableProperty]
        private UserModel user;

        [ObservableProperty]
        private string filePath;

        [ObservableProperty]
        private string coverPath;

        public UserTrackModel(int id, int trackId, TrackModel track, int userId, UserModel user, string filePath, string coverPath)
        {
            this.id = id;
            this.trackId = trackId;
            this.track = track;
            this.userId = userId;
            this.user = user;
            this.filePath = filePath;
            this.coverPath = coverPath;
        }

        public UserTrackModel(int id, int trackId, int userId, string filePath, string coverPath)
        {
            this.id = id;
            this.trackId = trackId;
            this.userId = userId;
            this.filePath = filePath;
            this.coverPath = coverPath;

            //todo: query
            this.track = new TrackModel(0, "c-null", "c-null");
            this.user = new UserModel(0, "c-null", "c-null");
        }

        public UserTrackModel(int id, TrackModel track, UserModel user, string filePath, string coverPath)
        {
            this.id = id;
            this.trackId = track.Id;
            this.userId = user.Id;
            this.filePath = filePath;
            this.coverPath = coverPath;

            this.track = track;
            this.user = user;
        }
    }
}
