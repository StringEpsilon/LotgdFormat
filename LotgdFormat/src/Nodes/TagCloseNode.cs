namespace LotgdFormat;

#nullable enable

public struct TagCloseNode : INode {
	public string Tag { get; set; }
	public TagCloseNode(string tag) {
		this.Tag = tag;
	}
	public string Render() {
		return $"</{this.Tag}>";
	}
}
