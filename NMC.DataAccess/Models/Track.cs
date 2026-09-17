using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.DataAccess.Models
{
    public class Track
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;

        public ICollection<UserTrack> UserTracks { get; set; } = new List<UserTrack>();
        public ICollection<GenreScore> GenreScores { get; set; } = new List<GenreScore>();
        public ICollection<AcousticScore> AcousticScores { get; set; } = new List<AcousticScore>();
    }
}
