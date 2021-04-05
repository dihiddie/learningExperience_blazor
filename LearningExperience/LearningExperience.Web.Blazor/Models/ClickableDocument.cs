namespace LearningExperience.Web.Blazor.Models
{
    using System.Collections.Generic;

    using LearningExperience.Core.Documents.Models;

    public class ClickableDocument : DocumentBase
    {
        public bool IsClicked { get; set; }

        public string DisplayState => IsClicked ? "show" : "none";

        public string ArrowState => IsClicked ? "down" : "right";

        public List<ClickableDocument> Documents { get; set; }
    }
}
