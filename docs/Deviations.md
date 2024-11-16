# Color-Tag nesting

The original handles nesting of tags and colors differently. LotgdFormat aims to always produce valid HTML without
leaving tags unclosed, whereas the original has several common scenarios that leave tags open.

Example 1 (tag and color):
```html
Input:  `$`bRed and Bold`0 only bold
Original:
<span class="c36">
	<strong >Red and Bold
</span> only bold

LotgdFormat:
<span class="c36">
	<b>Red and Bold</b>
</span>
<b> only bold
```

# "color close" inside a tag.

The original has a quirk where formatting tags that use the `span` html tag have a bad interaction with the color close
token when the tag is not closed:

Example:
```
input:
`$red`Hnavhi`0 still red
output:
<span class="c36">
	red
	<span class="navhi">navhi</span>
	still red still red
```
The fist span can only be closed with \`H`0.

Same input in LotgdFormat:
```html
<span class="c36">
	red
	<span class='navhi'>navhi</span>
</span>
<span class='navhi'> still red
```

Here the \`H tage is closed, then the color is span closed and a second \`H tag is opened to continue it's formatting.
See above chapter about nesting.
