// SPDX-License-Identifier: GPL-2.0-only
namespace LotgdFormat;

using System.Runtime.InteropServices;
using System.Web;

public class FormatterConfig {
	internal readonly HashArray<LotgdFormatCode> _codeLookup;

	public FormatterConfig(List<LotgdFormatCode> codes) {
		var codeArray = CollectionsMarshal.AsSpan(codes);
		Span<char> keys = stackalloc char[codes.Count];
		for (int i = 0; i < codeArray.Length; i++) {
			keys[i] = codeArray[i].Token;
		}
		this._codeLookup = new HashArray<LotgdFormatCode>(keys, codeArray);
	}
}

public class Formatter {
	private HashArray<LotgdFormatCode> _codeLookup;
	private char? _currentColor;
	private readonly List<Node> _nodes = new();
	private readonly Dictionary<char, bool> _openTags = new();
	private int _lastColor = -1;
	private bool _color = true;
	public bool Color { get => _color; set => _color = value; }

	#region Public methods
	public Formatter(List<LotgdFormatCode> codes, bool color = true) {
		var codeArray = CollectionsMarshal.AsSpan(codes);
		Span<char> keys = stackalloc char[codes.Count];
		for (int i = 0; i < codeArray.Length; i++) {
			keys[i] = codeArray[i].Token;
		}
		this._color = color;
		this._codeLookup = new HashArray<LotgdFormatCode>(keys, codeArray);
	}

	public Formatter(FormatterConfig config, bool color = true) {
		this._color = color;
		this._codeLookup = config._codeLookup;
	}

	/// <summary>
	/// Add text to the formatter.
	/// </summary>
	/// <param name="input">
	/// The text to parse and add to the output.
	/// </param>
	/// <param name="isUnsafe">
	/// If set to true, the formatter will pass through HTML content without escapting it.
	/// Use with caution.
	/// </param>
	public string AddText(in string input, bool isUnsafe = false, bool isPrivileged = false) {
		if (input == null || input.Length == 0) {
			return "";
		}
		var inputSpan = input.AsSpan();
		var enumerator = new TokenEnumerator(inputSpan);
		foreach (var token in enumerator) {
			switch (token._identifier) {
				case '\0':
					break;
				case '0':
					this.CloseColor();
					break;
				default:
					var code = this._codeLookup.Get(token._identifier);
					if (code != null) {
						if (!code.Privileged || isPrivileged) {
							this.AddNode(new Node(code));
						}
					} else {
						this._nodes.Add(new Node(token._index - 1, 1, isUnsafe));
					}
					break;
			}
			if (token._length != 0) {
				if (token._length == input.Length) {
					// we got the entire span back as text => no formatting token present
					return isUnsafe || inputSpan.IsSafe()
						? input
						: HttpUtility.HtmlEncode(input);
				}
				if (inputSpan.Slice(token._index, token._length).ContainsAnyExcept("\t\r\n")) {
					this._nodes.Add(new Node(token._index, token._length, isUnsafe));
				}
			}
		}

		return this.CreateOutput(input);
	}

	/// <summary>
	/// Close the currently open tags.
	/// </summary>
	public string CloseOpenTags() {
		if (this._color) {
			if ((_lastColor != -1 && this._nodes.Count == 0) || this._currentColor != null) {
				this.AddNode(new Node(NodeType.ColorClose));
				this._lastColor = -1;
				this._currentColor = null;
			} else {
				this.CloseColor();
			}
		}

		foreach (var token in this._openTags.Keys) {
			if (this._openTags[token]) {
				var code = this._codeLookup.Get(token);
				if (code != null) {
					this.AddNode(new Node(NodeType.TagClose, code));
				}
				this._openTags[token] = false;
			}
		}
		if (this._nodes.Count == 0) {
			this._nodes.Clear();
			this._lastColor = -1;
			return "";
		}
		return this.CreateOutput("");
	}

	/// <summary>
	/// Wether or not the formatter has open tags.
	/// </summary>
	public bool IsClear() {
		return !this._openTags.Values.Any(y => y);
	}

	/// <summary>
	/// Reset open tags.
	/// </summary>
	public void Clear() {
		this._openTags.Clear();
	}
	#endregion

	#region Private methods
	private void SetTagOpenStatus(in char token, in bool open) {
		if (!this._openTags.TryAdd(token, open)) {
			this._openTags[token] = open;
		}
	}

	private void CloseColor() {
		if (this._currentColor == null) {
			return;
		}
		if (this._lastColor < 0) {
			this._nodes.Add(new Node(NodeType.ColorClose));
			this._currentColor = null;
			return;
		}
		var index = this._nodes.Count - 1;
		if (index == this._lastColor && this._nodes[_lastColor]._type == NodeType.Color) {
			this._nodes.RemoveAt(this._lastColor);
			this._lastColor = -1;
			this._currentColor = null;
			return;
		}

		Span<Node> stack = new Node[index - this._lastColor];
		var i = stack.Length - 1;
		for (; index > this._lastColor; index--) {
			var node = this._nodes[index];
			if (node._type == NodeType.Tag && this.IsTagOpen(node._token)) {
				var code = this._codeLookup.Get(node._token);
				if (code != null) {
					stack[i] = node;
					i--;
					this._nodes.Add(new Node(NodeType.TagClose, code));
					this._openTags[node._token] = false;
				}
			}
		}
		this._nodes.Add(new Node(NodeType.ColorClose));
		this._lastColor = -1;
		this._currentColor = null;
		for (; i < stack.Length; i++) {
			if (stack[i]._type != NodeType.Text) {
				this.AddNode(stack[i]);
			}
		}
	}

	private void AddNode(in Node node) {
		switch (node._type) {
			case NodeType.Tag: {
				var code = this._codeLookup.Get(node._token)!;
				if (this.IsTagOpen(node._token)) {
					if (this._nodes.Count > 0 && this._nodes.Last()._token == node._token) {
						this._nodes.RemoveAt(this._nodes.Count - 1);
					} else {
						this._nodes.Add(new Node(NodeType.TagClose, code));
					}
					this._openTags[node._token] = false;
				} else {
					this._nodes.Add(node);
					this.SetTagOpenStatus(node._token, true);
				}
				break;
			}
			case NodeType.Color: {
				if (this._color) {
					if (this._currentColor == node._token) {
						break;
					}
					if (this._currentColor != null ) {
						this._nodes.Add(new Node(NodeType.ColorClose));
						this._currentColor = null;
					}
					this._lastColor = this._nodes.Count;
					this._nodes.Add(node);
					this._currentColor = node._token;
					break;
				}
				break;
			}
			default: {
				this._nodes.Add(node);
				break;
			}
		}
	}

	private bool IsTagOpen(in char token) {
		this._openTags.TryGetValue(token, out bool result);
		return result;
	}

	private string CreateOutput(ReadOnlySpan<char> input) {
		int nodeCount = this._nodes.Count;
		string[] outputs = new string[nodeCount];

		for (int i = 0; i < nodeCount; i++) {
			var node = _nodes[i];
			var code = node._token == '\0'
				? null
				: this._codeLookup.Get(node._token);
			if (code != null) {
				outputs[i] = node.GetOuput(code);
			} else {
				outputs[i] = node.GetOuput(input);
			}
		}
		this._nodes.Clear();
		this._lastColor = -1;
		return string.Concat(outputs);
	}

	#endregion
}
