using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Web.Admin.TagHelperCustoms
{
    [HtmlTargetElement("atv-group")]
    public class AtvGroupTagHelper : TagHelper
    {
        public string Title { get; set; }
        public string Id { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "atv-group");
            if (!string.IsNullOrEmpty(Id))
            {
                output.Attributes.SetAttribute("id", Id);
            }

            // Lấy nội dung con
            var childContent = output.GetChildContentAsync().Result.GetContent();

            output.Content.SetHtmlContent($@"
                <div class='atv-group-title'>{Title}</div>
                {childContent}
            ");
        }
    }
}
