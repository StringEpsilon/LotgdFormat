// SPDX-License-Identifier: GPL-2.0-only
namespace LotgdFormat;

using System.Buffers;

internal static class StringExtensions {

	internal static SearchValues<char> _safeHtmlCharacters = SearchValues.Create(
		"1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,;.:-_!?$()/\\@[]*%` \t\n"
	);

	internal static bool IsSafe(this in ReadOnlySpan<char> input) {
		return input.IndexOfAnyExcept(_safeHtmlCharacters) == -1;
	}

	internal static bool IsSafe(this ref char input) {
		return _safeHtmlCharacters.Contains(input);
	}

	internal static int CountUnsafe(this ReadOnlySpan<char> input) {
		int count = 0;
		var unsafeIndex = input.IndexOfAnyExcept(_safeHtmlCharacters);
		while(unsafeIndex != -1 && input.Length > 0) {
			input = input.Slice(unsafeIndex+1);
			unsafeIndex = input.IndexOfAnyExcept(_safeHtmlCharacters);
			count++;
		}
		return count;
	}
}
