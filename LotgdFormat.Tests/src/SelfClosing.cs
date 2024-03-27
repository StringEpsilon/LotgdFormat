namespace LotgdFormat.Tests;
using LotgdFormat;
using Xunit;

public class SelfClosing {
	private Formatter _formatter = new Formatter([
		new LotgdFormatCode('n', tag: "br", selfClosing: true),
		new LotgdFormatCode('-', tag: "hr", selfClosing: true)
	]);

	[Theory]
	[InlineData("Line one.`nLine two.", "Line one.<br/>Line two.")]
	[InlineData("`nLine two.", "<br/>Line two.")]
	[InlineData("Line one.`n`n", "Line one.<br/><br/>")]
	[InlineData("`-`n`-", "<hr/><br/><hr/>")]
	public void Renders_SafeAndUnsafe(string input, string expected) {
		string result = _formatter.AddText(input);
		Assert.Equal(expected, result);
	}
}
