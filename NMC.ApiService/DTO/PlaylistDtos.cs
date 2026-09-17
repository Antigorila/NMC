namespace NMC.ApiService.DTO
{
    public record PlaylistDto
    (
        int Id,
        int UserId,
        string PlaylistName,
        string? CoverPath,
        int TrackCount
    );

    public record PlaylistDetailDto
    (
        int Id,
        int UserId,
        string PlaylistName,
        string? CoverPath,
        List<UserTrackDto> Tracks
    );

    public record CreatePlaylistDto(string PlaylistName, string? CoverPath);
    public record AddTrackToPlaylistDto(int UserTrackId);
}
