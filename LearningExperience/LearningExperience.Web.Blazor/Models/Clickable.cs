namespace LearningExperience.Web.Blazor.Models
{
    public class Clickable
    {
        public bool IsClicked { get; set; }

        public string DisplayState => IsClicked ? "show" : "none";

        public string ArrowState => IsClicked ? "down" : "right";
    }
}
