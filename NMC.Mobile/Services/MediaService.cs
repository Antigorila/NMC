using System;
using System.Collections.Generic;
using System.Text;
using TagLib;

namespace NMC.Mobile.Services
{
    public class MediaMetadata
    {
        public string? Title { get; set; }
        public string? Artist { get; set; }
        public string AudioFilePath { get; set; } = string.Empty;
        public string? ExtractedCoverPath { get; set; }
    }

    public class MediaService
    {
        public async Task<MediaMetadata?> PickAudioFileAsync()
        {
            try
            {
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.mp3", "public.audio" } },
                    { DevicePlatform.Android, new[] { "audio/mpeg", "audio/mp3", "audio/*" } },
                    { DevicePlatform.WinUI, new[] { ".mp3", ".wav" } },
                    { DevicePlatform.MacCatalyst, new[] { "mp3", "wav" } }
                });

                var options = new PickOptions
                {
                    PickerTitle = "Select an Audio File",
                    FileTypes = customFileType
                };

                var result = await FilePicker.Default.PickAsync(options);
                if (result == null) return null;

                var localAudioPath = await SaveFileToLocalFolderAsync(result.FullPath, "tracks", result.FileName);

                var metadata = new MediaMetadata { AudioFilePath = localAudioPath };

                try
                {
                    using var tagFile = TagLib.File.Create(localAudioPath);
                    metadata.Title = tagFile.Tag.Title;
                    metadata.Artist = tagFile.Tag.FirstPerformer;

                    if (tagFile.Tag.Pictures.Length > 0)
                    {
                        var pic = tagFile.Tag.Pictures[0];
                        var imgBytes = pic.Data.Data;
                        var coverName = $"cover_{Guid.NewGuid()}.jpg";
                        metadata.ExtractedCoverPath = await SaveBytesToLocalFolderAsync(imgBytes, "covers", coverName);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to read MP3 tags: {ex.Message}");
                }

                return metadata;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"File pick error: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> PickCoverImageAsync()
        {
            try
            {
                var results = await MediaPicker.Default.PickPhotosAsync();
                var result = results?.FirstOrDefault();

                if (result == null) return null;

                var fileName = $"cover_{Guid.NewGuid()}{Path.GetExtension(result.FileName)}";
                return await SaveFileToLocalFolderAsync(result.FullPath, "covers", fileName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Cover pick error: {ex.Message}");
                return null;
            }
        }

        private async Task<string> SaveFileToLocalFolderAsync(string sourcePath, string subFolder, string fileName)
        {
            var targetDirectory = Path.Combine(FileSystem.AppDataDirectory, subFolder);
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            var targetPath = Path.Combine(targetDirectory, fileName);
            using var sourceStream = System.IO.File.OpenRead(sourcePath);
            using var targetStream = System.IO.File.Create(targetPath);
            await sourceStream.CopyToAsync(targetStream);

            return targetPath;
        }

        private async Task<string> SaveBytesToLocalFolderAsync(byte[] bytes, string subFolder, string fileName)
        {
            var targetDirectory = Path.Combine(FileSystem.AppDataDirectory, subFolder);
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            var targetPath = Path.Combine(targetDirectory, fileName);
            await System.IO.File.WriteAllBytesAsync(targetPath, bytes);
            return targetPath;
        }
    }
}