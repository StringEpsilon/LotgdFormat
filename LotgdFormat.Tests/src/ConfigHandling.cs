using Xunit;

namespace LotgdFormat.Tests;


public class InvalidConfig {
	[Fact]
	public void InvalidCode_Renders_Text() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'E', Tag = null, Color = null}
		});
		string result = formatter.AddText("`EThis is defective`0").GetOutput();

		Assert.Equal("This is defective", result);
	}

	[Fact]
	public void Color_Over_Tag() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'E', Tag = "b", Color = "FF0000"}
		});
		string result = formatter.AddText("`EThis is not bold.`0").GetOutput();

		Assert.Equal("<span class=\"c69\">This is not bold.</span>", result);
	}

	[Fact]
	public void Respects_Priviliged() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'H', Tag = "span", Style = "class=\"navhi\"", Privileged = true}
		});
		string result = formatter.AddText("This is `Hadmin reserved.`H").GetOutput();

		Assert.Equal("This is admin reserved.", result);

		formatter.Clear();
		string result2 = formatter.AddText("This is `Hadmin reserved.`H", isPrivileged: true).GetOutput();
		Assert.Equal("This is <span class=\"navhi\">admin reserved.</span>", result2);
	}

	[Fact]
	public void Unknonw_Token_Is_Text() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color = "00FF00"},
			new LotgdFormatCode(){ Token = '$', Color = "FF0000"}
		});
		string result = formatter.AddText("regular `@green`0 `_regular").GetOutput();

		Assert.Equal("regular <span class=\"c64\">green</span> _regular", result);
	}
}
