using System.Text;

namespace LotgdFormat;

#nullable disable

public class Formatter {
	private readonly Dictionary<char, LotgdFormatCode> _codeDictionary;
	private readonly List<Node> _nodes = new();
	private readonly Dictionary<char, bool> _openTags = new();
	private int _lastColor = -1;

	public bool Color { get; set; } = true;

	#region Private methods

	private void AddNode(Node node) {
		if (node.Type == NodeType.Tag) {
			if (this.IsTagOpen(node.Token)) {
				this._nodes.Add(Node.CreateTagCloseNode(_codeDictionary[node.Token].Tag));
				this._openTags[node.Token] = false;
			} else {
				this._nodes.Add(node);
				if (!this._openTags.ContainsKey(node.Token)) {
					this._openTags.Add(node.Token, true);
				} else {
					this._openTags[node.Token] = true;
				}
			}
		} else if (node.Type == NodeType.Color) {
			if (this.Color) {
				if (_lastColor >= 0) {
					this._nodes.Add(Node.CreateColorCloseNode());
				}
				this._nodes.Add(node);
				this._lastColor = this._nodes.Count - 1;
			}
		} else {
			this._nodes.Add(node);
		}
	}

	private void AddTextNode(string text, bool isUnsafe) {
		this._nodes.Add(Node.CreateTextNode(text, isUnsafe));
	}

	private bool IsTagOpen(char token) {
		if (!this._openTags.ContainsKey(token)) {
			return false;
		}
		return this._openTags[token];
	}

	private void Parse(string input, bool isUnsafe) {
		if (!input.Contains('`')) {
			this.AddTextNode(input, isUnsafe);
			return;
		}

		var lastToken = 0;
		while (true) {
			int i = input.IndexOf('`', lastToken);
			if (i == -1) {
				break;
			}
			if (input[i] != '`') {
				continue;
			}
			char token = input[i + 1];

			if (lastToken <= i - 1) {
				this.AddTextNode(input.Substring(lastToken, i - lastToken), isUnsafe);
			}
			lastToken = i + 2;
			if (token == '`') {
				this.AddTextNode("`", isUnsafe);
			} else if (token == '0') {
				if (_lastColor >= 0) {
					var index = this._nodes.Count - 1;
					List<Node> stack = new(index - this._lastColor);
					while (index > this._lastColor && index != this._lastColor) {
						var node = this._nodes[index];
						if (node.Type == NodeType.Tag) {
							if (this.IsTagOpen(node.Token)) {
								stack.Add(node);
								this._nodes.Add(Node.CreateTagCloseNode(_codeDictionary[node.Token].Tag));
								this._openTags[node.Token] = false;
							}
						}
						index--;
					}
					this._nodes.Add(Node.CreateColorCloseNode());
					foreach (Node node in stack) {
						this.AddNode(node);
					}
					this._lastColor = -1;
				}
			} else if (this._codeDictionary.TryGetValue(token, out var code)) {
				this.AddNode(code.GetNode());
			}
		}

		if (lastToken < input.Length) {
			this.AddTextNode(input.Substring(lastToken), isUnsafe);
		}
	}

	#endregion

	public Formatter(List<LotgdFormatCode> codes) {
		_codeDictionary = new Dictionary<char, LotgdFormatCode>();
		foreach (var code in codes) {
			_codeDictionary.Add(code.Token, code);
		}
	}

	/// <summary>
	/// Clear all output and open tags.
	/// </summary>
	public void Clear() {
		this.ClearText();
		this._openTags.Clear();
	}

	/// <summary>
	/// Clear current output, but keep memory of currently open tags.
	/// </summary>
	public void ClearText() {
		this._nodes.Clear();
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
		var output = new StringBuilder();
		for (int i = 0; i < this._nodes.Count; i++) {
			output.Append(_nodes[i].Output);
		}
		return output.ToString();
	}

	/// <summary>
	/// Close the currently open tags and return HTML for the respective closing tags.
	/// This also clears the output.
	/// </summary>
	public string CloseOpenTags() {
		var builder = new StringBuilder();
		foreach (var token in this._openTags.Keys) {
			var code = this._codeDictionary[token];
			if (code.Color != null) {
				builder.Append("</span>");
			} else if (code != null && code.Tag != null) {
				builder.Append(Node.CreateTagCloseNode(code.Tag).Output);
			}
		}

		this._openTags.Clear();
		this.Clear();
		return builder.ToString();
	}
}
