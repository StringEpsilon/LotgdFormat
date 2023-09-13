namespace LotgdFormat;

/// <summary>
/// Token produced by the enumerator that walks over the input span.
/// </summary>
internal ref struct FormatToken {
	internal FormatToken(ReadOnlySpan<char> text, char token) {
		Text = text;
		Token = token;
	}

	internal FormatToken(ReadOnlySpan<char> text) {
		Text = text;
	}

	internal ReadOnlySpan<char> Text;
	internal char Token;
}

