using Microsoft.AspNetCore.Mvc;

namespace LearningExperience.WebApi.ContentLoader.Controllers
{
    using System;
    using System.IO;

    using Microsoft.Extensions.Configuration;

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ContentLoaderController : ControllerBase
    {
        private readonly string folderPath;

        public ContentLoaderController(IConfiguration configuration) =>
            folderPath = configuration["DocumentsSchemeFolder"]
                         ?? throw new NullReferenceException("Configuration key - DocumentsSchemeFolder doesn't exist");

        [HttpGet]
        public string Get(string pathToFile)
        {
            var path = Path.Combine(folderPath, pathToFile);
            var content = System.IO.File.ReadAllText(path);
            return content;
        }
    }
}
