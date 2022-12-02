namespace LotgdFormat;

#nullable enable
internal enum CodeType {
	Invalid = -1,
	Color = 0,
	SelfClosing,
	Formatting,
	Terminating,
}

public partial class LotgdFormatCode {
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
	/// <value></value>
	public string? Tag { get; set; }

	/// <summary>
	/// Whether or not the associated tag is self-closing.
	/// </summary>
	public bool SelfClosing { get; set; } = false;

	internal INode? GetNode() {
		if (this.Color != null) {
			return new ColorNode(this.Token);
		};
		if (this.Tag == null) {
			return null;
		}
		return this.SelfClosing
			? new SelfClosingNode(this.Tag)
			: new TagNode(this.Token, this.Tag, this.Style);
	}
}

