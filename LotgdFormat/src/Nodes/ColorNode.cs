using System.Text;
using Microsoft.AspNetCore.Html;

namespace LotgdFormat;

#nullable enable

public class ColorNode : INode {
	public ColorNode(char token, string? styles = null) {
		this.Token = token;
	}

	public char Token { get; set; }
	public string? Styles { get; set; }

	public IHtmlContent Render() {
		var rawHthml = new StringBuilder();
		rawHthml.Append("<span class=\"c")
			.Append((int)this.Token)
			.Append("\">");

		return new HtmlContentBuilder().AppendHtml(rawHthml.ToString());
	}
}
