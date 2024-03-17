namespace LotgdFormat.Tests;

using LotgdFormat;
using Xunit;

public class Tag {
	[Fact]
	public void Renders_OpenTag() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode('c', tag: "center")
		});

		string result = formatter.AddText("`cThis is centered");

		Assert.Equal("<center>This is centered", result);
	}

	[Fact]
	public void Renders_CloseTag() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode('c', tag: "center")
		});

		string result = formatter.AddText("`cThis is centered`cThis isn't");

		Assert.Equal("<center>This is centered</center>This isn&#39;t", result);
	}

	[Fact]
	public void Renders_Style() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode('h', tag: "span", style: "style=\"font-size: larger\"")
		});

		string result = formatter.AddText("`hBig Text`h");

		Assert.Equal("<span style=\"font-size: larger\">Big Text</span>", result);
	}

	[Fact]
	public void Renders_Class() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode('h', tag: "span", style: "class=\"headline\"")
		});

		string result = formatter.AddText("`hBig Text`h");;

		Assert.Equal("<span class=\"headline\">Big Text</span>", result);
	}

	[Fact]
	public void Renders_CloseOpenTags() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode('c', tag: "center")
		});
		string result = formatter.AddText("`cThis is centered") + formatter.CloseOpenTags();
		Assert.Equal("<center>This is centered</center>", result);
	}
}
