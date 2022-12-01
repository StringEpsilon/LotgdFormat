namespace LotgdFormat.Tests;
using LotgdFormat;
using Microsoft.AspNetCore.Html;
using Xunit;

public class Plaintext {
	[Fact]
	public void Echoes_Plaintext() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color="00FF00"}
		});
		IHtmlContent result = formatter.AddText("This is plaintext").GetOutput();

		Assert.Equal("This is plaintext", result.GetString());
	}

 	[Fact]
	public void Escapes_Backtick() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color="00FF00"}
		});
		IHtmlContent result = formatter.AddText("``").GetOutput();

		Assert.Equal("`", result.GetString());
	}
}
