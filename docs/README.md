# Technical documentation

## Principle

The formatter works by first parsing the text into a list of nodes, then rendering each node in turn.

```
`@This is`0 `ban example`b
```

Becomes
```
{
	{ ColorNode, Color = "00FF00" }
	{ TextNode,  Text = "This is" }
	{ ColorCloseNode }
	{ TagNode, Tag = "strong" }
	{ TextNode, Text = "An example" }
	{ TagCloseNode, Tag = "strong" }
}
```

Becomes:

```html
<span class="c26">
this is
</span>
<strong>
An example
</strong>
```

When handling the `0 code, the formatter has to backtrack to close and reapply reapply non-color formatting for the following node to produce the correct HTML.

```
`@This `bis an`0 example`b
```

Becomes
```
{
	{ ColorNode, Color = "00FF00" }
	{ TextNode,  Text = "This" }
	{ TagNode, Tag = "strong" }
	{ TextNode,  Text = "is an" }
	{ ColorCloseNode }
	{ TagCloseNode, Tag = "strong" }
	{ TagNode, Tag = "strong" }
	{ TextNode, Text = "example" }
	{ TagCloseNode, Tag = "strong" }
}
```

Becomes
<span style="color: #00FF00">This <strong>is an</strong></span> <strong> example</strong>

```html
<span style="color: #00FF00">
This
<strong>
is an
</strong>
</span>
<strong>
example
</strong>
```
