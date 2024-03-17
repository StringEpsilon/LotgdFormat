using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LotgdFormat;

#nullable enable

public class Formatter {
	private readonly HashArray<LotgdFormatCode> _codeLookup;
	private char? _currentColor;
	private readonly List<Node> _nodes = new();
	private readonly Dictionary<char, bool> _openTags = new();
	private int _lastColor = -1;
	public bool Color { get; set; } = true;

	#region Private methods
	private void SetTagOpenStatus(char token, bool open) {
		if (!this._openTags.ContainsKey(token)) {
			this._openTags.Add(token, open);
		} else {
			this._openTags[token] = open;
		}
	}

	private void CloseColor() {
		if (this._currentColor != null && this._lastColor < 0 ) {
			this._nodes.Add(new Node(NodeType.ColorClose));
			return;
		}
		if (this._currentColor == null || this._lastColor < 0 || this._nodes.Count == 0) {
			return;
		}
		var index = this._nodes.Count - 1;
		if (index == this._lastColor && this._nodes[_lastColor].Type == NodeType.Color) {
			this._nodes.RemoveAt(this._lastColor);
			this._lastColor = -1;
			this._currentColor = null;
			return;
		}
		if (index - this._lastColor < 0) {
			this._nodes.Add(new Node(NodeType.ColorClose));
			this._lastColor = -1;
			this._currentColor = null;
			return;
		}

		Span<Node> stack = new Node[index - this._lastColor];
		var i = stack.Length - 1;
		for (; index > this._lastColor; index--) {
			var node = this._nodes[index];
			if (node.Type == NodeType.Tag) {
				var code = _codeLookup[node.Token];
				if (this.IsTagOpen(node.Token) && code?.Tag != null) {
					stack[i] = node;
					i--;
					this._nodes.Add(new Node(NodeType.TagClose, code));
					this._openTags[node.Token] = false;
				}
			}
		}
		this._nodes.Add(new Node(NodeType.ColorClose));
		this._lastColor = -1;
		this._currentColor = null;
		for (i = 0; i < stack.Length; i++) {
			this.AddNode(stack[i]);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void AddNode(in Node node) {
		switch (node.Type) {
			case NodeType.Invalid: {
				return;
			}
			case NodeType.Tag: {
				var code = _codeLookup[node.Token];
				if (code?.Tag != null && this.IsTagOpen(node.Token)) {
					if (this._nodes.Count > 0 && this._nodes.Last().Token == node.Token) {
						this._nodes.RemoveAt(this._nodes.Count-1);
					} else {
						this._nodes.Add(new Node(NodeType.TagClose, code));
					}
					this._openTags[node.Token] = false;
				} else {
					this._nodes.Add(node);
					this.SetTagOpenStatus(node.Token, true);
				}
				break;
			}
			case NodeType.Color: {
				if (this.Color) {
					if (this._currentColor == node.Token) {
						break;
					}
					if (_lastColor >= 0) {
						this._nodes.Add(new Node(NodeType.ColorClose));
						this._currentColor = null;
					}
					this._nodes.Add(node);
					this._lastColor = this._nodes.Count - 1;
					this._currentColor = node.Token;
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

	private void AddTextNode(int textStart, int length, bool isUnsafe) {
		this._nodes.Add(new Node(textStart, length, isUnsafe));
	}

	private bool IsTagOpen(char token) {
		this._openTags.TryGetValue(token, out bool result);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private string CreateOutput(List<Node> nodes, in ReadOnlySpan<char> input) {
		string[] outputs = new string[this._nodes.Count];

		var totalLength = 0;
		for (int i = 0; i < nodes.Count; i++) {
			outputs[i] = _nodes[i].GetOuput(input);
			totalLength += outputs[i].Length;
		}
		Span<char> ouputSpan = stackalloc char[totalLength];
		int index = 0;
		int nodeLength;
		for (int i = 0; i < nodes.Count; i++) {
			nodeLength = outputs[i].Length;
			outputs[i].CopyTo(ouputSpan.Slice(index, nodeLength));
			index += nodeLength;
		}
		this._nodes.Clear();
		this._lastColor = -1;
		return ouputSpan.ToString();
	}

	#endregion

	public Formatter(List<LotgdFormatCode> config) {
		var codeArray = CollectionsMarshal.AsSpan(config);
		Span<char> keys = stackalloc char[config.Count];
		for (int i = 0; i < codeArray.Length; i++) {
			keys[i] = codeArray[i].Token;
		}
		this._codeLookup = new HashArray<LotgdFormatCode>(keys, codeArray);
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
	public string AddText(ReadOnlySpan<char> input, bool isUnsafe = false, bool isPrivileged = false) {
		if (input == null || input.Length == 0) {
			return "";
		}
		var enumerator = new TokenEnumerator(input);
		foreach (var token in enumerator) {
			switch (token.Identifier) {
				case '\0':
					break;
				case '0':
					this.CloseColor();
					break;
				default:
					var code = _codeLookup[token.Identifier];
					if (code != null) {
						if (!code.Privileged || isPrivileged) {
							this.AddNode(new Node(code._nodeType, code));
						}
					} else {
						this.AddTextNode(token.Index-1, 1, isUnsafe);
					}
					break;
			}
			if (token.Length != 0) {
				this.AddTextNode(token.Index, token.Length, isUnsafe);
			}
		}
		return this.CreateOutput(this._nodes, input);
	}

	/// <summary>
	/// Close the currently open tags.
	/// </summary>
	public string CloseOpenTags() {
		if ((_lastColor != -1 && this._nodes.Count == 0) || this._currentColor != null) {
			this.AddNode(new Node(NodeType.ColorClose));
			this._lastColor = -1;
			this._currentColor = null;
		} else {
			this.CloseColor();
		}

		foreach (var token in this._openTags.Keys) {
			if (this._openTags[token]) {
				var code = this._codeLookup[token];
				if (code?.Tag != null) {
					this.AddNode(new Node(NodeType.TagClose, code));
				}
				this._openTags[token] = false;
			}
		}
		return this.CreateOutput(this._nodes, "");
	}
}
