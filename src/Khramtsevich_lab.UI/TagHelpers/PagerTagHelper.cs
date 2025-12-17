using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace Khramtsevich_lab.TagHelpers
{
    [HtmlTargetElement("pager")]
    public class PagerTagHelper : TagHelper
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PagerTagHelper(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        // Старые имена атрибутов (чтобы не сломать прошлые лабы)
        [HtmlAttributeName("page-current")]
        public int PageCurrent { get; set; }

        [HtmlAttributeName("page-total")]
        public int PageTotal { get; set; }

        // Новые имена атрибутов (как в методичке)
        [HtmlAttributeName("current-page")]
        public int CurrentPage { get; set; }

        [HtmlAttributeName("total-pages")]
        public int TotalPages { get; set; }

        [HtmlAttributeName("action")]
        public string Action { get; set; } = "Index";

        [HtmlAttributeName("controller")]
        public string Controller { get; set; } = "Product";

        [HtmlAttributeName("category")]
        public string? Category { get; set; }

        // ВАЖНО: для админских Razor Pages
        [HtmlAttributeName("admin")]
        public bool Admin { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Поддержка двух вариантов имен
            var cur = CurrentPage > 0 ? CurrentPage : PageCurrent;
            var total = TotalPages > 0 ? TotalPages : PageTotal;

            if (total <= 1)
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "div";
            output.AddClass("row", HtmlEncoder.Default);

            var nav = new TagBuilder("nav");
            nav.Attributes.Add("aria-label", "pagination");

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination justify-content-center");

            int prev = cur == 1 ? 1 : cur - 1;
            int next = cur == total ? total : cur + 1;

            ul.InnerHtml.AppendHtml(CreateItem(cur, total, Category, prev, "«"));
            for (int i = 1; i <= total; i++)
                ul.InnerHtml.AppendHtml(CreateItem(cur, total, Category, i, i.ToString(), isNumber: true));
            ul.InnerHtml.AppendHtml(CreateItem(cur, total, Category, next, "»"));

            nav.InnerHtml.AppendHtml(ul);
            output.Content.AppendHtml(nav);
        }

        private TagBuilder CreateItem(int cur, int total, string? category, int pageNo, string text, bool isNumber = false)
        {
            var li = new TagBuilder("li");
            li.AddCssClass("page-item");

            if (isNumber && pageNo == cur)
                li.AddCssClass("active");

            if (!isNumber)
            {
                if (text == "«" && cur == 1) li.AddCssClass("disabled");
                if (text == "»" && cur == total) li.AddCssClass("disabled");
            }

            var a = new TagBuilder("a");
            a.AddCssClass("page-link");

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                a.Attributes["href"] = "#";
            }
            else
            {
                var routeData = new { pageNo = pageNo, category = category };
                string? url;

                // Admin = Razor Pages, не MVC
                if (Admin)
                    url = _linkGenerator.GetPathByPage(httpContext, page: "./Index", values: routeData);
                else
                    url = _linkGenerator.GetPathByAction(httpContext, Action, Controller, routeData);

                a.Attributes["href"] = url ?? "#";
            }

            a.InnerHtml.AppendHtml(new HtmlString(text));
            li.InnerHtml.AppendHtml(a);
            return li;
        }
    }
}
