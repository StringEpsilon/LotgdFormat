namespace LotgdFormat;

/// <summary>
/// Walks over a string (as a readonly span) and produces <see cref="FormatToken"/>s. <br/>
/// </summary>
internal ref struct TokenEnumerator {
	private readonly ReadOnlySpan<char> _inputString;
	private FormatToken _current;
	private int _offset = 0;

	public TokenEnumerator(ReadOnlySpan<char> input) {
		this._inputString = input;
		_current = default;
	}

	// For use in foreach():
	public readonly TokenEnumerator GetEnumerator() => this;

	public bool MoveNext() {
		if (_offset == _inputString.Length) {
			return false;
		}
		var span = this._inputString.Slice(_offset);

		var currentLength = span.IndexOf('`'); // next index of ` is the length of the text we got.
		if (currentLength == -1 || span.Length == 1) {
			// handling of input end:
			this._current.Index = this._offset;
			this._current.Length = span.Length;
			this._current.Identifier = '\0';
			_offset = _inputString.Length;
			return true;
		}
		if (span[0] == '`') {
			if (span[1] == '`') { // Handle "``" by treating the second ` as part of the text.
				this._current.Index = this._offset+1;
				this._current.Length = currentLength+1;
				this._current.Identifier = '\0';
			} else {
				this._current.Index = this._offset+2;
				this._current.Length = currentLength;
				this._current.Identifier = span[1]; // the code token itself is always the character after `
			}
			_offset += currentLength+2;
		} else {
			// If we have no `, we can return the entire span we got.
			this._current.Index = this._offset;
			this._current.Length = currentLength;
			this._current.Identifier = '\0';
			_offset += currentLength;
		}
		return true;
	}

	public FormatToken Current { get => _current;  }
}
