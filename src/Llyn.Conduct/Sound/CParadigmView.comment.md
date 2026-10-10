# CParadigmView.cs
Hash: `ce2e6cbd1f144e5f`

## `public sealed record CParadigmView(CParadigmTable CParadigmViewCollapsed, CParadigmTable CParadigmViewExpanded)`

Both views of the inflection box, ready to show.
Toggling between them is a visual state, so Conduct holds no gate for it.

**Parameters**

- `CParadigmViewCollapsed`: the short view.
- `CParadigmViewExpanded`: the full view.

## `internal static CParadigmView? CParadigmViewCreate(LParadigmView? view)`

Maps the engine's answer, and a null answer stays null.
Each cell's text and tip arrive ready, since the engine applies the status rule below Conduct.
It adds no rule.
