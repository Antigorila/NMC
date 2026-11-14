/*
 * DemoDataService - makes the bundled demo files available on the device.
 *
 * EnsureFilesAsync  copies every file listed in Resources/Raw/Demo/manifest.txt into
 *                   AppDataDirectory/demo once; files that already exist are skipped.
 * ResolvePath       turns a "demo/..." path stored by the server into a full local path;
 *                   any other path is returned unchanged.
 * IsDemoFile        true for files inside the demo folder, these are shared and never deleted.
 */

namespace NMC.Client.Services
{
    public class DemoDataService
    {
        public const string DemoPrefix = "demo/";
        private const string ManifestFile = "Demo/manifest.txt";

        public static string DemoFolder => Path.Combine(FileSystem.AppDataDirectory, "demo");

        public async Task EnsureFilesAsync()
        {
            try
            {
                List<string> files = await ReadManifestAsync();

                foreach (string file in files)
                {
                    string targetPath = ResolvePath(DemoPrefix + file);
                    if (File.Exists(targetPath))
                    {
                        continue;
                    }

                    string? directory = Path.GetDirectoryName(targetPath);
                    if (directory != null)
                    {
                        Directory.CreateDirectory(directory);
                    }

                    string temporaryPath = targetPath + ".part";
                    using (Stream source = await FileSystem.OpenAppPackageFileAsync("Demo/" + file))
                    using (FileStream target = File.Create(temporaryPath))
                    {
                        await source.CopyToAsync(target);
                    }

                    File.Move(temporaryPath, targetPath, true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Demo files could not be copied: {ex.Message}");
            }
        }

        public static string ResolvePath(string path)
        {
            if (!path.StartsWith(DemoPrefix, StringComparison.Ordinal))
            {
                return path;
            }

            string relativePath = path.Substring(DemoPrefix.Length).Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(DemoFolder, relativePath);
        }

        public static string? ResolveOptionalPath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            return ResolvePath(path);
        }

        public static bool IsDemoFile(string filePath)
        {
            return filePath.StartsWith(DemoFolder, StringComparison.OrdinalIgnoreCase);
        }

        #region Support Methods
        private static async Task<List<string>> ReadManifestAsync()
        {
            var files = new List<string>();

            using Stream stream = await FileSystem.OpenAppPackageFileAsync(ManifestFile);
            using var reader = new StreamReader(stream);

            string? line = await reader.ReadLineAsync();
            while (line != null)
            {
                string trimmed = line.Trim();
                if (trimmed.Length > 0)
                {
                    files.Add(trimmed);
                }

                line = await reader.ReadLineAsync();
            }

            return files;
        }
        #endregion
    }
}
