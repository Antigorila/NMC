namespace NMC.Client.Services
{
    public record AudioMetadata(string? Title, string? Artist, byte[]? CoverData, string CoverExtension);

    public class AudioMetadataService
    {
        public AudioMetadata Read(string audioFilePath)
        {
            try
            {
                using TagLib.File tagFile = TagLib.File.Create(audioFilePath);

                string? title = tagFile.Tag.Title;
                string? artist = tagFile.Tag.FirstPerformer;
                byte[]? coverData = null;
                string coverExtension = ".jpg";

                if (tagFile.Tag.Pictures.Length > 0)
                {
                    TagLib.IPicture picture = tagFile.Tag.Pictures[0];
                    coverData = picture.Data.Data;

                    if (picture.MimeType == "image/png")
                    {
                        coverExtension = ".png";
                    }
                }

                return new AudioMetadata(title, artist, coverData, coverExtension);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MP3 tags could not be read: {ex.Message}");
                return new AudioMetadata(null, null, null, ".jpg");
            }
        }
    }
}
