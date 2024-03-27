namespace LotgdFormat;

/// <summary>
/// Token produced by the enumerator that walks over the input span.
/// </summary>
internal ref struct FormatToken {
	internal int Index;
	internal int Length;

	internal char Identifier;
}

