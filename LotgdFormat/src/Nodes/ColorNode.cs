

namespace LotgdFormat;

#nullable enable

public struct ColorNode : INode {
	public ColorNode(char token) {
		this.Token = token;
	}

	public char Token { get; set; }

	public string Render() {
		return $"<span class=\"c{(int)this.Token}\">";
	}
}
