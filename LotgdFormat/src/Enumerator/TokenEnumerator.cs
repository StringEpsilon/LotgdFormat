using System.Runtime.InteropServices;

namespace LotgdFormat;

/// <summary>
/// Walks over a string (as a readonly span) and produces <see cref="FormatToken"/>s. <br/>
/// </summary>
internal ref struct TokenEnumerator {
	private ReadOnlySpan<char> _inputString;
	private FormatToken _current;

	public TokenEnumerator(ReadOnlySpan<char> input) {
		this._inputString = input;
		_current = default;
	}

	// For use in foreach():
	public readonly TokenEnumerator GetEnumerator() => this;

	public bool MoveNext() {
		if (this._inputString.Length == 0) {
			return false;
		}
		var span = this._inputString;

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
				this._current.Text = span.Slice(1, currentLength+1);
				this._current.Token = '\0';
			} else {
				this._current.Text = span.Slice(2, currentLength); // text = everything after the code-token.
				this._current.Token = span[1]; // the code token itself is always the character after `
			}
		} else {
			// If we have no `, we can return the entire span we got. The +2 is because we accounted for "`_"
			// when determining the currentLength (by starting from pos. 2).
			this._current.Text = span.Slice(0, currentLength+2);
			this._current.Token = '\0';
		}
		return true;
	}

	public FormatToken Current { get => _current;  }
}

// | Method |     Mean |   Error |  StdDev |   Gen0 | Allocated |
// |  Parse | 274.0 ns | 1.52 ns | 1.42 ns | 0.0153 |     256 B |

// | Method |     Mean |   Error |  StdDev |   Gen0 | Allocated |
// |------- |---------:|--------:|--------:|-------:|----------:|
// |  Parse | 269.1 ns | 1.58 ns | 1.40 ns | 0.0153 |     256 B |

// |              Method |       Mean |     Error |    StdDev |   Gen0 |   Gen1 | Allocated |
// |-------------------- |-----------:|----------:|----------:|-------:|-------:|----------:|
// |               Parse |   274.4 ns |   2.01 ns |   1.88 ns | 0.0153 |      - |     256 B |
// | FormatRealistic_New | 8,099.4 ns | 156.56 ns | 153.76 ns | 1.8616 | 0.0916 |   31392 B |
