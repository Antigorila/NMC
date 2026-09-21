using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NMC.Mobile.Models
{
    public class AuthResponse
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;
    }
}