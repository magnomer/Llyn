# CCategory.cs
Hash: `5a81b26b3cc96c5d`

## `public sealed record CCategory(IReadOnlyList<CCategoryRow> CCategoryRows, bool CCategoryDeclared, bool CCategoryMatched, bool CCategoryShown)`

The category menu of the part of speech field, ready to show.

**Parameters**

- `CCategoryRows`: the parts the typed text matches, in catalog order.
- `CCategoryDeclared`: the engine's verdict that the draft language names any part.
- `CCategoryMatched`: the engine's verdict that some part matches the typed text.
- `CCategoryShown`: the engine's verdict that the menu opens for the typed text.

## `public string? CCategoryHint`

The key the menu shows in place of rows, or null while rows match.
A language that names no part and a text that matches none each have their own key.
Conduct chooses the key, so the menu only looks it up.
