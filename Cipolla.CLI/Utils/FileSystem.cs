using System.IO;

namespace Cipolla.CLI.Utils
{
    public static class FileSystem
    {
        public static void EnsureDirectoryExists(string path)
        {
            Directory.CreateDirectory(path);
        }
    }
}
