# CParadigmView.cs
Hash: `ad196d08e0878d2e`

## `public sealed record CParadigmView(CParadigmTable CParadigmViewCollapsed, CParadigmTable CParadigmViewExpanded)`

Both views of the inflection box, ready to show.
Toggling between them is a visual state, so Conduct holds no gate for it.

**Parameters**

- `CParadigmViewCollapsed`: the short view.
- `CParadigmViewExpanded`: the full view.

## `internal static CParadigmView? CParadigmViewCreate(LParadigmView? view, bool held)`

Maps the engine's answer, and a null answer stays null.
The held flag is true for the editor.
It adds no rule.
