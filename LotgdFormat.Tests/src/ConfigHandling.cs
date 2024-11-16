// SPDX-License-Identifier: GPL-2.0-only
namespace LotgdFormat.Tests;

using Xunit;

public class InvalidConfig {
	[Fact]
	public void InvalidCode_Throws() {
		var exception = Assert.Throws<ArgumentException>(() => {
			new LotgdFormatCode('E', tag: null, color: null);
		});
		Assert.Equal("When no color is configured, a tag name must be provided (Parameter 'tag')", exception.Message);
		Assert.Equal("tag", exception.ParamName);

		exception = Assert.Throws<ArgumentException>(() => {
			new LotgdFormatCode('0', tag: "span");
		});
		Assert.Equal(
			"The token '0' is reserved. Overriding them in the configuration is not supported (Parameter 'token')",
			exception.Message
		);
		Assert.Equal("token", exception.ParamName);
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
