namespace NMC.Client.Services
{
    public class LocalFileService
    {
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
    }
}
