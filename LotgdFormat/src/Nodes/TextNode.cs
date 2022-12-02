using System.Web;
using Microsoft.AspNetCore.Html;
namespace LotgdFormat;

public class TextNode : INode {
	public string Text { get; set; } = "";
	public bool IsUnsafe { get; set; } = false;
	public IHtmlContent Render() {
		return this.IsUnsafe
			? new HtmlString(this.Text)
			: new HtmlString(HttpUtility.HtmlEncode(this.Text));
	}
}
