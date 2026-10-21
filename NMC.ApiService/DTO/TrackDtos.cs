namespace NMC.ApiService.DTO
{
    public record TrackDto
    (
        int Id,
        string Title,
        string Artist
    );

    public record UserTrackDto
    (
        int UserTrackId,
        int TrackId,
        string Title,
        string Artist,
        string FilePath,
        string? CoverPath,
        List<GenreScoreDto> GenreScores,
        List<AcousticScoreDto> AcousticScores
    );

    public record CreateUserTrackRequestDto(
        int UserId,
        string Title,
        string Artist,
        string FilePath,
        string? CoverPath
    );

    public record GenreScoreDto(int GenreId, string GenreName, double Score);
    public record AcousticScoreDto(int AttributeId, string AttributeName, double Score);
    public record UpdateCoverDto(string CoverPath);
    public record TrackFilterDto(int GenreId, bool IsHardFilter, double Threshold);
}
