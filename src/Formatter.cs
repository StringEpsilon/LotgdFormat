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

	public Formatter(List<LotgdFormatCode> codes) {
		this._codes = codes;
	}

	public void Clear() {
		this.ClearText();
		this.OpenTags.Clear();
	}

	public void ClearText() {
		this._nodes.Clear();
	}

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
		if (code != null) {
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

	public void Parse(string input, bool isUnsafe) {
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
						this.AddNode(new ColorNode(token, code.Style));
						break;
					}
					case CodeType.SelfClosing: {
						this.AddNode(new SelfClosingNode(code.Tag));
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

	public Formatter AddText(string input, bool isUnsafe = false) {
		this.Parse(input, isUnsafe);
		return this;
	}

	public IHtmlContent GetOutput() {
		var output = new HtmlContentBuilder();
		foreach (INode node in this._nodes) {
			output.AppendHtml(node.Render());
		}
		return output;
	}

	internal IHtmlContent CloseOpenTags() {
		var builder = new HtmlContentBuilder();
		foreach (var token in this.OpenTags.Keys) {
			var code = this._codes.Find(y => y.Token == token);
			if (code?.GetCodeType() == CodeType.Color) {
				builder.AppendHtml("</span>");
			} else if (code != null) {
				builder.AppendHtml(new TagCloseNode(code.Tag).Render());
			}
		}

		this.OpenTags.Clear();
		this.Clear();
		return builder;
	}
}
