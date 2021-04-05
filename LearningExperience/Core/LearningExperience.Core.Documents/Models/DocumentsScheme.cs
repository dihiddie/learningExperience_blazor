using System.Collections.Generic;

namespace LearningExperience.Core.Documents.Models
{
    public class DocumentsScheme
    {
        public List<Document> Documents { get; set; }
    }

    // ReSharper disable once StyleCop.SA1402
    public class Document
    {
        public string Value { get; set; }

        public string Path { get; set; }

        public List<Document> Documents { get; set; }
    }
}
