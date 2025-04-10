using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CourseProject.Helpers
{
    [HtmlTargetElement("profile-picture")]
    public class ProfilePicture : TagHelper
    {
        public byte[] ImageData { get; set; }
        public int MaxWidth { get; set; } = 100;
        public int MaxHeight { get; set; } = 100;
        public string Alt { get; set; } = "Profile Photo";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ImageData != null && ImageData.Length > 0)
            {
                var base64 = Convert.ToBase64String(ImageData);
                var src = $"data:image/jpeg;base64,{base64}";

                output.TagName = "img";
                output.Attributes.SetAttribute("src", src);
                output.Attributes.SetAttribute("alt", Alt);
                output.Attributes.SetAttribute("style", $"max-width: {MaxWidth}px; max-height: {MaxHeight}px;");
            }
            else
            {
                output.TagName = "span";
                output.Content.SetContent("No photo");
            }
        }
    }
}
