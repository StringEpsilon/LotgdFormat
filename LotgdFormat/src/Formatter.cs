using System.Runtime.CompilerServices;

namespace LotgdFormat;

#nullable disable

public class Formatter {
	private readonly Dictionary<char, LotgdFormatCode> _codeDictionary;
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
		if (_lastColor < 0 || this._nodes.Count == 0) {
			return;
		}

		var index = this._nodes.Count - 1;
		Node[] stack = new Node[index - this._lastColor];
		var i = stack.Length - 1;
		for (; index > this._lastColor; index--) {
			var node = this._nodes[index];
			if (node.Type == NodeType.Tag) {
				if (this.IsTagOpen(node.Token)) {
					stack[i] = node;
					i--;
					this._nodes.Add(Node.CreateTagCloseNode(_codeDictionary[node.Token].Tag));
					this._openTags[node.Token] = false;
				}
			}
		}
		this._nodes.Add(Node.CreateColorCloseNode());
		for (; i < stack.Length; i++) {
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
					this._nodes.Add(Node.CreateTagCloseNode(_codeDictionary[node.Token].Tag));
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

	private void AddTextNode(string text, bool isUnsafe) {
		this._nodes.Add(Node.CreateTextNode(text, isUnsafe));
	}

	private bool IsTagOpen(char token) {
		this._openTags.TryGetValue(token, out bool result);
		return result;
	}

	private void Parse(string input, bool isUnsafe) {
		int i = input.IndexOf('`');
		if (i == -1) {
			this.AddTextNode(input, isUnsafe);
			return;
		}

		var lastToken = 0;

		do {
			char token = input[i + 1];
			if (lastToken <= i - 1) {
				this.AddTextNode(input.Substring(lastToken, i - lastToken), isUnsafe);
			}
			lastToken = i + 2;
			switch (token) {
				case '`':
					this.AddTextNode("`", isUnsafe);
					break;
				case '0':
					this.CloseColor();
					break;
				default:
					if (this._codeDictionary.TryGetValue(token, out var code)) {
						this.AddNode(code.GetNode());
					}
					break;
			}
			i = input.IndexOf('`', lastToken);
		} while (i > 0);

		if (lastToken < input.Length) {
			this.AddTextNode(input.Substring(lastToken), isUnsafe);
		}
	}

	#endregion

	public Formatter(List<LotgdFormatCode> codes) {
		_codeDictionary = new Dictionary<char, LotgdFormatCode>(codes.Count);
		foreach (var code in codes) {
			_codeDictionary.Add(code.Token, code);
		}
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
	public Formatter AddText(string input, bool isUnsafe = false) {
		this.Parse(input, isUnsafe);
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
		Span<char> ouputSpan = new char[totalLength];
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
				var code = this._codeDictionary[token];
				if (code.Tag != null) {
					this.AddNode(Node.CreateTagCloseNode(code.Tag));
				}
				this._openTags[token] = false;
			}
		}
		return this;
	}
}
