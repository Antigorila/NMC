using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NMC.Mobile.Models
{
    public class PlaylistModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("playlistName")]
        public string PlaylistName { get; set; } = string.Empty;

        [JsonPropertyName("coverPath")]
        public string? CoverPath { get; set; }

        [JsonPropertyName("trackCount")]
        public int TrackCount { get; set; }
    }
}