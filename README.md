# LotgdFormat

A C# implementation of the LotgD style formatting code for text formatting.

## How to use this library

See API documentation. (TODO)

## Acknowledgements

This library is based on the `appoencode()` function found in LotgD 0.9.7. DragonSlayer V2.5. Known contributors to that specific version of the function are:

- MightyE
- JT Traub
- Eliwood
- Talion

## Known issues

The original handles tags followed by "\`0" differently than this library. I am not quite sure if the original has a bug there or if the behavior should be replicated here.

Example - original:
```
input:
`$red`Hnavhi`0 still red`0 still red.

output (indented for easier reading):
<span class="c36">
	red
	<span class='navhi'>
		navhi
	</span>
	still red
	still red.
```

Same input in this libary:
```
input:
`$red`Hnavhi`0 still red`0 still red.

output (indented for easier reading):
<span class=\"c36\">
	red
	<span class=\"navhi\">
		navhi
	</span>
</span>
<span class=\"navhi\">
	still red still red.
```

## Code syntax

Every code is marked with a backtick ` and consists of a single arbitrary character.

Example:

```
This text is `@green from here.
```

There are 4 types of codes:

- Color codes.
- Single format codes
- Paired format codes
- The closing code.

### Color codes

A color code creates a `span` tag with a specific class to change the text color within. The tag will encapsulate all text until either:

- The end of the text.
- The next color is encountered
- The closing code is encountered.

Examples:

```html
`@this is green `$this is red.
=>
<span class="lotgd-c64">this is green</span>
<span class="lotgd-c36">this is red</span>
```

This library treats all codes that only have a token and a color as color codes.

### Self-closing format codes

A self-closing format codes is used to generate self closing tags in the output. The most prominent example in LotgD is `n` for a `<br>`. Another possible usecase would be for creating an `hr` tag.

Since they are self contained, they have no interactions with other codes other than possibily being embedded.

Example:

```html
This is line one.`nThis is line two.
=>
This is line one.<br/>
This is line two.
```

This library only treats codes configured as such to be self closing.

### Regular format codes

These create a formatting tag such as `strong`, `i`, or `center`. They enclose the text following the tag until either the end of the text or if another instance of the same tag is encountered.

The closing tag does not affect them.

Example:

```html
`bThis is bold `ithis is italic
=>
<strong>This is bold <i>this is bold and italic<i></strong>
```

They can however be closed with themselves:

```html
`bThis is bold`b `ithis is bold and italic`i. This is normal.
=>
<strong>This is bold</strong> <i>this is italic<i>. This is normal.
```

This library treats all codes as regular format codes if they have a tag specified, even if also a color is specified.

### The closing code

The code `0` closes the currently open color span.

```html
`bBold `iand italic `$and red`0. This is no longer red.
=>
<strong>
	Bold
	<i>
		and italic
		<span class="lotgd-c36">and red</span>
	</i>
</strong>
<strong>
	<i>
		. This is no longer red.
	</i>
</strong>
```

This code is reserved and not configurable.

### Escaping

To render the backtick (`), the backtick needs to be doubled up:

```
This is a backtick: ``
=>
This is a backtick: `
```

----
## Appendix

The code definitions used in this files examples are:

| Token | Color  | HTML-Tag   | Self-closing |
| ----- | ------ | ---------- | ------------ |
| n     | -      | `<br>`     | true         |
| b     | -      | `<strong>` | false        |
| i     | -      | `<i>`      | false        |
| @     | 00FF00 | *none*     | false        |
| $     | FF0000 | *none*     | false        |
| 0     | -      | -          | -            |
