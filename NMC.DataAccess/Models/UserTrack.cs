using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.DataAccess.Models
{
    public class UserTrack
    {
        public int Id { get; set; }
        public int TrackId { get; set; }
        public int UserId { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string? CoverPath { get; set; }
    }
}
