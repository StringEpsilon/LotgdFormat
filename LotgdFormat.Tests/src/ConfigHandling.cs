namespace LotgdFormat.Tests;

using LotgdFormat;
using Microsoft.AspNetCore.Html;
using Xunit;

public class InvalidConfig {
	[Fact]
	public void InvalidCode_Renders_Text() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'E', Tag = null, Color = null}
		});
		IHtmlContent result = formatter.AddText("`EThis is defective`0").GetOutput();

		Assert.Equal("This is defective", result.GetString());
	}

	[Fact]
	public void Color_Over_Tag() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = 'E', Tag = "b", Color = "FF0000"}
		});
		IHtmlContent result = formatter.AddText("`EThis is not bold.`0").GetOutput();

		Assert.Equal("<span class=\"c69\">This is not bold.</span>", result.GetString());
	}
}
