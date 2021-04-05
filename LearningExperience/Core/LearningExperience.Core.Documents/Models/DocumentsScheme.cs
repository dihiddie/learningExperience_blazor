using System.Collections.Generic;

namespace LearningExperience.Core.Documents.Models
{
    public class DocumentsScheme<T>
    {
        public List<T> Documents { get; set; }
    }

    // ReSharper disable once StyleCop.SA1402
    public class Document : DocumentBase<Document>
    {
    }
}
