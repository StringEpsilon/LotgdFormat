namespace LotgdFormat;

/// <summary>
/// Token produced by the enumerator that walks over the input span.
/// </summary>
internal ref struct FormatToken {
	internal FormatToken(ReadOnlySpan<char> text, char token) {
		Text = text;
		Identifier = token;
	}

	internal FormatToken(ReadOnlySpan<char> text) {
		Text = text;
	}

	internal ReadOnlySpan<char> Text;
	internal char Identifier;
}

