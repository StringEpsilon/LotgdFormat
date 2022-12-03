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
	private static readonly Node _invalidNode = new(NodeType.Invalid);

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

	internal Node GetNode() {
		if (this.Color != null) {
			return Node.CreateColorNode(this.Token);
		};
		if (this.Tag == null) {
			return _invalidNode;
		}
		return this.SelfClosing
			? Node.CreateSelfClosingNode(this.Tag)
			: Node.CreateTagNode(this.Token, this.Tag, this.Style);
	}
}

