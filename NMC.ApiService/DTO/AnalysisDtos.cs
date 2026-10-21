namespace NMC.ApiService.DTO
{
    public record TrackCheckDto(bool Exists);

    public class UploadTrackFormDto
    {
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string? CoverPath { get; set; }
        public IFormFile? File { get; set; }
    }
}
