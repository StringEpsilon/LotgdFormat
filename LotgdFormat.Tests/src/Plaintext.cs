namespace LotgdFormat.Tests;
using LotgdFormat;
using Xunit;

public class Plaintext {
	private Formatter _formatter = new Formatter([]);

	[Theory]
	[InlineData("Hello World", "Hello World")]
	[InlineData("`0Hello World", "Hello World")]
	[InlineData("Hello`0 World", "Hello World")]
	[InlineData("Hello World`0", "Hello World")]
	[InlineData("Hello World`", "Hello World`")]
	public void Renders_Plaintext(string input, string expectedResult) {
		var result = _formatter.AddText(input, false, false);
		Assert.Equal(expectedResult, result);
	}

	[Theory]
	[InlineData("``", "`")]
	[InlineData("``Hello World``", "`Hello World`")]
	[InlineData("Hello ``World``", "Hello `World`")]
	[InlineData("Hello ``World``!", "Hello `World`!")]
	[InlineData("`u`w`u", "uwu")] // drop the ` on unknown tokens.
	public void Handles_Escape(string input, string expectedResult) {
		var result = _formatter.AddText(input, false, false);
		Assert.Equal(expectedResult, result);
	}

	[Theory]
	[InlineData("<script>alert('XSS');</script>", true, "<script>alert('XSS');</script>")]
	[InlineData("`0&`0`0\"`0", false, "&amp;&quot;")]
	[InlineData("<script>alert('XSS');</script>", false, "&lt;script&gt;alert(&#39;XSS&#39;);&lt;/script&gt;")]
	public void Renders_SafeAndUnsafe(string input, bool isUnsafe, string expected) {
		string result = _formatter.AddText(input, isUnsafe);
		Assert.Equal(expected, result);
	}
}
