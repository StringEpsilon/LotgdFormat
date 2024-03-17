
using System.Web;

namespace LotgdFormat;

public readonly struct Node {
	public readonly NodeType Type;
	public readonly int TextStart;
	public readonly bool IsUnsafe = false;
	public readonly int Size = 0;
	public readonly char Token;
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
		} else if (this.Type == NodeType.Text && input.Length > 0) {
			return this.IsUnsafe
				? input.Slice(this.TextStart, this.Size).ToString()
				: HttpUtility.HtmlEncode(input.Slice(this.TextStart, this.Size).ToString());
		} else if (this.Type == NodeType.ColorClose) {
			return "</span>";
		}
		return "";
	}
}
