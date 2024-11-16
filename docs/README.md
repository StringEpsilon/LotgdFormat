For documentation of the differences between this implementation and the original, see [Deviations.md](./Deviations.md)

# How to use

First, instantiate a formatter, then use `AddText()`:

```cs
var formatter = new Formatter([
	new LotgdFormatCode('n', tag: "br", selfClosing: true),
	new LotgdFormatCode('b', tag: "b"),
	new LotgdFormatCode('i', tag: "i"),
	new LotgdFormatCode('@', color: "00FF00"),
	new LotgdFormatCode('$', color: "00FF00"),
]);

var html = formatter.AddText("`bLegend`b of the `@Green`0 Dragon.");
// html == "<b>Legend<b> of the <span class='c64'>Green</span> Dragon"
```

The formatter does keep track of tags that were opened but not closed.
You can check the status via `Formatter.IsClear()`.
And generate the appropriate closing tags with `Formatter.CloseOpenTags()`.

# Code syntax

Every code is marked with a backtick ` and consists of a single arbitrary character.
**Example:**

```
This text is `@green from here.
```

There are 4 types of codes:

- Color codes.
- Single format codes
- Paired format codes
- The closing code.

## Color codes

A color code creates a `span` tag with a specific class to change the text color within. The tag will encapsulate all text until either:

- The end of the text (the formatter does not add a closing span).
- The next color is encountered
- The closing code is encountered.

Examples:

```html
`@this is green `$this is red.
=>
<span class="c64">this is green</span>
<span class="c36">this is red</span>
```

**Example configuration:**
```cs
var greenColorCode = new LotgdFormatCode('@', color: "00FF00");
```


## Self-closing format codes

A self-closing format codes is used to generate self closing tags in the output. The most prominent example in LotgD is `n` for a `<br>`. Another possible usecase would be for creating an `hr` tag.

Since they are self contained, they have no interactions with other codes other than possibily being embedded.

**Example:**
```html
This is line one.`nThis is line two.
=>
This is line one.<br/>This is line two.
```

**Example configuration:**
```cs
var newLineCode = new LotgdFormatCode('b', tag: "br", selfClosing: true);
```

## Regular format codes

These create a formatting tag such as `strong`, `i`, or `center`. They enclose the text following the tag until either the end of the text or if another instance of the same tag is encountered.

The color closing tag does not affect them.

Example:

```html
`bThis is bold `ithis is italic
=>
<strong>This is bold <i>this is bold and italic
```

They can however be closed with themselves:

```html
`bThis is bold`b `ithis is bold and italic`i. This is normal.
=>
<strong>This is bold</strong> <i>this is italic<i>. This is normal.
```

**Example configuration:**
```cs
var boldCode = new LotgdFormatCode('b', tag: "strong");
var italicCode = new LotgdFormatCode('i', tag: "i");
```

## The color closing code

The code `0` closes the currently open color span. It is hardcoded and must not be overwritten by configuration. If no color is open, it simply has no effect.

```html
`$This sentence is red.`0 This one is not.
=>
<span class="c36">This sentence is red.</span> This one is not.
```

## Escaping

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
