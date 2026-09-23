using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NMC.Mobile.Models
{
    public class CreateUserTrackRequest
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("artist")]
        public string Artist { get; set; } = string.Empty;

        [JsonPropertyName("filePath")]
        public string FilePath { get; set; } = string.Empty;

        [JsonPropertyName("coverPath")]
        public string? CoverPath { get; set; }

        public CreateUserTrackRequest(int userId, string title, string artist, string filePath, string? coverPath = null)
        {
            UserId = userId;
            Title = title;
            Artist = artist;
            FilePath = filePath;
            CoverPath = coverPath;
        }
    }
}