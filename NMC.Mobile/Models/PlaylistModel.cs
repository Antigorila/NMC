using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.Mobile.Models
{
    public class PlaylistModel
    {
        public string Name { get; set; } = string.Empty;
        public string GenresText { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
    }
}
