using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.DataAccess.Models
{
    public class GenreScore
    {
        public int Id { get; set; }
        public int TrackId { get; set; }
        public int GenreId { get; set; }
        public double Score { get; set; }
    }
}
