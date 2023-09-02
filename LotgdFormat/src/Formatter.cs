using System.Runtime.CompilerServices;
using System.Security.Permissions;

namespace LotgdFormat;

#nullable disable

public class Formatter {
	private readonly LotgdFormatCode[] _codeLookup;
	private readonly List<Node> _nodes = new();
	private readonly Dictionary<ushort, bool> _openTags = new();
	private int _lastColor = -1;

	public bool Color { get; set; } = true;

	#region Private methods
	private void SetTagOpenStatus(ushort token, bool open) {
		if (!this._openTags.ContainsKey(token)) {
			this._openTags.Add(token, open);
		} else {
			this._openTags[token] = open;
		}
	}

	private void CloseColor() {
		if (_lastColor < 0 || this._nodes.Count == 0) {
			return;
		}

		var index = this._nodes.Count - 1;
		if (index == this._lastColor && this._nodes[_lastColor].Type == NodeType.Color) {
			this._nodes.RemoveAt(this._lastColor);
			this._lastColor = -1;
			return;
		}
		if (index - this._lastColor < 0) {
			this._nodes.Add(Node.CreateColorCloseNode());
			this._lastColor = -1;
			return;
		}

		Node[] stack = new Node[index - this._lastColor];
		var i = stack.Length - 1;
		for (; index > this._lastColor; index--) {
			var node = this._nodes[index];
			if (node.Type == NodeType.Tag) {
				if (this.IsTagOpen(node.Token)) {
					stack[i] = node;
					i--;
					this._nodes.Add(Node.CreateTagCloseNode(_codeLookup[node.Token].Tag));
					this._openTags[node.Token] = false;
				}
			}
		}
		this._nodes.Add(Node.CreateColorCloseNode());
		for (i = 0; i < stack.Length; i++) {
			this.AddNode(stack[i]);
		}
		this._lastColor = -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void AddNode(Node node) {
		switch (node.Type) {
			case NodeType.Invalid: {
				return;
			}
			case NodeType.Tag: {
				if (this.IsTagOpen(node.Token)) {
					this._nodes.Add(Node.CreateTagCloseNode(_codeLookup[node.Token].Tag));
					this._openTags[node.Token] = false;
				} else {
					this._nodes.Add(node);
					this.SetTagOpenStatus(node.Token, true);
				}
				break;
			}
			case NodeType.Color: {
				if (this.Color) {
					if (_lastColor >= 0) {
						this._nodes.Add(Node.CreateColorCloseNode());
					}
					this._nodes.Add(node);
					this._lastColor = this._nodes.Count - 1;
				}
				break;
			}
			default: {
				this._nodes.Add(node);
				break;
			}
		}
	}

	private void AddTextNode(ReadOnlySpan<char> text, bool isUnsafe) {
		this._nodes.Add(Node.CreateTextNode(text, isUnsafe));
	}

	private bool IsTagOpen(char token) {
		this._openTags.TryGetValue(token, out bool result);
		return result;
	}

	private void Parse(string input, bool isUnsafe, bool isPrivileged ) {
		var enumerator = new TokenEnumerator(input);

		foreach (var token in enumerator) {
			switch (token.Token) {
				case '\0':
					break;
				case '0':
					this.CloseColor();
					break;
				default:
					if (token.Token < this._codeLookup.Length && this._codeLookup[token.Token] != null) {
						if (!this._codeLookup[token.Token].Privileged || isPrivileged) {
							this.AddNode(this._codeLookup[token.Token].GetNode());
						}
					} else {
						this.AddTextNode(token.Token.ToString(), isUnsafe);
					}
					break;
			}
			if (token.Text.Length != 0) {
				this.AddTextNode(token.Text, isUnsafe);
			}
		}
	}

	#endregion

	public Formatter(List<LotgdFormatCode> codes) {
		ushort highestToken = codes.Max(y => (ushort)y.Token);
		this._codeLookup = new LotgdFormatCode[highestToken+1];
		foreach (var code in codes) {
			_codeLookup[(ushort)code.Token] = code;
		}
	}

	/// <summary>
	/// Wether or not the formatter is clear of text and open tags.
	/// </summary>
	public bool IsClear() {
		return this._nodes.Count == 0
			&& this._openTags.Values.All(y => y);
	}

	/// <summary>
	/// Wether or not the formatter has pending markup or text to render.
	/// </summary>
	public bool HasContent() {
		return this._nodes.Any();
	}

	/// <summary>
	/// Clear all output and open tags.
	/// </summary>
	public Formatter Clear() {
		this.ClearText();
		this._openTags.Clear();
		return this;
	}

	/// <summary>
	/// Clear current output, but keep memory of currently open tags.
	/// </summary>
	public Formatter ClearText() {

		this._nodes.Clear();
		return this;
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
	/// <returns>
	/// The same instance of the formatter for easy chaining.
	/// </returns>
	public Formatter AddText(string input, bool isUnsafe = false,  bool isPrivileged = false) {
		this.Parse(input, isUnsafe, isPrivileged);
		return this;
	}

	/// <summary>
	/// Get the output of the formatter.
	/// </summary>
	public string GetOutput() {
		var totalLength = 0;
		for (int i = 0; i < this._nodes.Count; i++) {
			totalLength += _nodes[i].Output.Length;
		}
		Span<char> ouputSpan = stackalloc char[totalLength];
		int index = 0;
		int nodeLength;
		for (int i = 0; i < this._nodes.Count; i++) {
			nodeLength = this._nodes[i].Output.Length;
			this._nodes[i].Output.AsSpan().CopyTo(ouputSpan.Slice(index, nodeLength));
			index += nodeLength;
		}
		return ouputSpan.ToString();
	}

	/// <summary>
	/// Close the currently open tags.
	/// </summary>
	public Formatter CloseOpenTags() {
		this.CloseColor();

		foreach (var token in this._openTags.Keys) {
			if (this._openTags[token]) {
				var code = this._codeLookup[token];
				if (code.Tag != null) {
					this.AddNode(Node.CreateTagCloseNode(code.Tag));
				}
				this._openTags[token] = false;
			}
		}
		return this;
	}
}
