namespace LotgdFormat.Tests;

using LotgdFormat;
using Xunit;

public class Color {
	[Fact]
	public void Renders_Color() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color = "00FF00"}
		});
		formatter.AddText("This is White `@this is green.");
		string result = formatter.GetOutput();

		Assert.Equal("This is White <span class=\"c64\">this is green.", result);
	}

	[Fact]
	public void Renders_Plaintext_On_Redudant_CloseTag() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color = "00FF00"}
		});
		formatter.AddText("`0This is plaintext.");
		string result = formatter.GetOutput();

		Assert.Equal("This is plaintext.", result);
	}

	[Fact]
	public void Renders_Plaintext_ColorDisabled() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color = "00FF00"},
			new LotgdFormatCode(){ Token = '$', Color = "FF0000"}
		});
		formatter.Color = false;
		formatter.AddText("`$red `@green `$red `@green");
		string result = formatter.GetOutput();

		Assert.Equal("red green red green", result);
	}

	[Fact]
	public void Continues_Color() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color = "00FF00"}
		});
		formatter.AddText("This is White `@this is green.");
		string result = formatter.GetOutput();

		Assert.Equal("This is White <span class=\"c64\">this is green.", result);

		formatter.AddText(" This is still green.");
		result = formatter.GetOutput();
		Assert.Equal("This is White <span class=\"c64\">this is green. This is still green.", result);
	}


	[Fact]
	public void Closes_Color() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color = "00FF00"}
		});
		formatter.AddText("This is White `@this is green.");
		string result = formatter.GetOutput();

		Assert.Equal("This is White <span class=\"c64\">this is green.", result);

		formatter.AddText("`0 This is no longer green.");
		result = formatter.GetOutput();
		Assert.Equal("This is White <span class=\"c64\">this is green.</span> This is no longer green.", result);
	}

	[Fact]
	public void CloseOpenTags_Closes_Color() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color = "00FF00"}
		});
		formatter.AddText("`@green");
		string result = formatter.GetOutput();

		Assert.Equal("<span class=\"c64\">green", result);
		formatter.CloseOpenTags();
		result = formatter.GetOutput();
		Assert.Equal("<span class=\"c64\">green</span>", result);
	}

	[Fact]
	public void Acjacent_Colors() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color = "00FF00"},
			new LotgdFormatCode(){ Token = '$', Color = "FF0000"}
		});
		formatter.AddText("`@green`$red");
		string result = formatter.GetOutput();

		Assert.Equal("<span class=\"c64\">green</span><span class=\"c36\">red", result);
	}

	[Fact]
	public void Immediate_Close() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color = "00FF00"},
			new LotgdFormatCode(){ Token = '$', Color = "FF0000"}
		});
		formatter.AddText("`@`0Normal text");
		string result = formatter.GetOutput();

		Assert.Equal("Normal text", result);
	}

	[Fact]
	public void Immediate_Close_OtherTags() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color = "00FF00"},
			new LotgdFormatCode(){ Token = '$', Color = "FF0000"},
			new LotgdFormatCode(){ Token = 'b', Tag = "strong"}
		});
		formatter.AddText("`b`@`0strong text");
		string result = formatter.GetOutput();

		Assert.Equal("<strong>strong text", result);
	}

	[Fact]
	public void Does_Not_Crash() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color = "00FF00"},
			new LotgdFormatCode(){ Token = '$', Color = "FF0000"},
			new LotgdFormatCode(){ Token = '&', Color = "FFFFFF"},
			new LotgdFormatCode(){ Token = 'b', Tag = "strong"}
		});
		formatter.AddText("`0Dummy");
		formatter.GetOutput();
		formatter.ClearText();
		formatter.AddText("Dummy");
		formatter.GetOutput();
		formatter.ClearText();
		formatter.AddText("`0`&Dummy`0`&");
		formatter.GetOutput();
		formatter.ClearText();
		formatter.AddText("Dummy`0");
		formatter.GetOutput();
		formatter.ClearText();
		Assert.True(true);
	}
}
