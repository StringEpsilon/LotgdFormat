namespace LotgdFormat.Tests;
using LotgdFormat;
using Xunit;

public class SelfClosing {
	[Fact]
	public void Renders_Linebreak() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'n', Tag = "br", SelfClosing = true}
		});
		string result = formatter.AddText("Line one.`nLine two.").GetOutput();

		Assert.Equal("Line one.<br/>Line two.", result);
	}

	[Fact]
	public void Renders_HorizontalLine() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '-', Tag = "hr", SelfClosing = true}
		});
		string result = formatter.AddText("`-").GetOutput();

		Assert.Equal("<hr/>", result);
	}
}
