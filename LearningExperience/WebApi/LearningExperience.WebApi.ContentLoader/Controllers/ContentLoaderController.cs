using Microsoft.AspNetCore.Mvc;

namespace LearningExperience.WebApi.ContentLoader.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text.Json;

    using LearningExperience.Core.Documents.Models;
    using LearningExperience.Core.Extensions;

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
            var documentScheme = JsonSerializer.Deserialize<DocumentsScheme<Document>>(baseJson);
            return SetHasContent(documentScheme);
        }

        [HttpGet("search")]
        public DocumentsScheme<Document> Filter(string text)
        {
            var filteredScheme = new DocumentsScheme<Document>();
            var originalScheme = GetScheme();

            foreach (var docLevel1 in originalScheme.Documents)
            {
                Document firstLevel;
                Document secondLevel;
                Document thirdLevel;
                var foundedAny = false;

                if (docLevel1.Value.Contains(text))
                {
                    docLevel1.Value = docLevel1.Value.WrapWordsInTag(new List<string> { text }, "em");
                    foundedAny = true;
                }

                firstLevel = new Document { Value = docLevel1.Value, Path = docLevel1.Path };

                foreach (var docLevel2 in docLevel1.Documents)
                {
                    if (docLevel2.Value.CaseInsensitiveContains(text))
                    {
                        docLevel2.Value = docLevel2.Value.WrapWordsInTag(new List<string> { text }, "em");
                        foundedAny = true;
                    }

                    secondLevel = new Document { Value = docLevel2.Value, Path = docLevel2.Path };
                    if (firstLevel.Documents != null) firstLevel.Documents.Add(secondLevel);
                    else firstLevel.Documents = new List<Document> { secondLevel };

                    foreach (var docLevel3 in docLevel2.Documents)
                    {
                        if (docLevel3.Value.CaseInsensitiveContains(text))
                        {
                            docLevel3.Value = docLevel3.Value.WrapWordsInTag(new List<string> { text }, "em");
                            foundedAny = true;
                        }

                        thirdLevel = new Document { Value = docLevel3.Value, Path = docLevel3.Path };
                        if (secondLevel.Documents != null) secondLevel.Documents.Add(thirdLevel);
                        else secondLevel.Documents = new List<Document> { thirdLevel };

                        var foundedList = new List<Document>();

                        foreach (var docLevel4 in docLevel3.Documents)
                        {
                            if (docLevel4.Value.CaseInsensitiveContains(text))
                            {
                                docLevel4.Value = docLevel4.Value.WrapWordsInTag(new List<string> { text }, "em");
                                foundedList.Add(docLevel4);
                            }
                        }

                        if (foundedList.Any())
                        {
                            foundedAny = true;
                            if (thirdLevel.Documents != null) thirdLevel.Documents.AddRange(foundedList);
                            else
                            {
                                thirdLevel.Documents = new List<Document>();
                                thirdLevel.Documents.AddRange(foundedList);
                            }

                        }
                    }
                }

                if (foundedAny)
                {
                    if (filteredScheme.Documents != null) filteredScheme.Documents.Add(firstLevel);
                    else filteredScheme.Documents = new List<Document> { firstLevel };
                }
            }

            return filteredScheme;
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
    }
}