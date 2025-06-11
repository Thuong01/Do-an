using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace Web.Admin.TagHelperCustoms
{
    [HtmlTargetElement("label", Attributes = ForAttributeName)]
    public class RequiredMarkTagHelper : LabelTagHelper
    {
        private const string ForAttributeName = "asp-for";

        public RequiredMarkTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression AspFor { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (AspFor == null)
            {
                throw new InvalidOperationException("The 'asp-for' attribute must be specified.");
            }

            var modelExplorer = AspFor.ModelExplorer;
            var metadata = modelExplorer?.Metadata;

            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            if (metadata != null && metadata.IsRequired)
            {
                var content = output.Content.GetContent();
                output.Content.AppendHtml("<span class=\"text-danger mx-2\">*</span>");
            }

            //await base.ProcessAsync(context, output);
        }
    }
}
