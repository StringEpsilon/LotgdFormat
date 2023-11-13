namespace LotgdFormat.Tests;
using LotgdFormat;
using Xunit;

public class Plaintext {
	[Fact]
	public void Echoes_Plaintext() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color="00FF00"}
		});
		formatter.AddText("This is plaintext");
		string result = formatter.GetOutput();

		Assert.Equal("This is plaintext", result);
	}

	[Fact]
	public void Escapes_Backtick() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color="00FF00"}
		});
		formatter.AddText("``");
		string result = formatter.GetOutput();

		Assert.Equal("`", result);
	}

	[Fact]
	public void Renders_Unsafe() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color="00FF00"}
		});
		formatter.AddText("<script>alter('XSS');</script>", true);
		string result = formatter.GetOutput();

		Assert.Equal("<script>alter('XSS');</script>", result);
	}

	[Fact]
	public void Renders_Safe() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color="00FF00"}
		});
		formatter.AddText("<script>alter('XSS');</script>");
		string result = formatter.GetOutput();

		Assert.Equal("&lt;script&gt;alter(&#39;XSS&#39;);&lt;/script&gt;", result);
	}

	[Fact]
	public void Renders_SafeUnsafeSafe() {
		var formatter = new Formatter(new List<LotgdFormatCode> {
			new LotgdFormatCode(){ Token = '@', Color="00FF00"}
		});
		formatter.AddText("<safe/>");
		formatter.AddText("<unsafe/>", true);
		formatter.AddText("<safe/>", false);
		string result = formatter.GetOutput();

		Assert.Equal("&lt;safe/&gt;<unsafe/>&lt;safe/&gt;", result);
	}
}
