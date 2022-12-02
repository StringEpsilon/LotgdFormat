# Nodes

As explained in the ["Principle"](./README.md#principle) section, the formatter creates a list of nodes to then render.

The following nodetypes are available:

| Name        | Summary                                 |
| ----------- | --------------------------------------- |
| Text        | Plain text                              |
| Color       | Renders the opening `span` for coloring |
| ColorClose  | Renders the closing `span` for coloring |
| SelfClosing | Renders a self-closing HTML tag         |
| Tag         | Renders a `<tag>`                       |
| TagClose    | Renders the corresponding `</tag>`      |

All nodes implement the INode interface:

```c#
public interface INode {
	public IHtmlContent Render();
}
```

The Render function is called on each node by `Formatter.GetOutput()` and the results concatenated.

## Text

**Class:** `TextNode`
**Properties:**
| Name     | Type     | Description                                              |
| -------- | -------- | -------------------------------------------------------- |
| Text     | `string` | The text to render.                                      |
| IsUnsafe | `bool`   | Whether or not to render unsafe HTML. **Default: False** |

## Color

**Class:** `ColorNode`
**Properties:**
| Name  | Type   | Description                                     |
| ----- | ------ | ----------------------------------------------- |
| Token | `char` | The token character associated with this color. |

## ColorClose

**Class:** `ColorCloseNode`
**Properties:**
*none*

## SelfClosing

**Class:** `SelfClosingNode`
**Properties:**
| Name | Type     | Description                     |
| ---- | -------- | ------------------------------- |
| Tag  | `string` | Name of the HTML tag to render. |

## Tag

**Class:** `TagNode`
**Properties:**
| Name   | Type     | Description                                   |
| ------ | -------- | --------------------------------------------- |
| Token  | `char`   | The token character associated with this tag. |
| Tag    | `string` | Name of the HTML tag to render.               |
| Styles | `string` | Additional `style` to add to the tag.         |

## TagClose

**Class:** `TagCloseNode`
**Properties:**
| Name | Type     | Description                    |
| ---- | -------- | ------------------------------ |
| Tag  | `string` | Name of the HTML tag to close. |

