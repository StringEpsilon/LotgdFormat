namespace LotgdFormat;

public class LotgdFormatCode {
	private static readonly Node _invalidNode = new(NodeType.Invalid);
	public LotgdFormatCode() {
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
	/// <value></value>
	public string? Tag { get; set; }

	/// <summary>
	/// Whether or not the associated tag is self-closing.
	/// </summary>
	public bool SelfClosing { get; set; } = false;

	private Node? _node = null;

	internal Node GetNode() {
		if (this._node != null) {
			return this._node.Value;
		}
		if (this.Color != null) {
			this._node = Node.CreateColorNode(this.Token);
		} else if (this.Tag == null) {
			this._node = _invalidNode;
		} else {
			this._node = this.SelfClosing
				? Node.CreateSelfClosingNode(this.Tag)
				: Node.CreateTagNode(this.Token, this.Tag, this.Style);
		}

		return _node.Value;
	}
}

