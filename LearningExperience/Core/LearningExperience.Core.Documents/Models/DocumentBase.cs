namespace LearningExperience.Core.Documents.Models
{
    using System.Collections.Generic;

    public class DocumentBase<T>
    {
        public string Value { get; set; }

        public string Path { get; set; }

        public bool HasContent { get; set; }

        public List<T> Documents { get; set; }
    }
}
