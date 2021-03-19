namespace LearningExperience.BAL.Json
{
    using System;
    using System.IO;
    using System.Text.Json;

    using LearningExperience.Core.Interfaces;
    using LearningExperience.Core.Models;

    using Microsoft.Extensions.Configuration;

    public class DocumentsSchemeService : IDocumentsSchemeService
    {
        private readonly string filePath;

        public DocumentsSchemeService(IConfiguration configuration)
        {
            filePath = configuration["FilePath"] ?? throw new NullReferenceException("Configuration key - FilePath doesn't exist");

            if (!File.Exists(filePath)) throw new FileNotFoundException("Topic scheme file not found");
        }

        public DocumentsScheme GetScheme() => JsonSerializer.Deserialize<DocumentsScheme>(filePath);
    }
}
