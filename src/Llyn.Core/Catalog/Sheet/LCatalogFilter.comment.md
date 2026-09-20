# LCatalogFilter.cs

## `public sealed record LCatalogFilter(IReadOnlyList<string> LCatalogFilterHidden)`

The languages a browsing panel keeps out of its list.
It records what is hidden rather than what is shown.
A language pack added later is therefore shown until the user hides it.
An empty record hides nothing, which is how every panel opens.

**Parameters**

- `LCatalogFilterHidden` — The language names whose rows the panel leaves out.

## `public static readonly LCatalogFilter LCatalogFilterEmpty`

The filter that hides nothing, shared so no panel builds its own.

## `public bool LCatalogFilterActive`

Whether any language is hidden, which is what the panel marks its button with.

## `public bool LCatalogFilterMatch(string? language)`

Whether a row in `language` stays in the list.
A row with no language stays, because nothing was hidden by that name.
Names are compared without case, as the language folders are.

## `public IReadOnlyList<LCatalogRow> LCatalogFilterApply<LCatalogRow>(IReadOnlyList<LCatalogRow> rows, Func<LCatalogRow, string?> language)`

The rows whose language is not hidden, read through `language`.
An empty filter hands the same list back untouched.
