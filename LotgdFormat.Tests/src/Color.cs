// SPDX-License-Identifier: GPL-2.0-only
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
		var config = new FormatterConfig(new List<LotgdFormatCode> {
			new LotgdFormatCode('@', color: "00FF00"),
			new LotgdFormatCode('$', color: "FF0000"),
			new LotgdFormatCode('b', tag: "strong")
		});
		var formatter = new Formatter(config);

		string result = formatter.AddText(input);
		Assert.Equal(expected, result);
	}

	[Fact]
	public void FormatsLargeInput() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode('^', color: "FFFF00"),
			new LotgdFormatCode('@', color: "00FF00"),
			new LotgdFormatCode('$', color: "FF0000"),
			new LotgdFormatCode('b', tag: "strong"),
			new LotgdFormatCode('n', tag: "br", selfClosing: true)
		});

		string result = formatter.AddText(
			"`^`bYour first day`b`n`@"
			+ "Your first day in the world can be very confusing! You're presented with a lot of information, and you don't "
			+ "need almost any of it! It's true! One thing you should probably keep an eye on though, are your hit points. "
			+ "This is found under \"Vital Info.\" No matter what profession you choose, in the end, you are some kind of "
			+ "warrior or fighter, and so you need to learn how to do battle. The best way to do this is to look for "
			+ "creatures to kill in the forest. When you find one, check it out, and make sure that it's not a higher "
			+ "level than you, because if it is, you might not live through the fight. Keep in mind that you can always "
			+ "try to run away from something that you encountered, but some times it might take several tries before you "
			+ "get away. You might want to buy armor and weapons in the village square in order to give yourself a better "
			+ "chance against these creatures out in the forest.`n"
		);
		Assert.Equal(
			"<span class=\"c94\"><strong>Your first day</strong><br/></span><span class=\"c64\">"
			+ "Your first day in the world can be very confusing! You&#x27;re presented with a lot of information, and you "
			+ "don&#x27;t need almost any of it! It&#x27;s true! One thing you should probably keep an eye on though, are your hit "
			+ "points. This is found under &quot;Vital Info.&quot; No matter what profession you choose, in the end, you are some "
			+ "kind of warrior or fighter, and so you need to learn how to do battle. The best way to do this is to look "
			+ "for creatures to kill in the forest. When you find one, check it out, and make sure that it&#x27;s not a higher "
			+ "level than you, because if it is, you might not live through the fight. Keep in mind that you can always "
			+ "try to run away from something that you encountered, but some times it might take several tries before you "
			+ "get away. You might want to buy armor and weapons in the village square in order to give yourself a better "
			+ "chance against these creatures out in the forest.<br/>",
			result

		);

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
		Assert.False(formatter.Color);
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
	[Fact]
	public void ClosesColorAccrossFragments() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode('@', color: "00FF00")
		});

		string result = formatter.AddText("`@green") + formatter.AddText("`0");
		Assert.Equal("<span class=\"c64\">green</span>", result);
	}

}
