using Microsoft.AspNetCore.Html;

namespace LotgdFormat;

public class ColorCloseNode : INode {
	public IHtmlContent Render() {
		return new HtmlString("</span>");
	}
}
