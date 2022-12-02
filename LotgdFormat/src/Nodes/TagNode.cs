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

	public string Render() {
		if (this.Styles == null) {
			return $"<{this.Tag}>";
		} else {
			return $"<{this.Tag} style=\"{this.Styles}\">";
		}
	}
}

