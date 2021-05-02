using Microsoft.AspNetCore.Mvc;

namespace LearningExperience.WebApi.ContentLoader.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text.Json;
    using System.Threading;

    using LearningExperience.Core.Documents.Models;
    using LearningExperience.Core.Extensions;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.FileProviders;

    [ApiController]
    [Route("api/[controller]")]
    public class ContentLoaderController : ControllerBase
    {
        private readonly string baseJsonFileName;

        private readonly string folderPath;

        private readonly PhysicalFileProvider fileProvider;

        public ContentLoaderController(IConfiguration configuration)
        {
            folderPath = configuration["DocumentsSchemeFolder"] ?? throw new NullReferenceException("Configuration key - DocumentsSchemeFolder doesn't exist");
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
            var documentScheme = JsonSerializer.Deserialize<DocumentsScheme<Document>>(baseJson);
            return SetHasContent(documentScheme);
        }

        [HttpGet("search")]
        public DocumentsScheme<Document> Filter(string text)
        {
            if (string.IsNullOrEmpty(text)) return GetScheme();

            Thread.Sleep(1000);

            var filteredScheme = new DocumentsScheme<Document>() { Documents = new List<Document>() };
            var originalScheme = GetScheme();

            foreach (var docLevel1 in originalScheme.Documents)
            {
                var foundedAny = false;

                var firstLevel = new Document
                                     {
                                         Value = docLevel1.Value, Path = docLevel1.Path, Documents = new List<Document>()
                                     };

                if (docLevel1.Value.CaseInsensitiveContains(text))
                {
                    firstLevel.Value = docLevel1.Value.WrapWordsInTag(new List<string> { text }, "em");
                    firstLevel.Documents = docLevel1.Documents;
                    HighLightLowerDocuments(firstLevel, text);
                    filteredScheme.Documents.Add(firstLevel);
                    continue;
                }

                foreach (var docLevel2 in docLevel1.Documents)
                {
                    if (docLevel2.Value.CaseInsensitiveContains(text))
                    {
                        docLevel2.Value = docLevel2.Value.WrapWordsInTag(new List<string> { text }, "em");
                        firstLevel.Documents.Add(docLevel2);

                        if (!filteredScheme.Documents.Select(x => x.Value).Contains(firstLevel.Value))
                        {
                            HighLightLowerDocuments(firstLevel, text);
                            filteredScheme.Documents.Add(firstLevel);
                        }

                        continue;
                    }

                    foreach (var docLevel3 in docLevel2.Documents)
                    {
                        if (docLevel3.Value.CaseInsensitiveContains(text))
                            docLevel3.Value = docLevel3.Value.WrapWordsInTag(new List<string> { text }, "em");

                        var foundedList = new List<Document>();

                        foreach (var docLevel4 in docLevel3.Documents)
                            if (docLevel4.Value.CaseInsensitiveContains(text))
                            {
                                docLevel4.Value = docLevel4.Value.WrapWordsInTag(new List<string> { text }, "em");
                                foundedList.Add(docLevel4);
                            }

                        if (!foundedList.Any()) continue;
                        foundedAny = true;

                        var secondLevel = new Document { Value = docLevel2.Value, Path = docLevel2.Path, Documents = new List<Document>() };

                        var thirdLevel = new Document { Value = docLevel3.Value, Path = docLevel3.Path, Documents = new List<Document>() };
                        if (!secondLevel.Documents.Select(x => x.Value).Contains(thirdLevel.Value))
                            secondLevel.Documents.Add(thirdLevel);
                        thirdLevel.Documents.AddRange(foundedList);

                        if (!firstLevel.Documents.Select(x => x.Value).Contains(secondLevel.Value))
                            firstLevel.Documents.Add(secondLevel);
                        else
                        {
                            var second = firstLevel.Documents.FirstOrDefault(x => x.Value == secondLevel.Value);
                            second?.Documents.Add(thirdLevel);
                        }
                    }
                }

                if (foundedAny)
                    if (!filteredScheme.Documents.Select(x => x.Value).Contains(firstLevel.Value))
                        filteredScheme.Documents.Add(firstLevel);
            }

            return filteredScheme;
        }

        [HttpPost]
        public void SaveContent(string path, string content)
        {
            var filePath = Path.Combine(folderPath, path);
            System.IO.File.WriteAllText(filePath, content);
        }

        private DocumentsScheme<Document> SetHasContent(DocumentsScheme<Document> documentsScheme)
        {
            foreach (var docLevel1 in documentsScheme.Documents)
                foreach (var docLevel2 in docLevel1.Documents)
                    foreach (var docLevel3 in docLevel2.Documents)
                        foreach (var docLevel4 in docLevel3.Documents)
                            if (new FileInfo(fileProvider.GetFileInfo(docLevel4.Path).PhysicalPath).Length != 0)
                                docLevel4.HasContent = true;

            return documentsScheme;
        }

        private void HighLightLowerDocuments(Document document, string text)
        {
            if (document.Documents == null) return;
            foreach (var doc in document.Documents)
            {
                if (doc.Value.CaseInsensitiveContains(text))
                    doc.Value = doc.Value.WrapWordsInTag(new List<string> { text }, "em");
                HighLightLowerDocuments(doc, text);
            }
        }
    }
}