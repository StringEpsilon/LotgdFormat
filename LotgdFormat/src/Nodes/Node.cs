
using System.Web;

namespace LotgdFormat;

public readonly struct Node {
	public readonly NodeType Type;
	public readonly string Output;
	public readonly char Token;

	public Node(NodeType type) {
		this.Type = type;
		this.Output = string.Empty;
	}

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

	public static Node CreateTextNode(ReadOnlySpan<char> Text, bool IsUnsafe) {
		string output = IsUnsafe
			? Text.ToString()
			: HttpUtility.HtmlEncode(Text.ToString());

		return new Node(NodeType.Text, output);
	}

	public static Node CreateSelfClosingNode(in string tag) {
		return new Node(NodeType.SelfClosing, $"<{tag}/>");
	}

	public static Node CreateTagNode(char token, in string tag, string? styles = null) {
		string output;
		if (styles == null) {
			output = $"<{tag}>";
		} else {
			output = $"<{tag} {styles}>";
		}
		return new Node(NodeType.Tag, output, token);
	}

	public static Node CreateTagCloseNode(in string tag) {
		return new Node(NodeType.TagClose, $"</{tag}>");
	}

}
