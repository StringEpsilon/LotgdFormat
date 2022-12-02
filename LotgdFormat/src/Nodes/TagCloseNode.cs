using Microsoft.AspNetCore.Html;

namespace LotgdFormat;

#nullable enable

public class TagCloseNode : INode {
	public string Tag { get; set; }
	public TagCloseNode(string tag) {
		this.Tag = tag;
	}
	public IHtmlContent Render() {
		return new HtmlString($"</{this.Tag}>");
	}
}
