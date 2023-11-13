
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
}
