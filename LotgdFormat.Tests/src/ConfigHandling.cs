using Xunit;

namespace LotgdFormat.Tests;


public class InvalidConfig {
	[Fact]
	public void InvalidCode_Renders_Text() {
		var formatter = new Formatter([
			new LotgdFormatCode('E', tag: null, color: null)
		]);
		string result = formatter.AddText("`EThis is defective`0");

		Assert.Equal("This is defective", result);
	}

	[Fact]
	public void Color_Over_Tag() {
		var formatter = new Formatter([
			new LotgdFormatCode('E', tag: "b", color: "FF0000")
		]);
		string result = formatter.AddText("`EThis is not bold.`0");

		Assert.Equal("<span class=\"c69\">This is not bold.</span>", result);
	}

	[Fact]
	public void Respects_Priviliged() {
		var formatter = new Formatter([
			new LotgdFormatCode('H', tag: "span", style: "class=\"navhi\"", privileged: true)
		]);

		string result = formatter.AddText("This is `Hadmin reserved.`H");
		Assert.Equal("This is admin reserved.", result);
		formatter.Clear();
		string result2 = formatter.AddText("This is `Hadmin reserved.`H", isPrivileged: true);
		Assert.Equal("This is <span class=\"navhi\">admin reserved.</span>", result2);
	}

	[Fact]
	public void Unknonw_Token_Is_Text() {
		var formatter = new Formatter([
			new LotgdFormatCode('@', color: "00FF00"),
			new LotgdFormatCode('$', color: "FF0000")
		]);

		string result = formatter.AddText("regular `@green`0 `_regular");
		Assert.Equal("regular <span class=\"c64\">green</span> _regular", result);
	}
}
