namespace NMC.Client.Services
{
    public class MediaPickerService
    {
        public async Task<FileResult?> PickAudioFileAsync()
        {
            try
            {
                var fileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "audio/mpeg" } },
                    { DevicePlatform.WinUI, new[] { ".mp3" } },
                    { DevicePlatform.iOS, new[] { "public.mp3" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.mp3" } }
                });

                var options = new PickOptions
                {
                    PickerTitle = "Select an MP3 file",
                    FileTypes = fileTypes
                };

                return await FilePicker.Default.PickAsync(options);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Audio file pick failed: {ex.Message}");
                return null;
            }
        }

        public async Task<FileResult?> PickImageAsync()
        {
            try
            {
                List<FileResult>? results = await MediaPicker.Default.PickPhotosAsync();

                if (results is null || results.Count == 0)
                {
                    return null;
                }

                return results[0];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Image pick failed: {ex.Message}");
                return null;
            }
        }
    }
}
