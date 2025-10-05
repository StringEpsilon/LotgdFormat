# 0.16 (Future)
- Slight performance improvement in the output generation.
- Target .NET 10.

# 1.15.2
- Throw an exception when trying to configure an `LotgdFormatCode` with "0" or "\`" token.
- Updated documentation.
- Revert the trimming of whitespace. It can cause some text errors in the output (e.g. Hello`@World).

# 0.15.1
- Fix color tags not being closed when the follow-up color is added in a separate fragment.
  See unit tests `Unstacks_Colors_MultiFragment` for details.

# 0.15.0
- Added new Formatter constructor that takes a `FormatterConfig`
- Performance: Trim newlines and tabs off text leafs.
- Add .NET 9.0 to target frameworks.

# 0.14.0
**BREAKING**
- `new LotgdFormatCode()` will now throw when attempting to construct invalid configuration, such as
	`new LotgdFormatCode(color: null, tag: null)`
- Switched to using `HtmlEncoder` over `HttpUtility.HtmlEncode`

  This improves performance on medium to large inputs that contain characters that should be encoded. Bigger inputs see
  bigger gains. But it also slightly changes how individual characters may be encoded.
  On small inputs (< 64 characters), especially without characters that need escaping, there is a minor performance
  regression.

- Made several enums and structs internal that were erroneously public before.

**Regular changes:**
- Added some missing documentation comments on `LotgdFormatCode`
- Added short-circuit for `CloseOpenTags()` if there are no output nodes (saves some memory allocations)
- Avoid generating zero-length text nodes when unstacking and restacking tags on "color close" (`0).
  - Avoids some memory allocation and saves loop iterations in 3 places.
- Use an ArrayPool for the temporary string allocations needed to HTML encode output text.

# 0.13.4
- Fixed color tracking where the use of "`0" could prevent the emission of a new color span
  if the new color is the same as the previously closed:

For example
```
Some `@green `0text. `@More green`0
=>
Some <span class="c64>greeen </span> text. More green
```

Now correctly renders

```
Some `@green `0text. `@More green`0
=>
Some <span class="c64>greeen </span> text. <span class="c64">More green</span>
```

# 0.13.3
- Performance: The assembly of the final output string of `.AddText()`  has been sped up.
- Performance: Bypass `HttpUtility.HtmlEncode` for strings containing only whitelisted characters.
- Performance: Improve construction time of `LotgdFormatCode`.

# 0.13.2
- Bugfix: Inputs ending on ` will no longer crash.
- Performance: If input contains no tokens, shortcut the output.
- Performance: Shortcut length 1 strings of "&" in html encode (into `&amp`).

# 0.13.1
- Bugfix: Code at the very beginning of the input could misbehave in some scenarios.

# 0.13

**BREAKING:**
- Removed `Formatter.GetOutput()`, instead `AddText(...)` and `CloseOpenTags()` return output directly.
- Added return type `string` to `Formatter.AddText(...)` and `Formatter.CloseOpenTags()`.
- Removed `Formatter.ClearText()`
- `Formatter.Clear()`  only clears open tags now (w/o generating output, of course)
- `Formatter.IsClear()`  only indicates open tags now.
- The `Formatter` instance no longer keeps nodes in between calls of `AddText()` and `CloseOpenTags()`.
- Formatter omits `\n` in the output (but not `<br/>`) in some cases.
- Made `Node`  internal
- Added constructor to `LotgdFormatCode`.
- Made `LotgdFormatCode` properties read-only.


# 0.12
- Do not re-emit spans when on color tags when the new color is already the current color.

For example:
```
`@green `@green still
used to emit:
<span class="c64">green </span><span class="c64">green still</span>
now emits:
<span class="c64">green green still</span>
```

This should not break any formatting, but reduce the size of the output and can prevent some edge case bugs when using
the formatter in ASP.NET Core razor views.

# 0.11.1:
- Close color correctly when calling `CloseOpenTags()` after Nodes have been cleared.

# 0.11:

**BREAKING:**
- Library now targets .NET8.0 only.

**Regular changes:**
- Fixed exception caused by looking at `.Last()` open node when trying to close a tag without checking if that collection has
  any items to begin with.


# 0.10.1:
- Bugfix: When eliminating empty tags, the tag was not properly flagged as closed, which caused incorrect markup to be
	rendered. Example input: ` ``b``b ``bbold text``b ` rendered as ` </b>bold text<b>` instead of ` <b>bold text</b>`

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

