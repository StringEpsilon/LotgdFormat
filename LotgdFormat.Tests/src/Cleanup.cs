namespace LotgdFormat.Tests;

using LotgdFormat;
using Xunit;

public class Cleanup {
	[Fact]
	public void Keeps_Data() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'c', Tag = "center"}
		});
		Assert.Equal("<center>This is centered", formatter.AddText("`cThis is centered").GetOutput());
		Assert.Equal("<center>This is centered", formatter.GetOutput());
	}

	[Fact]
	public void Clears_Content() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'c', Tag = "center"}
		});
		Assert.Equal("<center>This is centered", formatter.AddText("`cThis is centered").GetOutput());
		Assert.Equal("", formatter.ClearText().GetOutput());
		Assert.Equal("</center>", formatter.CloseOpenTags().GetOutput());
	}

	[Fact]
	public void Clears_All() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'c', Tag = "center"}
		});
		Assert.Equal("<center>This is centered", formatter.AddText("`cThis is centered").GetOutput());
		Assert.Equal("", formatter.Clear().GetOutput());
		Assert.Equal("", formatter.CloseOpenTags().GetOutput());
	}


	[Fact]
	public void DoubleCloseOpenTags() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'c', Tag = "center"}
		});
		var foo = formatter.AddText("`cThis is centered").GetOutput();
		Assert.Equal("<center>This is centered", foo);
		Assert.False(formatter.IsClear());
		Assert.Equal(foo + "</center>", formatter.CloseOpenTags().GetOutput());
		Assert.Equal(foo + "</center>", formatter.CloseOpenTags().GetOutput());
		Assert.Equal("", formatter.Clear().GetOutput());
		Assert.False(formatter.Clear().HasContent());
		Assert.True(formatter.Clear().IsClear());
	}

	[Fact]
	public void ColorCloseMultipleTexts() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'c', Tag = "center"},
			new LotgdFormatCode(){ Token = '$', Color = "FF0000"},
			new LotgdFormatCode(){ Token = '@', Color = "00FF00"},
		});
		var foo = formatter.AddText("`$").GetOutput();
		formatter.ClearText();
		foo += formatter.AddText("FooBar`0").GetOutput();
		Assert.Equal("<span class=\"c36\">FooBar</span>", foo);
	}
}
