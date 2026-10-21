namespace NMC.Client.Services
{
    public class LocalFileService
    {
        public const string TracksFolder = "tracks";
        public const string CoversFolder = "covers";

        public async Task<string?> CopyToAppFolderAsync(FileResult file, string subFolder)
        {
            try
            {
                string targetPath = CreateTargetPath(subFolder, Path.GetExtension(file.FileName));

                using (Stream source = await file.OpenReadAsync())
                using (FileStream target = File.Create(targetPath))
                {
                    await source.CopyToAsync(target);
                }

                return targetPath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"File could not be copied: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> SaveToAppFolderAsync(byte[] data, string subFolder, string extension)
        {
            try
            {
                string targetPath = CreateTargetPath(subFolder, extension);
                await File.WriteAllBytesAsync(targetPath, data);

                return targetPath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"File could not be saved: {ex.Message}");
                return null;
            }
        }

        public void DeleteAppFile(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            if (!filePath.StartsWith(FileSystem.AppDataDirectory, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"File could not be deleted: {ex.Message}");
            }
        }

        #region Support Methods
        private static string CreateTargetPath(string subFolder, string extension)
        {
            string directory = Path.Combine(FileSystem.AppDataDirectory, subFolder);
            Directory.CreateDirectory(directory);

            return Path.Combine(directory, $"{Guid.NewGuid()}{extension}");
        }
        #endregion
    }
}
