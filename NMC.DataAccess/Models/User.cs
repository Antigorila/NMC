using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.DataAccess.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<UserTrack> UserTracks { get; set; } = new List<UserTrack>();
        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
    }
}
