namespace ONVIFStream
{
    public static class JSONReader
    {
        /// <summary>
        /// Read JSON-string from file with name == fileName.
        /// </summary>
        /// <param name="fileName">Filename like *.json.</param>
        /// <returns>JSON-string</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static string ReadJsonFile(string fileName)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            string relativeFilePath = Path.Combine(basePath, "Config", fileName);

            if (!File.Exists(relativeFilePath))
            {
                throw new FileNotFoundException($"Файл не найден: {relativeFilePath}");
            }

            string json = File.ReadAllText(relativeFilePath);

            return json;
        }
    }
}
