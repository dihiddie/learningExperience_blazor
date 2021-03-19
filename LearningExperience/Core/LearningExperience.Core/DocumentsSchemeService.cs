namespace LearningExperience.BAL
{
    using System.IO;
    using System.Text.Json;
    using LearningExperience.BAL.Models;

    public class DocumentsSchemeService
    {
        private readonly string filePath;

        public DocumentsSchemeService()
        {
            filePath = @"D:\work\personal\learningExperience\src\base.json";

            if (!File.Exists(filePath)) throw new FileNotFoundException("Topic scheme file not found");
        }

        public DocumentsScheme GetScheme() => JsonSerializer.Deserialize<DocumentsScheme>(filePath);
    }
}
