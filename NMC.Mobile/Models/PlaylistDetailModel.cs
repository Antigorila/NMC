using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json.Serialization;

namespace NMC.Mobile.Models
{
    public class PlaylistDetailModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("playlistName")]
        public string PlaylistName { get; set; } = string.Empty;

        [JsonPropertyName("coverPath")]
        public string? CoverPath { get; set; }

        [JsonPropertyName("tracks")]
        public ObservableCollection<UserTrackModel> Tracks { get; set; } = new();
    }
}
