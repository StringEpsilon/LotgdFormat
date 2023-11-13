# 0.10.0:

**BREAKING:**
- Formatter methods no longer return the instance.

**Regular changes:**
- A small optimization in the parsing (<1% speedup)
- Skip parsing step entirely if input is null or empty
- Do not emit a tag if it's immediately closed.

# 0.9.0:
- Added an overload to `Formatter.AddText` that accepts a `ReadonlySpan<char>`
- Speedup constructor and reduce memory footprint.

# 0.8.1:
- Fixed handling of unknown tokens.
	Before: "\`_text" -> "text"
	Now: "\`_text" -> "_text"
	Added a test for this also.

# 0.8.0:

Added support for privileged codes. Codes with the `Privileged` flag set will be ignored in all texts by default, unless `AddText()` is called with the new optional `isPriviliged` parameter set to true.


# 0.7.5

- Changed how the `style` on a node is rendered to match the original implementation.
	- Allows to configure any arbitrary attribute data for the rendered tag, such as `class="foo"`

# 0.7.4

- Improved parsing performance and reduced heap allocation.

# 0.7.3

- Fixed a faulty tag closing when `ClearText()` was used while an open color is still "on the stack".
	(See `ColorCloseMultipleTexts` test.)

# 0.7.2

- Fixed unhandled exception when multiple texts where added in between `ClearText()` that had color close tokens (`0)
	- basically the parser tried to close a color that no longer with none on the stack, causing an index out of bounds exception.

# 0.7.1

- Fixed incorrect handling of \`0 that comes immediately after a color tag. I.E. \`@\`0.

