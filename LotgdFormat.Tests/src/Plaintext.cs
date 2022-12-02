namespace LotgdFormat.Tests;
using LotgdFormat;
using Xunit;

public class Plaintext {
	[Fact]
	public void Echoes_Plaintext() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color="00FF00"}
		});
		string result = formatter.AddText("This is plaintext").GetOutput();

		Assert.Equal("This is plaintext", result);
	}

	[Fact]
	public void Escapes_Backtick() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color="00FF00"}
		});
		string result = formatter.AddText("``").GetOutput();

		Assert.Equal("`", result);
	}

	[Fact]
	public void Renders_Unsafe() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color="00FF00"}
		});
		string result = formatter.AddText("<script>alter('XSS');</script>", true).GetOutput();

		Assert.Equal("<script>alter('XSS');</script>", result);
	}

	[Fact]
	public void Renders_Safe() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color="00FF00"}
		});
		string result = formatter.AddText("<script>alter('XSS');</script>").GetOutput();

		Assert.Equal("&lt;script&gt;alter(&#39;XSS&#39;);&lt;/script&gt;", result);
	}

	[Fact]
	public void Renders_SafeUnsafeSafe() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color="00FF00"}
		});
		string result = formatter
			.AddText("<safe/>")
			.AddText("<unsafe/>", true)
			.AddText("<safe/>", false)
			.GetOutput();

		Assert.Equal("&lt;safe/&gt;<unsafe/>&lt;safe/&gt;", result);
	}
}
