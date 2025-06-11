using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Web.Admin.TagHelperCustoms
{
    [HtmlTargetElement("a", Attributes = "active")]
    public class ActiveLinkTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Kiểm tra xem có thuộc tính yellow-background không
            if (context.AllAttributes.ContainsName("yellow-background"))
            {
                // Thêm thuộc tính style vào thẻ a
                output.Attributes.Add("style", "background-color: yellow;");
            }
        }
    }
}
