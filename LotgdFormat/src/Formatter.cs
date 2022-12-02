using Microsoft.AspNetCore.Html;

namespace LotgdFormat;

#nullable enable

public class Formatter {
	private readonly Dictionary<char, LotgdFormatCode> _codeDictionary;
	private readonly List<INode> _nodes = new();
	private readonly Dictionary<char, bool> _openTags = new();
	private int _lastColor = -1;

	public bool Color { get; set; } = true;

	#region Private methods

	private void AddNode(INode node) {

		if (node is TagNode tagNode) {
			if (this.IsTagOpen(tagNode.Token)) {
				this._nodes.Add(new TagCloseNode(tagNode.Tag));
				this._openTags[tagNode.Token] = false;
			} else {
				this._nodes.Add(node);
				if (!this._openTags.ContainsKey(tagNode.Token)) {
					this._openTags.Add(tagNode.Token, true);
				} else {
					this._openTags[tagNode.Token] = true;
				}
			}
		} else if (node is ColorNode) {
			if (this.Color) {
				if (_lastColor >= 0) {
					this._nodes.Add(new ColorCloseNode());
				}
				this._nodes.Add(node);
				this._lastColor = this._nodes.Count - 1;
			}
		} else {
			this._nodes.Add(node);
		}
	}

	private void AddCloser(string tag, char token) {
		this.AddNode(new TagCloseNode(tag));
		this._openTags[token] = false;
	}

	private void AddTextNode(string text, bool isUnsafe) {
		this.AddNode(new TextNode { Text = text, IsUnsafe = isUnsafe });
	}

	private bool IsTagOpen(char token) {
		this._openTags.TryGetValue(token, out bool isOpen);
		return isOpen;
	}

	private void Parse(string input, bool isUnsafe) {
		if (!input.Contains('`')) {
			this.AddTextNode(input, isUnsafe);
			return;
		}

		var lastToken = 0;

		while (true) {
			int i = input.IndexOf('`', lastToken);
			if (i < 0) {
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
			i++;
			if (token == '`') {
				this.AddTextNode("`", isUnsafe);
			} else if (token == '0') {
				if (_lastColor >= 0) {
					List<INode> stack = new();
					var index = this._nodes.Count - 1;
					while (index > 0 && index != this._lastColor) {
						if (this._nodes[index] is TagNode tagNode) {
							if (this.IsTagOpen(tagNode.Token)) {
								stack.Add(tagNode);
								this.AddCloser(tagNode.Tag, tagNode.Token);
							}
						}
						index--;
					}
					this.AddNode(new ColorCloseNode());
					foreach (INode node in stack) {
						this.AddNode(node);
					}
					this._lastColor = -1;
				} else {
				}
			} else if (token != '`') {
				if (!this._codeDictionary.ContainsKey(token)) {
					continue;
				}
				var code = this._codeDictionary[token];
				INode? node = code.GetNode();
				if (node != null) {
					this.AddNode(node);
				}
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
	public IHtmlContent GetOutput() {
		var output = new HtmlContentBuilder();
		foreach (INode node in this._nodes) {
			output.AppendHtml(node.Render());
		}
		return output;
	}

	/// <summary>
	/// Close the currently open tags and return HTML for the respective closing tags.
	/// This also clears the output.
	/// </summary>
	public IHtmlContent CloseOpenTags() {
		var builder = new HtmlContentBuilder();
		foreach (var token in this._openTags.Keys) {
			var code = this._codeDictionary[token];
			if (code.Color != null) {
				builder.AppendHtml("</span>");
			} else if (code != null && code.Tag != null) {
				builder.AppendHtml(new TagCloseNode(code.Tag).Render());
			}
		}

		this._openTags.Clear();
		this.Clear();
		return builder;
	}
}
