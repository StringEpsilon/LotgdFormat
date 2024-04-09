using System.Web;

namespace LotgdFormat;

internal readonly struct Node {
	internal readonly NodeType Type;
	internal readonly int TextStart;
	internal readonly bool IsUnsafe = false;
	internal readonly int Size = 0;
	internal readonly char Token;

	internal Node(NodeType type) {
		this.Type = type;
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
	internal Node(int textStart, int textLength, bool IsUnsafe) {
		this.Type = NodeType.Text;
		this.TextStart = textStart;
		this.Size = textLength;
		this.IsUnsafe = IsUnsafe;
	}

	internal Node(NodeType type, LotgdFormatCode code) {
		this.Type = type;
		this.Token = code.Token;
		this.Size = 0;
	}

	internal Node(LotgdFormatCode code) {
		this.Type = code._nodeType;
		this.Token = code.Token;
		this.Size = 0;
	}
}

internal static class NodeExtension {
	internal static string GetOuput(this in Node node, in ReadOnlySpan<char> input) {
		if (node.Type != NodeType.Text) {
			// Only non-text node without a LotgdFormatCode required is NodeType.ColorClose:
			return "</span>";
		}
		ReadOnlySpan<char> text = input.Slice(node.TextStart, node.Size);
		if (node.IsUnsafe) {
			return text.ToString();
		}
		if (node.Size == 1) {
			char character = text[0];
			switch (character) {
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
				default: {
					if (character.IsSafe()) {
						return character.ToString();
					}
					return HttpUtility.HtmlEncode(character.ToString());
				}
			}
		}
		if (text.IsSafe()) {
			return text.ToString();
		}
		return HttpUtility.HtmlEncode(text.ToString());
	}

	internal static string GetOuput(this in Node node, LotgdFormatCode code) {
		return node.Type switch {
			NodeType.Color => code._nodeOutput,
			NodeType.Tag => code._nodeOutput,
			NodeType.SelfClosing => code._nodeOutput,
			NodeType.TagClose => code._nodeOutputClose,
			_ => ""
		};
	}
}
