namespace LearningExperience.Web.Blazor.Models
{
    using LearningExperience.Core.Documents.Models;

    public class ClickableDocument : DocumentBase<ClickableDocument>
    {
        public bool IsClicked { get; set; }

        public bool IsSelected { get; set; }

        public string SelectedId => IsSelected ? "selected" : string.Empty;

        public string FilledState => HasContent ? "filled" : "unfilled";

        public string DisplayState => IsClicked ? "show" : "none";

        public string ArrowState => IsClicked ? "down" : "right";
    }
}
