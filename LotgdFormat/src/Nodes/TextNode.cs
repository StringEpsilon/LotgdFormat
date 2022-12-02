using System.Web;

namespace LotgdFormat;

public struct TextNode : INode {
	public string Text { get; set; }
	public bool IsUnsafe { get; set; }
	public string Render() {
		return this.IsUnsafe
			? this.Text
			: HttpUtility.HtmlEncode(this.Text);
	}
}
