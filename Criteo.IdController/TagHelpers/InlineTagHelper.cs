using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;

namespace Criteo.IdController.TagHelpers
{
    // Use the word 'inline' within the script tag to inject code inline (specified in the src attribute)
    // This class supports both (JS) scripts and (CSS) styles
    // Note that external styles are referred by using the <link> tag but this custom approach is more readable

    // Example: <script inline type="text/javascript" src="/dist/js/js-code.js"></script>
    // Example: <style inline src="/dist/css/css-code.css"></style>

    [HtmlTargetElement("script", Attributes = "inline")]
    [HtmlTargetElement("style", Attributes = "inline")]
    public class InlineTagHelper : InlineTagHelperBase
    {
        [HtmlAttributeName("src")]
        public string Src { get; set; }

        public InlineTagHelper(IHostingEnvironment hostingEnvironment, IMemoryCache cache)
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
