using Microsoft.AspNetCore.Html;

namespace LotgdFormat;
#nullable enable

public class SelfClosingNode : INode {
	public string Tag { get; set; }
	public SelfClosingNode(string tag) {
		this.Tag = tag;
	}
	public IHtmlContent Render() {
		return new HtmlString($"<{this.Tag}/>");
	}
}
