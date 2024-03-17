namespace LotgdFormat;

public class LotgdFormatCode {
	public LotgdFormatCode(
		char token,
		bool selfClosing = false,
		bool privileged = false,
		string? color = null,
		string? style = null,
		string? tag = null
	) {
		this.Token = token;
		this.Color = color;
		this.Style = style;
		this.Tag = tag;
		this.SelfClosing = selfClosing;
		this.Privileged = privileged;

		if (this.Color != null) {
			this._nodeType = NodeType.Color;
			this._nodeOutput = $"<span class=\"c{(int)this.Token}\">";
		} else if (this.Tag == null) {
			this._nodeType = NodeType.Invalid;
			this._nodeOutput = "";
		} else if (this.SelfClosing) {
			this._nodeType = NodeType.SelfClosing;
			this._nodeOutput = $"<{this.Tag}/>";
		} else {
			this._nodeType = NodeType.Tag;
			this._nodeOutputClose = $"</{this.Tag}>";
			this._nodeOutput = this.Style == null
				? $"<{this.Tag}>"
				: $"<{this.Tag} {this.Style}>";
		}
	}

	/// <summary>
	/// The single character token to that marks this formatting token.
	/// Must be unique.
	/// </summary>
	public char Token { get; set; }

	/// <summary>
	/// The six digit hexcode of the desired text color.
	/// </summary>
	public string? Color { get; set; }

	/// <summary>
	/// Additional CSS rules applied to the rendered HTML element per inline style attribute.
	/// </summary>
	public string? Style { get; set; }

	/// <summary>
	/// Specifies which tag
	/// </summary>
	public string? Tag { get; set; }

	/// <summary>
	/// Whether or not the associated tag is self-closing.
	/// </summary>
	public bool SelfClosing { get; set; } = false;

	/// <summary>
	/// Code may only apply to text added with the isPriviliged flag set. Otherwise the token is ignored.
	/// </summary>
	public bool Privileged { get; set; } = false;

	internal string _nodeOutput = "";
	internal string _nodeOutputClose = "";
	internal NodeType _nodeType = NodeType.Invalid;
}

