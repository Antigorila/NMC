namespace NMC.ApiService.DTO
{
    public record AnalysisUploadResponseDto
    (
        int TrackId,
        int UserTrackId,
        string Title,
        string Artist,
        List<GenreScoreDto> GenreScores,
        List<AcousticScoreDto> AcousticScores
    );
}
