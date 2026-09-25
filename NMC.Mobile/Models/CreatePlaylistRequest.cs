using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NMC.Mobile.Models
{
    public class CreatePlaylistRequest
    {
        [JsonPropertyName("playlistName")]
        public string PlaylistName { get; set; } = string.Empty;

        [JsonPropertyName("coverPath")]
        public string? CoverPath { get; set; }

        public CreatePlaylistRequest(string playlistName, string? coverPath = null)
        {
            PlaylistName = playlistName;
            CoverPath = coverPath;
        }
    }
}