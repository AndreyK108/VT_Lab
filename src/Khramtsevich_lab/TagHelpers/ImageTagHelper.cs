using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Khramtsevich_lab.TagHelpers
{
    [HtmlTargetElement("img", Attributes = "img-action, img-controller")]
    public class ImageTagHelper : TagHelper
    {
        private readonly LinkGenerator _linkGenerator;

        public ImageTagHelper(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        [HtmlAttributeName("img-controller")]
        public string ImgController { get; set; } = string.Empty;

        [HtmlAttributeName("img-action")]
        public string ImgAction { get; set; } = string.Empty;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var url = _linkGenerator.GetPathByAction(ImgAction, ImgController);

            // Если src уже есть — можно не трогать, но по заданию мы формируем src сами.
            output.Attributes.SetAttribute("src", url ?? "/");

            // Эти атрибуты в итоговой разметке не нужны
            output.Attributes.RemoveAll("img-controller");
            output.Attributes.RemoveAll("img-action");
        }
    }
}
