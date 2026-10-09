using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.App.MVVM.Models
{
    public partial class AcousticScoreModel : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private int trackId;

        [ObservableProperty]
        private TrackModel track;

        [ObservableProperty]
        private int attributeId;
        [ObservableProperty]
        private AcousticAttributeModel attribute;

        [ObservableProperty]
        private double score;

        public AcousticScoreModel(int id, int trackId, TrackModel track, int attributeId, AcousticAttributeModel attribute, double score)
        {
            this.id = id;
            this.trackId = trackId;
            this.track = track;
            this.attributeId = attributeId;
            this.attribute = attribute;
            this.score = score;
        }

        public AcousticScoreModel(int id, int trackId, int attributeId, double score)
        {
            this.id = id;
            this.trackId = trackId;
            this.attributeId = attributeId;
            this.score = score;

            //todo: add query
            this.track = new TrackModel(0, "c-null", "c-null");
            this.attribute = new AcousticAttributeModel(0, "c-null");
        }

        public AcousticScoreModel(int id, TrackModel track, AcousticAttributeModel attribute, double score)
        {
            this.id = id;
            this.trackId = track.Id;
            this.attributeId = attribute.Id;
            this.score = score;

            this.track = track;
            this.attribute = attribute;
        }
    }
}
