
using System.Web;

public enum NodeType {
	Invalid,
	Text,
	Color,
	ColorClose,
	SelfClosing,
	Tag,
	TagClose,
}


public struct Node {
	public readonly NodeType Type;
	public readonly string Output;
	public readonly char Token;

	public Node(NodeType type, string output) {
		this.Type = type;
		this.Output = output;
	}
	public Node(NodeType type, string output, char token) {
		this.Type = type;
		this.Output = output;
		this.Token = token;
	}

	public static Node CreateColorNode(char token) {
		return new Node(NodeType.Color, $"<span class=\"c{(int)token}\">");
	}
	public static Node CreateColorCloseNode() {
		return new Node(NodeType.ColorClose, "</span>");
	}
	public static Node CreateTextNode(string Text, bool IsUnsafe) {
		string output = IsUnsafe
			? Text
			: HttpUtility.HtmlEncode(Text);

		return new Node(NodeType.Text, output);
	}
	public static Node CreateSelfClosingNode(string tag) {
		return new Node(NodeType.SelfClosing, $"<{tag}/>");
	}
	public static Node CreateTagNode(char token, string tag, string? styles = null) {
		string output;
		if (styles == null) {
			output = $"<{tag}>";
		} else {
			output = $"<{tag} style=\"{styles}\">";
		}
		return new Node(NodeType.Tag, output, token);
	}

	public static Node CreateTagCloseNode(string tag) {
		return new Node(NodeType.TagClose, $"</{tag}>");
	}

}
