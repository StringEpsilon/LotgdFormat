namespace LotgdFormat;

/// <summary>
/// Token produced by the enumerator that walks over the input span.
/// </summary>
internal ref struct FormatToken {
	internal FormatToken(int index, int length) {
		this.Index = index;
		this.Length = length;
	}

	internal int Index;
	internal int Length;

	internal char Identifier;
}

