namespace LotgdFormat.Tests;

using LotgdFormat;
using Xunit;

public class Tag {
	[Fact]
	public void Renders_OpenTag() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'c', Tag = "center"}
		});
		string result = formatter.AddText("`cThis is centered").GetOutput();

		Assert.Equal("<center>This is centered", result);
	}

	[Fact]
	public void Renders_CloseTag() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'c', Tag = "center"}
		});
		string result = formatter.AddText("`cThis is centered`cThis isn't").GetOutput();

		Assert.Equal("<center>This is centered</center>This isn&#39;t", result);
	}

	[Fact]
	public void Renders_Style() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'h', Tag = "span", Style="style=\"font-size: larger\""}
		});
		string result = formatter.AddText("`hBig Text`h").GetOutput();

		Assert.Equal("<span style=\"font-size: larger\">Big Text</span>", result);
	}

	[Fact]
	public void Renders_Class() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'h', Tag = "span", Style="class=\"headline\""}
		});
		string result = formatter.AddText("`hBig Text`h").GetOutput();

		Assert.Equal("<span class=\"headline\">Big Text</span>", result);
	}

	[Fact]
	public void Renders_CloseOpenTags() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'c', Tag = "center"}
		});
		string result = formatter.AddText("`cThis is centered").CloseOpenTags().GetOutput();
		Assert.Equal("<center>This is centered</center>", result);
	}
}
