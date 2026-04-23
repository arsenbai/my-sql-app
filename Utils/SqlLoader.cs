
namespace MySqlApp.Utils
{
    internal static class SqlLoader
    {
        private static readonly string sqlFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
        public static string Load(string fileName) 
        {
            string fullPath = Path.Combine(sqlFolderPath, fileName);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException(
                    $"SQL file was not found: {fullPath}");
            }
            return File.ReadAllText(fullPath);
        }
    }
}
