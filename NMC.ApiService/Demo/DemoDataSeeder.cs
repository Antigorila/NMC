using Microsoft.AspNetCore.Identity;
using NMC.DataAccess;
using NMC.DataAccess.Models;

namespace NMC.ApiService.Demo
{
    public static class DemoDataSeeder
    {
        private record DemoTrack(string Key, string Title, string Artist, string FilePath, string? CoverPath);
        private record DemoPlaylist(string Name, string? CoverPath, string[] TrackKeys);
        private record DemoUser(string Username, string Password, string[] TrackKeys, DemoPlaylist[] Playlists);

        private static readonly DemoTrack[] Tracks =
        {
            new DemoTrack("atlas_bittersleep", "BITTERSLEEP", "ATLAS", "demo/mp3/atlas_bittersleep.mp3", "demo/covers/atlas_cover.jpg"),
            new DemoTrack("lornashore_and_i_return_to_nothingness", "...And I Return to Nothingness", "Lorna Shore", "demo/mp3/lornashore_and_i_return_to_nothingness.mp3", "demo/covers/lornashore_cover.jpg"),
            new DemoTrack("badomens_artificial_suicide", "Artificial Suicide", "Bad Omens", "demo/mp3/badomens_artificial_suicide.mp3", "demo/covers/badomens_thedeathofpeaceofmind_cover.png"),
            new DemoTrack("badomens_dethrone", "Dethrone", "Bad Omens", "demo/mp3/badomens_dethrone.mp3", "demo/covers/badomens_findinggodbeforegodfindsme_cover.jpg"),
            new DemoTrack("badomens_dying_to_love", "Dying To Love", "Bad Omens", "demo/mp3/badomens_dying_to_love.mp3", null),
            new DemoTrack("badomens_like_a_villain", "Like A Villain", "Bad Omens", "demo/mp3/badomens_like_a_villain.mp3", "demo/covers/badomens_thedeathofpeaceofmind_cover.png"),
            new DemoTrack("badomens_the_death_of_peace_of_mind", "The Death of Peace of Mind", "Bad Omens", "demo/mp3/badomens_the_death_of_peace_of_mind.mp3", "demo/covers/badomens_thedeathofpeaceofmind_cover.png"),
            new DemoTrack("badomens_the_grey", "The Grey", "Bad Omens", "demo/mp3/badomens_the_grey.mp3", "demo/covers/badomens_thedeathofpeaceofmind_cover.png"),
            new DemoTrack("bmth_throne", "Throne", "Bring Me The Horizon", "demo/mp3/bmth_throne.mp3", "demo/covers/bringmethehorizon_thatsthespirit_cover.jpg"),
            new DemoTrack("currents_remember_me", "Remember Me", "Currents", "demo/mp3/currents_remember_me.mp3", "demo/covers/currents_thedeathweseek_cover.jpg"),
            new DemoTrack("bmth_doomed", "Doomed", "Bring Me The Horizon", "demo/mp3/bmth_doomed.mp3", "demo/covers/bringmethehorizon_thatsthespirit_cover.jpg"),
            new DemoTrack("lornashore_to_the_hellfire", "To the Hellfire", "Lorna Shore", "demo/mp3/lornashore_to_the_hellfire.mp3", "demo/covers/lornashore_cover.jpg"),
            new DemoTrack("miw_count_back_from_zero", "Count Back From Zero", "Motionless In White", "demo/mp3/miw_count_back_from_zero.mp3", null),
            new DemoTrack("miw_disguise", "Disguise", "Motionless In White", "demo/mp3/miw_disguise.mp3", "demo/covers/motionlessinwhite_disguise_cover.jpg"),
            new DemoTrack("miw_playing_god", "Playing God (ft. Corey Taylor)", "Motionless In White", "demo/mp3/miw_playing_god.mp3", null),
            new DemoTrack("miw_porcelain", "Porcelain", "Motionless In White", "demo/mp3/miw_porcelain.mp3", "demo/covers/motionlessinwhite_scoringtheendoftheworld_cover.png"),
            new DemoTrack("miw_rip", "R.I.P. (feat. Skylar Grey)", "Motionless In White", "demo/mp3/miw_rip.mp3", null),
            new DemoTrack("miw_reincarnate_reincarnated", "Reincarnate Reincarnated", "Motionless In White", "demo/mp3/miw_reincarnate_reincarnated.mp3", "demo/covers/motionlessinwhite_reincarnate_cover.jpg"),
            new DemoTrack("miw_voices", "Voices", "Motionless In White", "demo/mp3/miw_voices.mp3", "demo/covers/motionlessinwhite_graveyardshift_cover.jpg"),
            new DemoTrack("sleeptoken_damocles", "Damocles", "Sleep Token", "demo/mp3/sleeptoken_damocles.mp3", "demo/covers/sleeptoken_eveninarcadia_cover.jpg"),
            new DemoTrack("sleeptoken_emergence", "Emergence", "Sleep Token", "demo/mp3/sleeptoken_emergence.mp3", "demo/covers/sleeptoken_eveninarcadia_cover.jpg"),
            new DemoTrack("sleeptoken_even_in_arcadia", "Even In Arcadia", "Sleep Token", "demo/mp3/sleeptoken_even_in_arcadia.mp3", "demo/covers/sleeptoken_eveninarcadia_cover.jpg"),
            new DemoTrack("sleeptoken_gods", "Gods", "Sleep Token", "demo/mp3/sleeptoken_gods.mp3", "demo/covers/sleeptoken_gods_cover.jpg"),
            new DemoTrack("sleeptoken_take_me_back_to_eden", "Take Me Back To Eden", "Sleep Token", "demo/mp3/sleeptoken_take_me_back_to_eden.mp3", "demo/covers/sleeptoken_takemebacktoeden_cover.jpg"),
            new DemoTrack("badomens_specter", "Specter", "Bad Omens", "demo/mp3/badomens_specter.mp3", "demo/covers/badomens_specter_cover.jpg"),
            new DemoTrack("spiritbox_no_loss_no_love", "No Loss, No Love", "Spiritbox", "demo/mp3/spiritbox_no_loss_no_love.mp3", "demo/covers/spiritbox_tsunamisea_cover.png"),
            new DemoTrack("spiritbox_sun_killer", "Sun Killer", "Spiritbox", "demo/mp3/spiritbox_sun_killer.mp3", "demo/covers/spiritbox_eternalblue_cover.png")
        };

        private static readonly DemoUser[] Users =
        {
            new DemoUser("anna", "anna123",
                new[] { "sleeptoken_damocles", "sleeptoken_emergence", "sleeptoken_even_in_arcadia", "sleeptoken_gods", "sleeptoken_take_me_back_to_eden", "spiritbox_no_loss_no_love", "spiritbox_sun_killer", "atlas_bittersleep" },
                new[]
                {
                    new DemoPlaylist("Even In Arcadia", "demo/covers/playlistcover_1.jpg", new[] { "sleeptoken_damocles", "sleeptoken_emergence", "sleeptoken_even_in_arcadia" }),
                    new DemoPlaylist("Spiritbox favourites", "demo/covers/playlistcover_2.png", new[] { "spiritbox_no_loss_no_love", "spiritbox_sun_killer" }),
                    new DemoPlaylist("Late night", "demo/covers/playlistcover_3.jpg", new[] { "sleeptoken_take_me_back_to_eden", "sleeptoken_gods", "atlas_bittersleep" })
                }),
            new DemoUser("bence", "bence123",
                new[] { "badomens_artificial_suicide", "badomens_dethrone", "badomens_dying_to_love", "badomens_like_a_villain", "badomens_the_death_of_peace_of_mind", "badomens_the_grey", "badomens_specter", "bmth_throne", "bmth_doomed" },
                new[]
                {
                    new DemoPlaylist("Bad Omens essentials", "demo/covers/playlistcover_4.png", new[] { "badomens_artificial_suicide", "badomens_like_a_villain", "badomens_the_death_of_peace_of_mind", "badomens_the_grey" }),
                    new DemoPlaylist("Gym", "demo/covers/playlistcover_5.jpg", new[] { "bmth_throne", "bmth_doomed", "badomens_dethrone", "badomens_specter" })
                }),
            new DemoUser("csilla", "csilla123",
                new[] { "miw_count_back_from_zero", "miw_disguise", "miw_playing_god", "miw_porcelain", "miw_rip", "miw_reincarnate_reincarnated", "miw_voices", "badomens_like_a_villain", "sleeptoken_gods" },
                new[]
                {
                    new DemoPlaylist("Motionless", "demo/covers/playlistcover_6.jpg", new[] { "miw_count_back_from_zero", "miw_disguise", "miw_porcelain", "miw_reincarnate_reincarnated", "miw_voices" })
                }),
            new DemoUser("david", "david123",
                new[] { "lornashore_and_i_return_to_nothingness", "lornashore_to_the_hellfire", "currents_remember_me", "atlas_bittersleep", "miw_voices", "miw_playing_god", "spiritbox_sun_killer" },
                new[]
                {
                    new DemoPlaylist("Deathcore", "demo/covers/playlistcover_7.webp", new[] { "lornashore_and_i_return_to_nothingness", "lornashore_to_the_hellfire" }),
                    new DemoPlaylist("Mixed", "demo/covers/playlistcover_1.jpg", new[] { "currents_remember_me", "atlas_bittersleep", "miw_playing_god", "spiritbox_sun_killer" })
                }),
            new DemoUser("eszter", "eszter123",
                new[] { "badomens_the_death_of_peace_of_mind", "sleeptoken_even_in_arcadia", "bmth_throne", "miw_disguise", "spiritbox_no_loss_no_love", "currents_remember_me", "sleeptoken_take_me_back_to_eden" },
                new[]
                {
                    new DemoPlaylist("Favourites", "demo/covers/playlistcover_2.png", new[] { "badomens_the_death_of_peace_of_mind", "sleeptoken_even_in_arcadia", "miw_disguise", "spiritbox_no_loss_no_love" }),
                    new DemoPlaylist("Chill", null, new[] { "sleeptoken_take_me_back_to_eden", "currents_remember_me" }),
                    new DemoPlaylist("Empty playlist", "demo/covers/playlistcover_3.jpg", new string[0])
                })
        };

        public static void Seed(NmcDbContext context)
        {
            if (context.Users.Any())
            {
                return;
            }

            List<Genre> genres = context.Genres.ToList();
            List<AcousticAttribute> attributes = context.AcousticAttributes.ToList();

            Dictionary<string, Track> tracksByKey = CreateTracks(context, genres, attributes);
            context.SaveChanges();

            var passwordHasher = new PasswordHasher<User>();

            foreach (DemoUser demoUser in Users)
            {
                var user = new User { Username = demoUser.Username };
                user.PasswordHash = passwordHasher.HashPassword(user, demoUser.Password);
                context.Users.Add(user);

                var userTracksByKey = new Dictionary<string, UserTrack>();
                foreach (string key in demoUser.TrackKeys)
                {
                    DemoTrack demoTrack = FindTrack(key);
                    var userTrack = new UserTrack
                    {
                        User = user,
                        Track = tracksByKey[key],
                        FilePath = demoTrack.FilePath,
                        CoverPath = demoTrack.CoverPath
                    };
                    context.UserTracks.Add(userTrack);
                    userTracksByKey.Add(key, userTrack);
                }

                foreach (DemoPlaylist demoPlaylist in demoUser.Playlists)
                {
                    var playlist = new Playlist
                    {
                        User = user,
                        PlaylistName = demoPlaylist.Name,
                        CoverPath = demoPlaylist.CoverPath
                    };
                    context.Playlists.Add(playlist);

                    foreach (string key in demoPlaylist.TrackKeys)
                    {
                        context.PlaylistTracks.Add(new PlaylistTrack
                        {
                            Playlist = playlist,
                            UserTrack = userTracksByKey[key]
                        });
                    }
                }
            }

            context.SaveChanges();
        }

        #region Support Methods
        private static Dictionary<string, Track> CreateTracks(NmcDbContext context, List<Genre> genres, List<AcousticAttribute> attributes)
        {
            var tracksByKey = new Dictionary<string, Track>();

            for (int i = 0; i < Tracks.Length; i++)
            {
                DemoTrack demoTrack = Tracks[i];
                var random = new Random(i + 1);
                var track = new Track
                {
                    Title = demoTrack.Title,
                    Artist = demoTrack.Artist
                };

                double[] genreWeights = new double[genres.Count];
                double weightSum = 0;
                for (int g = 0; g < genres.Count; g++)
                {
                    genreWeights[g] = GenreBase(genres[g].Name) + random.NextDouble() * 0.15;
                    weightSum += genreWeights[g];
                }

                for (int g = 0; g < genres.Count; g++)
                {
                    track.GenreScores.Add(new GenreScore { GenreId = genres[g].Id, Score = genreWeights[g] / weightSum });
                }

                foreach (AcousticAttribute attribute in attributes)
                {
                    track.AcousticScores.Add(new AcousticScore { AttributeId = attribute.Id, Score = AcousticValue(attribute.Name, random) });
                }

                context.Tracks.Add(track);
                tracksByKey.Add(demoTrack.Key, track);
            }

            return tracksByKey;
        }

        private static DemoTrack FindTrack(string key)
        {
            foreach (DemoTrack demoTrack in Tracks)
            {
                if (demoTrack.Key == key)
                {
                    return demoTrack;
                }
            }

            throw new InvalidOperationException($"Unknown demo track key: {key}");
        }

        private static double GenreBase(string genre)
        {
            switch (genre)
            {
                case "Metal":
                    return 1.0;
                case "Rock":
                    return 0.5;
                case "Electronic":
                    return 0.15;
                case "Pop":
                    return 0.1;
                default:
                    return 0.02;
            }
        }

        private static double AcousticValue(string attribute, Random random)
        {
            switch (attribute)
            {
                case "Energy":
                    return 0.7 + random.NextDouble() * 0.28;
                case "Danceability":
                    return 0.2 + random.NextDouble() * 0.4;
                case "Valence":
                    return 0.1 + random.NextDouble() * 0.4;
                case "Instrumentalness":
                    return 0.05 + random.NextDouble() * 0.25;
                default:
                    return random.NextDouble() * 0.15;
            }
        }
        #endregion
    }
}
