namespace LotgdFormat.Tests;
using LotgdFormat;
using Xunit;

public class SelfClosing {
	[Fact]
	public void Renders_Linebreak() {
		var formatter = new Formatter([
			new LotgdFormatCode('n', tag: "br", selfClosing: true)
		]);

		string result = formatter.AddText("Line one.`nLine two.");;

		Assert.Equal("Line one.<br/>Line two.", result);
	}

	[Fact]
	public void Renders_HorizontalLine() {
		var formatter = new Formatter([
			new LotgdFormatCode('-', tag: "hr", selfClosing: true)
		]);

		string result = formatter.AddText("`-");;

		Assert.Equal("<hr/>", result);
	}
}
