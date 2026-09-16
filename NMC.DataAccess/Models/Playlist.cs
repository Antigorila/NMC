using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.DataAccess.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PlaylistName { get; set; } = string.Empty;
        public string? CoverPath { get; set; }
    }
}
