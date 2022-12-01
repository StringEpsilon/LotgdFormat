using Microsoft.AspNetCore.Html;

namespace LotgdFormat;
#nullable enable

public class SelfClosingNode : INode {
	public string? Tag { get; set; }
	public SelfClosingNode(string? tag) {
		this.Tag = tag;
	}
	public IHtmlContent Render() {
		if (this.Tag == null) {
			return HtmlString.Empty;
		}
		return new HtmlString($"<{this.Tag}/>");
	}
}
