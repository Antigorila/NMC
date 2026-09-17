using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.DataAccess.Models
{
    public class PlaylistTrack
    {
        public int UserTrackId { get; set; }
        public int PlaylistId { get; set; }

        public UserTrack UserTrack { get; set; } = null!;
        public Playlist Playlist { get; set; } = null!;
    }
}
