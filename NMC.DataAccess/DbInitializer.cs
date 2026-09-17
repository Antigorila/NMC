using Microsoft.Data.SqlClient;
using NMC.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.DataAccess
{
    public static class DbInitializer
    {
        public static void Initialize(NmcDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Genres.Any())
            {
                context.Genres.AddRange(
                    new Genre { Name = "Rock" },
                    new Genre { Name = "Pop" },
                    new Genre { Name = "Electronic" },
                    new Genre { Name = "Hip-Hop" },
                    new Genre { Name = "Jazz" },
                    new Genre { Name = "Classical" },
                    new Genre { Name = "Metal" }
                );
            }

            if (!context.AnthropomorphicAttributes())
            {
                context.AcousticAttributes.AddRange(
                    new AcousticAttribute { Name = "Acousticness" },
                    new AcousticAttribute { Name = "Danceability" },
                    new AcousticAttribute { Name = "Energy" },
                    new AcousticAttribute { Name = "Instrumentalness" },
                    new AcousticAttribute { Name = "Valence" }
                );
            }

            context.SaveChanges();
        }

        private static bool AnthropomorphicAttributes(this NmcDbContext context)
        {
            return context.AcousticAttributes.Any();
        }
    }
}
