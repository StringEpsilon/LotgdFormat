namespace LotgdFormat.Tests;

using LotgdFormat;
using Xunit;

public class Color {
	[Theory]
	[InlineData("not green`@", "not green<span class=\"c64\">")]
	[InlineData("`@", "<span class=\"c64\">")]
	[InlineData("`@green", "<span class=\"c64\">green")]
	[InlineData("`@green`0", "<span class=\"c64\">green</span>")]
	[InlineData("`@`0text", "text")]
	[InlineData("`@`@green", "<span class=\"c64\">green")]
	[InlineData("`@`@`@`@`@`@`0green", "green")]
	[InlineData("`@green`$red", "<span class=\"c64\">green</span><span class=\"c36\">red")]
	public void Renders_Colors(string input, string expected) {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode('@', color: "00FF00"),
			new LotgdFormatCode('$', color: "FF0000"),
			new LotgdFormatCode('b', tag: "strong")
		});

		string result = formatter.AddText(input);
		Assert.Equal(expected, result);
	}

	[Fact]
	public void Renders_Plaintext_ColorDisabled() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode('@', color: "00FF00"),
			new LotgdFormatCode('$', color: "FF0000")
		}) {
			Color = false
		};
		string result = formatter.AddText("`$red `@green `$red `@green");

		Assert.Equal("red green red green", result);
	}

	[Fact]
	public void Continues_Color() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode('@', color: "00FF00")
		});

		string result = formatter.AddText("This is White `@this is green.");

		Assert.Equal("This is White <span class=\"c64\">this is green.", result);


		result += formatter.AddText(" This is still green.");
		Assert.Equal("This is White <span class=\"c64\">this is green. This is still green.", result);
	}

	[Fact]
	public void CloseOpenTags_Closes_Color() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode('@', color: "00FF00")
		});

		string result = formatter.AddText("`@green");
		Assert.Equal("<span class=\"c64\">green", result);
		result += formatter.CloseOpenTags();
		Assert.Equal("<span class=\"c64\">green</span>", result);
	}

}
