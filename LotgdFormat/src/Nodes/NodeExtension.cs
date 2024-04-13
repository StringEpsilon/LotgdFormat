// SPDX-License-Identifier: GPL-2.0-only
namespace LotgdFormat;

using System.Web;

internal static class NodeExtension {
	internal static string GetOuput(this in Node node, in ReadOnlySpan<char> input) {
		if (node._type != NodeType.Text) {
			// Only non-text node without a LotgdFormatCode required is NodeType.ColorClose:
			return "</span>";
		}
		ReadOnlySpan<char> text = input.Slice(node._textStart, node._size);
		if (node._isUnsafe) {
			return text.ToString();
		}
		if (node._size == 1) {
			char character = text[0];
			switch (character) {
				case ' ': {
					return " ";
				}
				case '\n': {
					return "";
				}
				case '"': {
					return "&quot;";
				}
				case '&': {
					return "&amp;";
				}
				default: {
					if (character.IsSafe()) {
						return character.ToString();
					}
					return HttpUtility.HtmlEncode(character.ToString());
				}
			}
		}
		if (text.IsSafe()) {
			return text.ToString();
		}
		return HttpUtility.HtmlEncode(text.ToString());
	}

	internal static string GetOuput(this in Node node, LotgdFormatCode code) {
		return node._type switch {
			NodeType.Color => code._nodeOutput,
			NodeType.Tag => code._nodeOutput,
			NodeType.SelfClosing => code._nodeOutput,
			NodeType.TagClose => code._nodeOutputClose,
			_ => ""
		};
	}
}
