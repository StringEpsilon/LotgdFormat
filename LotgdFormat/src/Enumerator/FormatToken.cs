// SPDX-License-Identifier: GPL-2.0-only
namespace LotgdFormat;

/// <summary>
/// Token produced by the enumerator that walks over the input span.
/// </summary>
internal ref struct FormatToken {
	internal int _index;
	internal int _length;
	internal char _identifier;
}

