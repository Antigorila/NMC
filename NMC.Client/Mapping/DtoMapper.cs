using NMC.Client.DTO;
using NMC.Client.Models;

namespace NMC.Client.Mapping
{
    public static class DtoMapper
    {
        public static UserSessionModel ToModel(this AuthResponseDto dto) => new(dto.UserId, dto.Username, dto.Token);
        public static ScoreModel ToModel(this GenreScoreDto dto) => new(dto.GenreId, dto.GenreName, dto.Score);
        public static ScoreModel ToModel(this AcousticScoreDto dto) => new(dto.AttributeId, dto.AttributeName, dto.Score);
        public static CategoryModel ToCategory(this GenreScoreDto dto) => new(dto.GenreId, dto.GenreName);
        public static CategoryModel ToCategory(this AcousticScoreDto dto) => new(dto.AttributeId, dto.AttributeName);

        #region Tracks
        public static TrackModel ToModel(this UserTrackDto dto)
            => new()
            {
                UserTrackId = dto.UserTrackId,
                TrackId = dto.TrackId,
                Title = dto.Title,
                Artist = dto.Artist,
                FilePath = dto.FilePath,
                CoverPath = dto.CoverPath,
                GenreScores = MapScores(dto.GenreScores, s => s.ToModel()),
                AcousticScores = MapScores(dto.AcousticScores, s => s.ToModel())
            };


        public static TrackModel ToModel(this AnalysisUploadResponseDto dto, string filePath, string? coverPath)
            => new()
            {
                UserTrackId = dto.UserTrackId,
                TrackId = dto.TrackId,
                Title = dto.Title,
                Artist = dto.Artist,
                FilePath = filePath,
                CoverPath = coverPath,
                GenreScores = MapScores(dto.GenreScores, s => s.ToModel()),
                AcousticScores = MapScores(dto.AcousticScores, s => s.ToModel())
            };
        #endregion

        #region Playlists
        public static PlaylistModel ToModel(this PlaylistDto dto)
            => new()
            {
                Id = dto.Id,
                Name = dto.PlaylistName,
                CoverPath = dto.CoverPath,
                TrackCount = dto.TrackCount
            };

        public static PlaylistModel ToModel(this PlaylistDetailDto dto)
        {
            var model = new PlaylistModel
            {
                Id = dto.Id,
                Name = dto.PlaylistName,
                CoverPath = dto.CoverPath
            };

            foreach (var track in dto.Tracks ?? [])
            {
                model.Tracks.Add(track.ToModel());
            }

            model.TrackCount = model.Tracks.Count;
            return model;
        }
        #endregion

        private static List<ScoreModel> MapScores<T>(List<T>? source, Func<T, ScoreModel> map) => source?.Select(map).OrderByDescending(s => s.Score).ToList() ?? [];
    }
}
