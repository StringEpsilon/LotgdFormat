namespace LotgdFormat.Tests;

using LotgdFormat;
using Xunit;

public class Cleanup {

	[Fact]
	public void Clears_Content() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'c', Tag = "center"}
		});
		Assert.Equal("<center>This is centered", formatter.AddText("`cThis is centered"));
		Assert.Equal("", formatter.AddText(""));
		Assert.Equal("</center>", formatter.CloseOpenTags());
	}

	[Fact]
	public void DoubleCloseOpenTags() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'c', Tag = "center"}
		});
		var foo = formatter.AddText("`cThis is centered");
		Assert.Equal("<center>This is centered", foo);
		Assert.False(formatter.IsClear());
		Assert.Equal("</center>", formatter.CloseOpenTags());
		Assert.Equal("", formatter.CloseOpenTags());
		formatter.Clear();
		Assert.Equal("", formatter.CloseOpenTags());
		Assert.True(formatter.IsClear());
	}

	[Fact]
	public void ColorCloseMultipleTexts() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'c', Tag = "center"},
			new LotgdFormatCode(){ Token = '$', Color = "FF0000"},
			new LotgdFormatCode(){ Token = '@', Color = "00FF00"},
		});
		;
		var foo = formatter.AddText("`$");
		foo += formatter.AddText("FooBar`0");
		Assert.Equal("<span class=\"c36\">FooBar</span>", foo);
	}
}
