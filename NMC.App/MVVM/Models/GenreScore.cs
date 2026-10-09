using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.App.MVVM.Models
{
    public partial class GenreScore : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private int trackId;

        [ObservableProperty]
        private TrackModel track;

        [ObservableProperty]
        private int genreId;

        [ObservableProperty]
        private GenreModel genre;

        [ObservableProperty]
        private double score;

        public GenreScore(int id, int trackId, TrackModel track, int genreId, GenreModel genre, double score)
        {
            this.id = id;
            this.trackId = trackId;
            this.track = track;
            this.genreId = genreId;
            this.genre = genre;
            this.score = score;
        }

        public GenreScore(int id, int trackId, int genreId, double score)
        {
            this.id = id;
            this.trackId = trackId;
            this.genreId = genreId;
            this.score = score;

            //todo: add query
            this.track = new TrackModel(0, "c-null", "c-null");
            this.genre = new GenreModel(0, "c-null");
        }

        public GenreScore(int id, TrackModel track, GenreModel genre, double score)
        {
            this.id = id;
            this.trackId = track.Id;
            this.genreId = genre.Id;
            this.score = score;

            this.track = track;
            this.genre = genre;
        }
    }
}
