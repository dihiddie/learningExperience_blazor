using Microsoft.AspNetCore.Mvc;

namespace LearningExperience.WebApi.ContentLoader.Controllers
{
    using System;
    using System.Net;
    using System.Text.Json;

    using LearningExperience.Core.Documents.Models;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.FileProviders;

    [ApiController]
    [Route("api/[controller]")]
    public class ContentLoaderController : ControllerBase
    {
        private readonly string baseJsonFileName;

        private readonly PhysicalFileProvider fileProvider;

        public ContentLoaderController(IConfiguration configuration)
        {
            var folderPath = configuration["DocumentsSchemeFolder"] ?? throw new NullReferenceException("Configuration key - DocumentsSchemeFolder doesn't exist");
            baseJsonFileName = configuration["DocumentsSchemeFile"] ?? throw new NullReferenceException("Configuration key - DocumentsSchemeFile doesn't exist");
            fileProvider = new PhysicalFileProvider(folderPath);
        }

        [HttpGet("{path}")]
        public ContentResult Get(string path)
        {
            try
            {
                var fileContent = System.IO.File.ReadAllText(fileProvider.GetFileInfo(path).PhysicalPath);
                return new ContentResult
                           {
                               ContentType = "text/html",
                               StatusCode = (int)HttpStatusCode.OK,
                               Content = fileContent
                };
            }
            catch
            {
                return new ContentResult
                           {
                               ContentType = "text/html",
                               StatusCode = (int)HttpStatusCode.InternalServerError,
                               Content = "<p>Произошла ошибка при получении содержимого файла</p>"
                };
            }
        }

        [HttpGet]
        public DocumentsScheme<Document> GetScheme()
        {
            var baseJson = System.IO.File.ReadAllText(fileProvider.GetFileInfo(baseJsonFileName).PhysicalPath);
            return JsonSerializer.Deserialize<DocumentsScheme<Document>>(baseJson);
        }
    }
}