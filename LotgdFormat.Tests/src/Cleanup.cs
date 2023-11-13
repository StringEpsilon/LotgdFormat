namespace LotgdFormat.Tests;

using LotgdFormat;
using Xunit;

public class Cleanup {
	[Fact]
	public void Keeps_Data() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'c', Tag = "center"}
		});
		formatter.AddText("`cThis is centered");
		Assert.Equal("<center>This is centered", formatter.GetOutput());
		Assert.Equal("<center>This is centered", formatter.GetOutput());
	}

	[Fact]
	public void Clears_Content() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'c', Tag = "center"}
		});
		formatter.AddText("`cThis is centered");
		Assert.Equal("<center>This is centered", formatter.GetOutput());
		formatter.ClearText();
		Assert.Equal("", formatter.GetOutput());
		formatter.CloseOpenTags();
		Assert.Equal("</center>", formatter.GetOutput());
	}

	[Fact]
	public void Clears_All() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'c', Tag = "center"}
		});
		formatter.AddText("`cThis is centered");
		Assert.Equal("<center>This is centered", formatter.GetOutput());
		formatter.Clear();
		Assert.Equal("", formatter.GetOutput());
		formatter.CloseOpenTags();
		Assert.Equal("", formatter.GetOutput());
	}


	[Fact]
	public void DoubleCloseOpenTags() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'c', Tag = "center"}
		});
		formatter.AddText("`cThis is centered");
		var foo = formatter.GetOutput();
		Assert.Equal("<center>This is centered", foo);
		Assert.False(formatter.IsClear());
		formatter.CloseOpenTags();
		Assert.Equal(foo + "</center>", formatter.GetOutput());
		formatter.CloseOpenTags();
		Assert.Equal(foo + "</center>", formatter.GetOutput());
		formatter.Clear();
		Assert.Equal("", formatter.GetOutput());
		formatter.Clear();
		Assert.False(formatter.HasContent());
		formatter.Clear();
		Assert.True(formatter.IsClear());
	}

	[Fact]
	public void ColorCloseMultipleTexts() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'c', Tag = "center"},
			new LotgdFormatCode(){ Token = '$', Color = "FF0000"},
			new LotgdFormatCode(){ Token = '@', Color = "00FF00"},
		});
		formatter.AddText("`$");
		var foo = formatter.GetOutput();
		formatter.ClearText();
		formatter.AddText("FooBar`0");
		foo += formatter.GetOutput();
		Assert.Equal("<span class=\"c36\">FooBar</span>", foo);
	}
}
