namespace LotgdFormat;

/// <summary>
/// Token produced by the enumerator that walks over the input span.
/// </summary>
internal readonly ref struct FormatToken {
	internal FormatToken(ReadOnlySpan<char> text, char token) {
		Text = text;
		Token = token;
	}

	internal FormatToken(ReadOnlySpan<char> text) {
		Text = text;
	}

	internal readonly ReadOnlySpan<char> Text;
	internal readonly char Token;
}

