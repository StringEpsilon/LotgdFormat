namespace LotgdFormat.Tests;

using LotgdFormat;
using Microsoft.AspNetCore.Html;
using Xunit;

public class Color {
	[Fact]
	public void Renders_Color() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color = "00FF00"}
		});
		IHtmlContent result = formatter.AddText("This is White `@this is green.").GetOutput();

		Assert.Equal("This is White <span class=\"c64\">this is green.", result.GetString());
	}

	[Fact]
	public void Renders_Plaintext_On_Redudant_CloseTag() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color = "00FF00"}
		});
		IHtmlContent result = formatter.AddText("`0This is plaintext.").GetOutput();

		Assert.Equal("This is plaintext.", result.GetString());
	}

	[Fact]
	public void Renders_Plaintext_ColorDisabled() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color = "00FF00"},
			new LotgdFormatCode(){ Token = '$', Color = "FF0000"}
		});
		formatter.Color = false;
		IHtmlContent result = formatter.AddText("`$red `@green `$red `@green").GetOutput();

		Assert.Equal("red green red green", result.GetString());
	}

	[Fact]
	public void Continues_Color() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color = "00FF00"}
		});
		IHtmlContent result = formatter.AddText("This is White `@this is green.").GetOutput();

		Assert.Equal("This is White <span class=\"c64\">this is green.", result.GetString());

		result = formatter.AddText(" This is still green.").GetOutput();
		Assert.Equal("This is White <span class=\"c64\">this is green. This is still green.", result.GetString());
	}


	[Fact]
	public void Closes_Color() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color = "00FF00"}
		});
		IHtmlContent result = formatter.AddText("This is White `@this is green.").GetOutput();

		Assert.Equal("This is White <span class=\"c64\">this is green.", result.GetString());

		result = formatter.AddText("`0 This is no longer green.").GetOutput();
		Assert.Equal("This is White <span class=\"c64\">this is green.</span> This is no longer green.", result.GetString());
	}

	[Fact]
	public void Acjacent_Colors() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color = "00FF00"},
			new LotgdFormatCode(){ Token = '$', Color = "FF0000"}
		});
		IHtmlContent result = formatter.AddText("`@green`$red").GetOutput();

		Assert.Equal("<span class=\"c64\">green</span><span class=\"c36\">red", result.GetString());
	}
}
