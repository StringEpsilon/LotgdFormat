namespace LotgdFormat.Tests;

using LotgdFormat;
using Microsoft.AspNetCore.Html;
using Xunit;

public class Tag {
	[Fact]
	public void Renders_OpenTag() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'c', Tag = "center"}
		});
		IHtmlContent result = formatter.AddText("`cThis is centered").GetOutput();

		Assert.Equal("<center>This is centered", result.GetString());
	}

	[Fact]
	public void Renders_CloseTag() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'c', Tag = "center"}
		});
		IHtmlContent result = formatter.AddText("`cThis is centered`cThis isn't").GetOutput();

		Assert.Equal("<center>This is centered</center>This isn&#39;t", result.GetString());
	}

	[Fact]
	public void Renders_CloseOpenTags() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'c', Tag = "center"}
		});
		IHtmlContent result = formatter.AddText("`cThis is centered").GetOutput();
		Assert.Equal("<center>This is centered", result.GetString());
		Assert.Equal("</center>", formatter.CloseOpenTags().GetString());
	}
}
