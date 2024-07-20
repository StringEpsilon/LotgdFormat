// SPDX-License-Identifier: GPL-2.0-only
namespace LotgdFormat;

internal enum NodeType : byte {
	Text,
	Color,
	ColorClose,
	SelfClosing,
	Tag,
	TagClose,
}
