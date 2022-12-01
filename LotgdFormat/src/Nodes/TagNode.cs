using System.Text;
using Microsoft.AspNetCore.Html;

namespace LotgdFormat;

#nullable enable

public class TagNode : INode {
	public char Token { get; set; }
	public string Tag { get; set; }
	public string? Styles { get; set; }

	public TagNode(char token, string tag, string? styles = null) {
		this.Tag = tag;
		this.Token = token;
		this.Styles = styles;
	}

	public IHtmlContent Render() {
		var rawHthml = new StringBuilder();
		rawHthml.Append('<').Append(this.Tag);
		if (this.Styles != null) {
			rawHthml.Append(" style=\"").Append(this.Styles).Append('"');
		}
		rawHthml.Append('>');

		return new HtmlContentBuilder().AppendHtml(rawHthml.ToString());
	}
}

