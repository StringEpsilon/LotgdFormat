using System.Buffers;

internal static class StringExtensions {

	internal static SearchValues<char> _safeHtmlCharacters = SearchValues.Create(
		"1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,;.:-_!?$()/\\@[]*%` \t\n"
	);

	internal static bool IsSafe(this in ReadOnlySpan<char> input) {
		return input.IndexOfAnyExcept(_safeHtmlCharacters) == -1;
	}

	internal static bool IsSafe(this string input) {
		return !input.AsSpan().ContainsAnyExcept(_safeHtmlCharacters);
	}

	internal static bool IsSafe(this ref char input) {
		return _safeHtmlCharacters.Contains(input);
	}
}
