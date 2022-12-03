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
}
