using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CourseProject.TagHelpers
{
    [HtmlTargetElement("popover")]
    public class Popover : TagHelper
    {
        public string Content { get; set; } = "";
        public string? Title { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrWhiteSpace(Content))
            {
                output.SuppressOutput(); // Skip rendering if no content
                return;
            }

            output.TagName = "button";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.SetAttribute("type", "button");
            output.Attributes.SetAttribute("class", "btn btn-link p-0 text-decoration-none");
            output.Attributes.SetAttribute("data-bs-toggle", "popover");
            output.Attributes.SetAttribute("data-bs-trigger", "hover focus");
            output.Attributes.SetAttribute("data-bs-content", Content);

            if (!string.IsNullOrEmpty(Title))
            {
                output.Attributes.SetAttribute("title", Title); // Optional title
            }

            output.Content.SetHtmlContent("❔");
        }
    }
}