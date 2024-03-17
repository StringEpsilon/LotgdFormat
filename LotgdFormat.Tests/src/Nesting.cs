
namespace LotgdFormat.Tests;
using LotgdFormat;
using Xunit;

public class Nesting {
	[Fact]
	public void Unnests_Properly() {
		var formatter = new Formatter([
			new LotgdFormatCode('@', color:"00FF00"),
			new LotgdFormatCode('b', tag:"b"),
		]);

		string result = formatter.AddText("`@green `bbold green`0 bold`b");;

		Assert.Equal("<span class=\"c64\">green <b>bold green</b></span><b> bold</b>", result);
	}

	[Fact]
	public void Unstacks_Nested_Tags_Correctly() {
		var formatter = new Formatter([
			new LotgdFormatCode('@', color: "00FF00"),
			new LotgdFormatCode('&', color: "FFFFFF"),
			new LotgdFormatCode('b', tag: "b"),
			new LotgdFormatCode('n', tag: "br", selfClosing: true),
		]);

		string result = formatter.AddText("`b`&with text`0`b `bbold`b");

		Assert.Equal("<b><span class=\"c38\">with text</span></b> <b>bold</b>", result);
		formatter.Clear();

		result = formatter.AddText("`b`&`0`b `bbold`b");

		Assert.Equal(" <b>bold</b>", result);
	}
}
