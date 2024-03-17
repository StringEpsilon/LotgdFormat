using System.Web;

namespace LotgdFormat;

internal readonly struct Node {
	internal readonly NodeType Type;
	internal readonly int TextStart;
	internal readonly bool IsUnsafe = false;
	internal readonly int Size = 0;
	internal readonly char Token;
	private readonly LotgdFormatCode? _code = null;

	public Node(NodeType type) {
		this.Type = type;
		if (type == NodeType.ColorClose) {
			this.Size = 7;
		}
	}

	public Node(int textStart, int textLength, bool IsUnsafe) {
		this.Type = NodeType.Text;
		this.TextStart = textStart;
		this.Size = textLength;
		this.IsUnsafe = IsUnsafe;
	}

	public Node(NodeType type, LotgdFormatCode code) {
		this.Type = type;
		this.Token = code.Token;
		this._code = code;
		this.Size = 0;
	}

	public string GetOuput(ReadOnlySpan<char> input) {
		if (this._code != null) {
			return this.Type switch {
				NodeType.Color => this._code._nodeOutput,
				NodeType.Tag => this._code._nodeOutput,
				NodeType.SelfClosing => this._code._nodeOutput,
				NodeType.ColorClose => "</span>",
				NodeType.TagClose => this._code._nodeOutputClose,
				_ => ""
			};
		} else if (this.Type == NodeType.Text) {
			ReadOnlySpan<char> text = input.Slice(this.TextStart, this.Size);
			if (this.IsUnsafe) {
				return text.ToString();
			}
			if (this.Size == 1) {
				switch (text[0]) {
					case ' ': {
						return " ";
					}
					case '\n': {
						return "\n";
					}
					case > 'a' and < 'z':
					case > '0' and < '9':
					case > 'A' and < 'Z': {
						return text[0].ToString();
					}
				}
			}
			return HttpUtility.HtmlEncode(text.ToString());
		} else if (this.Type == NodeType.ColorClose) {
			return "</span>";
		}
		return "";
	}
}
