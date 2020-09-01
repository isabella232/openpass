using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;

namespace Criteo.IdController.TagHelpers
{
    // Use the word 'inline' within the script tag to inject the JS code inline (specified in the src attribute)
    // Example: <script inline type="text/javascript" src="/dist/js/js-code.js"></script>
    [HtmlTargetElement("script", Attributes = "inline")]
    public class InlineScriptTagHelper : InlineTagHelper
    {
        [HtmlAttributeName("src")]
        public string Src { get; set; }

        public InlineScriptTagHelper(IHostingEnvironment hostingEnvironment, IMemoryCache cache)
            : base(hostingEnvironment, cache)
        { }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var fileContent = await GetFileContentAsync(Src);
            if (fileContent == null)
            {
                output.SuppressOutput();
                return;
            }

            output.Attributes.RemoveAll("inline");
            output.Attributes.RemoveAll("src");
            output.Content.AppendHtml(fileContent);
        }
    }
}
