namespace LotgdFormat;

#nullable enable

public struct SelfClosingNode : INode {
	public string Tag { get; set; }
	public SelfClosingNode(string tag) {
		this.Tag = tag;
	}
	public string Render() {
		return $"<{this.Tag}/>";
	}
}
