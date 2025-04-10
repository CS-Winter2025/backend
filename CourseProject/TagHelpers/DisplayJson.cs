using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace CourseProject.TagHelpers
{
    [HtmlTargetElement("details-json")]
    public class DisplayJson : TagHelper
    {
        public Dictionary<string, string>? Details { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "details-json");

            if (Details == null || Details.Count == 0)
            {
                output.Content.SetHtmlContent("<span class=\"text-muted\">None</span>");
                return;
            }

            var builder = new StringBuilder();
            builder.AppendLine("<ul class=\"list-unstyled m-0\">");
            foreach (var pair in Details)
            {
                builder.AppendLine($"<li><strong>{pair.Key}</strong>: {pair.Value ?? "N/A"}</li>");
            }
            builder.AppendLine("</ul>");
            output.Content.SetHtmlContent(builder.ToString());
        }
    }
}