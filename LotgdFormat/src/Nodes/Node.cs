// SPDX-License-Identifier: GPL-2.0-only
using System.Runtime.InteropServices;

namespace LotgdFormat;

internal readonly struct Node {
	internal readonly char _token;
	internal readonly NodeType _type;
	internal readonly bool _isUnsafe = false;
	internal readonly int _textStart;
	internal readonly int _size = 0;

	internal Node(NodeType type) {
		this._type = type;
	}

	/// <summary>
	/// Creates a new Text node.
	/// </summary>
	/// <param name="textStart">
	/// Start of the text content in the input string.
	/// </param>
	/// <param name="textLength">
	/// Length of the text cotent
	/// </param>
	/// <param name="IsUnsafe">
	/// Whether the text is unsafe and needs to be HTML-Encoded when rendering.
	/// </param>
	internal Node(int textStart, int textLength, bool IsUnsafe) {
		this._type = NodeType.Text;
		this._textStart = textStart;
		this._size = textLength;
		this._isUnsafe = IsUnsafe;
	}

	internal Node(NodeType type, LotgdFormatCode code) {
		this._type = type;
		this._token = code.Token;
		this._size = 0;
	}

	internal Node(LotgdFormatCode code) {
		this._type = code._nodeType;
		this._token = code.Token;
		this._size = 0;
	}
}
