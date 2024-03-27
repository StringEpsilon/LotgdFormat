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

	[Theory]
	[InlineData("`HReserved`H", true, "<span class=\"navhi\">Reserved</span>")]
	[InlineData("`HReserved`H", false, "Reserved")]
	[InlineData("`H`bR`beserved`H", false, "<b>R</b>eserved")]
	public void Respects_Priviliged(string input, bool isPrivileged, string expected) {
		var formatter = new Formatter([
			new LotgdFormatCode('H', tag: "span", style: "class=\"navhi\"", privileged: true),
			new LotgdFormatCode('b', tag: "b", privileged: false)
		]);

		string result = formatter.AddText(input, isPrivileged: isPrivileged);
		Assert.Equal(expected, result);
	}
}
