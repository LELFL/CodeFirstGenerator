using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CodeFirstGenerator;
public static class RazorExtensions
{
    public static IHtmlContent Raw(this IHtmlHelper htmlHelper, string content)
    {
        return new HtmlString(content);
    }
}
