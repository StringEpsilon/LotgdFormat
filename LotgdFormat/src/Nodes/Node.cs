using System.Collections.ObjectModel;
using System.Web;

namespace LotgdFormat;

internal readonly struct Node {
	internal readonly NodeType Type;
	internal readonly int TextStart;
	internal readonly bool IsUnsafe = false;
	internal readonly int Size = 0;
	internal readonly char Token;

	public Node(NodeType type) {
		this.Type = type;
		if (type == NodeType.ColorClose) {
			this.Size = 7;
		}
	}

	/// <summary>
	/// Creates a new Text node.
	/// </summary>
	/// <param name="textStart">
	/// Start of the text content in the input string.
	/// </param>
	/// <param name="textLength">
	/// Length of the text cotent
	/// </param>
	/// <param name="IsUnsafe">
	/// Whether the text is unsafe and needs to be HTML-Encoded when rendering.
	/// </param>
	public Node(int textStart, int textLength, bool IsUnsafe) {
		this.Type = NodeType.Text;
		this.TextStart = textStart;
		this.Size = textLength;
		this.IsUnsafe = IsUnsafe;
	}

	public Node(NodeType type, LotgdFormatCode code) {
		this.Type = type;
		this.Token = code.Token;
		this.Size = 0;
	}

	public Node(LotgdFormatCode code) {
		this.Type = code._nodeType;
		this.Token = code.Token;
		this.Size = 0;
	}
}

internal static class NodeExtension {
	internal static string GetOuput(this ref Node node, ReadOnlySpan<char> input) {
		if (node.Type == NodeType.Text) {
			ReadOnlySpan<char> text = input.Slice(node.TextStart, node.Size);
			if (node.IsUnsafe) {
				return text.ToString();
			}
			if (node.Size == 1) {
				switch (text[0]) {
					case ' ': {
						return " ";
					}
					case '\n': {
						return "";
					}
					case '"': {
						return "&quot;";
					}
					case '&': {
						return "&amp;";
					}
					case > '0' and < '9':
					case > 'a' and < 'z':
					case > 'A' and < 'Z': {
						return text[0].ToString();
					}
				}
			}
			return HttpUtility.HtmlEncode(text.ToString());
		}
		// Only non-text node without a LotgdFormatCode required is NodeType.ColorClose:
		return "</span>";
	}

	internal static string GetOuput(this ref Node node, LotgdFormatCode code) {
		return node.Type switch {
			NodeType.Color => code._nodeOutput,
			NodeType.Tag => code._nodeOutput,
			NodeType.SelfClosing => code._nodeOutput,
			NodeType.TagClose => code._nodeOutputClose,
			_ => ""
		};
	}
}
