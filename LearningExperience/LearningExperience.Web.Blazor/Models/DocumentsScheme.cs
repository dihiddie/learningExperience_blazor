﻿namespace LearningExperience.Web.Blazor.Models
{
    using System.Collections.Generic;

    public class DocumentsScheme
    {
        public List<Document> Documents { get; set; }
    }

    // ReSharper disable once StyleCop.SA1402
    public class Document
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public string Path { get; set; }

        public bool IsClicked { get; set; }

        public string DisplayState => IsClicked ? "show" : "none";

        public int Index { get; set; }

        public List<Document> Documents { get; set; }
    }
}