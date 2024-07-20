// SPDX-License-Identifier: GPL-2.0-only
namespace LotgdFormat;

using System.Buffers;
using System.Text.Encodings.Web;


internal static class NodeExtension {
	private static int _maxEncodeLength = HtmlEncoder.Default.MaxOutputCharactersPerInputCharacter;

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
						return text.ToString();
					}
					Span<char> charBuffer = stackalloc char[_maxEncodeLength];
					HtmlEncoder.Default.Encode(text, charBuffer, out _, out int charLength, true);
					return charBuffer.Slice(0, charLength).ToString();
				}
			}
		}
		if (text.IsSafe()) {
			return text.ToString();
		}
		var rentedArray = ArrayPool<char>.Shared.Rent(
			// renting 4k should be fine. But for longer inputs we should rent accurately.
			text.Length < 512
				? 512 * _maxEncodeLength
				: text.Length + text.CountUnsafe() *  _maxEncodeLength
		);
		Span<char> buffer = new Span<char>(rentedArray);
		HtmlEncoder.Default.Encode(text, buffer, out _, out int bytesWritten, true);
		var result = buffer.Slice(0, bytesWritten).ToString();
		ArrayPool<char>.Shared.Return(rentedArray);
		return result;
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
