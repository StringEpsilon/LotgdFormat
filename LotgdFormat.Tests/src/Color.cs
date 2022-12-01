namespace LotgdFormat.Tests;

using System.Collections.Specialized;
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
