using Microsoft.AspNetCore.Html;

namespace LotgdFormat;

#nullable enable

public class Formatter {
	private readonly List<LotgdFormatCode> _codes;
	private readonly List<INode> _nodes = new();
	private readonly Dictionary<char, bool> OpenTags = new();
	private int _currentIndex;
	private int _lastColor = -1;

	public bool Color { get; set; } = true;

	#region Private methods

	private void AddNode(INode node) {
		this._nodes.Add(node);
		this._currentIndex = this._nodes.Count - 1;
		if (node is TagNode tagNode) {
			if (!this.OpenTags.ContainsKey(tagNode.Token)) {
				this.OpenTags.Add(tagNode.Token, true);
			} else {
				this.OpenTags[tagNode.Token] = true;
			}
		}
		if (node is ColorNode) {
			this._lastColor = this._currentIndex;
		}
	}

	private void AddCloser(LotgdFormatCode code) {
		if (code?.Tag != null) {
			this.AddNode(new TagCloseNode(code.Tag));
			this.OpenTags[code.Token] = false;
		}
	}

	private void AddTextNode(string text, bool isUnsafe) {
		this.AddNode(new TextNode { Text = text, IsUnsafe = isUnsafe });
	}

	private bool IsTagOpen(char token) {
		this.OpenTags.TryGetValue(token, out bool isOpen);
		return isOpen;
	}

	private void Parse(string input, bool isUnsafe) {
		if (!input.Contains('`')) {
			this.AddTextNode(input, isUnsafe);
			return;
		}

		var sections = input.Split('`');
		if (!string.IsNullOrEmpty(sections[0])) {
			this.AddTextNode(sections[0], isUnsafe);
		}

		bool escaped = false;
		foreach (string section in sections.Skip(1)) {
			if (section.Length == 0) {
				if (escaped == true) {
					this.AddTextNode("`", isUnsafe);
				}
				escaped = !escaped;
				continue;
			}
			char token = section[0];
			string text = section.Substring(1);
			if (token == '0') {
				if (_lastColor > 0) {
					List<INode> stack = new();
					var index = this._currentIndex;
					while (index > 0 && index != this._lastColor) {
						if (this._nodes[index] is TagNode tagNode) {
							if (this.IsTagOpen(tagNode.Token)) {
								stack.Add(this._nodes[index]);
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
			} else {
				var code = this._codes.Find(y => y.Token == token);
				switch (code?.GetCodeType()) {
					case CodeType.Color: {
						if (!this.Color) {
							break;
						}
						if (_lastColor >= 0) {
							this.AddNode(new ColorCloseNode());
						}
						this.AddNode(new ColorNode(token));
						break;
					}
					case CodeType.SelfClosing: {
						if (code.Tag != null){
							this.AddNode(new SelfClosingNode(code.Tag));
						}
						break;
					}
					case CodeType.Formatting: {
						if (this.IsTagOpen(token)) {
							this.AddCloser(code);
						} else if (code.Tag != null) {
							this.AddNode(new TagNode(token, code.Tag, code.Style));
						}
						break;
					}
					default: {
						// Nothing to do.
						break;
					}
				}
			}
			if (text.Length > 0) {
				this.AddTextNode(text, isUnsafe);
			}
		}
	}

	#endregion

	public Formatter(List<LotgdFormatCode> codes) {
		this._codes = codes;
	}

	/// <summary>
	/// Clear all output and open tags.
	/// </summary>
	public void Clear() {
		this.ClearText();
		this.OpenTags.Clear();
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
		foreach (var token in this.OpenTags.Keys) {
			var code = this._codes.Find(y => y.Token == token);
			if (code?.GetCodeType() == CodeType.Color) {
				builder.AppendHtml("</span>");
			} else if (code != null && code.Tag != null) {
				builder.AppendHtml(new TagCloseNode(code.Tag).Render());
			}
		}

		this.OpenTags.Clear();
		this.Clear();
		return builder;
	}
}
