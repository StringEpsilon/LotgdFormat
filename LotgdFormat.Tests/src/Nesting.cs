
namespace LotgdFormat.Tests;
using LotgdFormat;
using Xunit;

public class Nesting {
	[Fact]
	public void Unnests_Properly() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color="00FF00"},
			new LotgdFormatCode(){ Token = 'b', Tag="b"}
		});
		formatter.AddText("`@green `bbold green`0 bold`b");
		string result = formatter.GetOutput();

		Assert.Equal("<span class=\"c64\">green <b>bold green</b></span><b> bold</b>", result);
	}

	[Fact]
	public void Unstacks_Nested_Tags_Correctly() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color="00FF00"},
			new LotgdFormatCode(){ Token = '&', Color="FFFFFF"},
			new LotgdFormatCode(){ Token = 'b', Tag="b"},
			new LotgdFormatCode(){ Token = 'n', Tag="br", SelfClosing = true}
		});
		formatter.AddText("`b`&with text`0`b `bbold`b");
		string result = formatter.GetOutput();

		Assert.Equal("<b><span class=\"c38\">with text</span></b> <b>bold</b>", result);
		formatter.Clear();
		formatter.AddText("`b`&`0`b `bbold`b");
		result = formatter.GetOutput();

		Assert.Equal(" <b>bold</b>", result);
	}
}
