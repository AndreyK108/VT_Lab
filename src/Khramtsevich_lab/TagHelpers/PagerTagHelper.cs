using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Khramtsevich_lab.TagHelpers
{
    [HtmlTargetElement("pager")]
    public class PagerTagHelper : TagHelper
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PagerTagHelper(
            LinkGenerator linkGenerator,
            IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        [HtmlAttributeName("page-current")]
        public int PageCurrent { get; set; }

        [HtmlAttributeName("page-total")]
        public int PageTotal { get; set; }

        [HtmlAttributeName("action")]
        public string Action { get; set; } = "Index";

        [HtmlAttributeName("controller")]
        public string Controller { get; set; } = "Product";

        [HtmlAttributeName("category")]
        public string? Category { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (PageTotal <= 1)
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "nav";
            output.Attributes.SetAttribute("aria-label", "Page navigation");

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination justify-content-center");

            // Prev
            ul.InnerHtml.AppendHtml(CreateItem("«", PageCurrent - 1, PageCurrent == 1));

            // Pages
            for (int i = 1; i <= PageTotal; i++)
            {
                ul.InnerHtml.AppendHtml(CreateItem(
                    i.ToString(),
                    i,
                    false,
                    i == PageCurrent));
            }

            // Next
            ul.InnerHtml.AppendHtml(CreateItem("»", PageCurrent + 1, PageCurrent == PageTotal));

            output.Content.SetHtmlContent(ul);
        }

        private TagBuilder CreateItem(
            string text,
            int page,
            bool disabled,
            bool active = false)
        {
            var li = new TagBuilder("li");
            li.AddCssClass("page-item");

            if (disabled) li.AddCssClass("disabled");
            if (active) li.AddCssClass("active");

            var a = new TagBuilder("a");
            a.AddCssClass("page-link");

            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
                return li;

            var url = _linkGenerator.GetPathByAction(
                httpContext,
                Action,
                Controller,
                new { pageNo = page, category = Category });

            a.Attributes["href"] = url ?? "#";
            a.InnerHtml.AppendHtml(new HtmlString(text));

            li.InnerHtml.AppendHtml(a);
            return li;
        }
    }
}
