using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.DataAccess.Models
{
    public class AcousticScore
    {
        public int Id { get; set; }
        public int TrackId { get; set; }
        public int AttributeId { get; set; }
        public double Score { get; set; }

        public Track Track { get; set; } = null!;
        public AcousticAttribute Attribute { get; set; } = null!;
    }
}
