using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NMC.Mobile.Models
{
    public class UserTrackModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("trackId")]
        public int TrackId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("artist")]
        public string Artist { get; set; } = string.Empty;

        [JsonPropertyName("filePath")]
        public string FilePath { get; set; } = string.Empty;

        [JsonPropertyName("coverPath")]
        public string? CoverPath { get; set; }
    }
}
