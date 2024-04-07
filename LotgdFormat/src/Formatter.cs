using System.Runtime.InteropServices;
using System.Web;

namespace LotgdFormat;

#nullable enable

public class Formatter {
	private readonly HashArray<LotgdFormatCode> _codeLookup;
	private char? _currentColor;
	private readonly List<Node> _nodes = new();
	private readonly Dictionary<char, bool> _openTags = new();
	private int _lastColor = -1;
	private bool _color = true;

	public bool Color { get => _color; set => _color = value; }

	#region Public methods
	public Formatter(List<LotgdFormatCode> config) {
		var codeArray = CollectionsMarshal.AsSpan(config);
		Span<char> keys = stackalloc char[config.Count];
		for (int i = 0; i < codeArray.Length; i++) {
			keys[i] = codeArray[i].Token;
		}
		this._codeLookup = new HashArray<LotgdFormatCode>(keys, codeArray);
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

		var enumerator = new TokenEnumerator(input);
		foreach (var token in enumerator) {
			switch (token.Identifier) {
				case '\0':
					break;
				case '0':
					this.CloseColor();
					break;
				default:
					var code = this._codeLookup.Get(token.Identifier);
					if (code != null) {
						if (!code.Privileged || isPrivileged) {
							this.AddNode(new Node(code));
						}
					} else {
						this._nodes.Add(new Node(token.Index - 1, 1, isUnsafe));
					}
					break;
			}
			if (token.Length != 0) {
				if (token.Length == input.Length) {
					// we got the entire span back as text => no formatting token present
					return isUnsafe || input.IsSafe()
						? input
						: HttpUtility.HtmlEncode(input);
				}
				this._nodes.Add(new Node(token.Index, token.Length, isUnsafe));
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
				if (code?.Tag != null) {
					this.AddNode(new Node(NodeType.TagClose, code));
				}
				this._openTags[token] = false;
			}
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
		if (index == this._lastColor && this._nodes[_lastColor].Type == NodeType.Color) {
			this._nodes.RemoveAt(this._lastColor);
			this._lastColor = -1;
			this._currentColor = null;
			return;
		}

		Span<Node> stack = new Node[index - this._lastColor];
		var i = stack.Length - 1;
		for (; index > this._lastColor; index--) {
			var node = this._nodes[index];
			if (node.Type == NodeType.Tag && this.IsTagOpen(node.Token)) {
				var code = this._codeLookup.Get(node.Token);
				if (code != null) {
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


	private void AddNode(in Node node) {
		switch (node.Type) {
			case NodeType.Invalid: {
				return;
			}
			case NodeType.Tag: {
				var code = this._codeLookup.Get(node.Token);
				if (code != null && this.IsTagOpen(node.Token)) {
					if (this._nodes.Count > 0 && this._nodes.Last().Token == node.Token) {
						this._nodes.RemoveAt(this._nodes.Count - 1);
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
				if (this._color) {
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

	private bool IsTagOpen(in char token) {
		this._openTags.TryGetValue(token, out bool result);
		return result;
	}

	private string CreateOutput(ReadOnlySpan<char> input) {
		int nodeCount = this._nodes.Count;
		string[] outputs = new string[nodeCount];

		for (int i = 0; i < nodeCount; i++) {
			var node = _nodes[i];
			LotgdFormatCode? code = node.Token == '\0'
				? null
				: this._codeLookup.Get(node.Token);
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
