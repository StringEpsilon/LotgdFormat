namespace LotgdFormat;

/// <summary>
/// Walks over a string (as a readonly span) and produces <see cref="FormatToken"/>s. <br/>
/// </summary>
internal ref struct TokenEnumerator {
	private ReadOnlySpan<char> _inputString;

	public TokenEnumerator(ReadOnlySpan<char> input) {
		this._inputString = input;
		Current = default;
	}

	// For use in foreach():
	public readonly TokenEnumerator GetEnumerator() => this;

	public bool MoveNext() {
		var span = this._inputString;
		if (span.Length < 2) {
			return false;
		}

		var currentLength = span.Slice(2).IndexOf('`'); // next index of ` is the length of the text we got.
		if (currentLength == -1) {
			// handling of input end:
			currentLength = span.Length-2;
			_inputString = ReadOnlySpan<char>.Empty;
		} else {
			// slice off the handled section from the input:
			_inputString = span.Slice(currentLength + 2);
		}

		if (span[0] == '`') {
			if (span[1] == '`') { // Handle "``" by treating the second ` as part of the text.
				this.Current = new FormatToken(
					span.Slice(1, currentLength+1)
				);
			} else {
				this.Current = new FormatToken(
					span.Slice(2, currentLength), // text = everything after the code-token.
					span[1] // the code token itself is always the character after `
				);
			}
		} else {
			// If we have no `, we can return the entire span we got. The +2 is because we accounted for "`_"
			// when determining the currentLength (by starting from pos. 2).
			this.Current = new FormatToken(
				span.Slice(0, currentLength+2)
			);
		}
		return true;
	}

	public FormatToken Current { get; private set; }
}

