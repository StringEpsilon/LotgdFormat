using System.Text;
using Microsoft.AspNetCore.Html;

namespace LotgdFormat;

#nullable enable

public class ColorNode : INode {
	public ColorNode(char token) {
		this.Token = token;
	}

	public char Token { get; set; }

	public IHtmlContent Render() {
		return new HtmlString($"<span class=\"c{(int)this.Token}\">");
	}
}
