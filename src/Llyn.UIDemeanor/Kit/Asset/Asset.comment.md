# Asset

The material the console draws with: the parts a blueprint names, the words and the palette.
It stands apart from `Shell` because no piece here knows the program, only the console.

## `BContract`

How a driver finds the parts it fills in, by the names the blueprint gives them.
It finds a blueprint in a shell by name, or a part in a blueprint by its `x:Name`.
A missing name throws `InvalidOperationException`, so a driver fails at construction, not while drawing.

## `BLocalizationCatalog`

The one place a console key resolves, as `QLocalizationCatalog` is for the GUI.
It is an instance the bootstrap owns, since a driver may hold no mutable static state.

## `BLocalizationCatalogApply`

Merges a catalog over the one in place and replaces the whole dictionary.
A key the new catalog lacks keeps its earlier text, as the GUI merge does.

## `BLocalizationTextRead`

Returns the text of a key, or the key itself when no catalog carries it, as the GUI does.

## `BTheme`

Turns a role such as `side` or `legend-label` into the escape sequence that colours a cell.
Its source is a function from role to text like `cyan on darkBlue`, so the theme names no file.
Both sides take one of the sixteen console colours, and `#RRGGBB` may follow a colour.
The basic code comes first and the exact one after it, so a terminal without true colour keeps the basic.
A background code is the foreground code plus 10.
A value that is not two colours, names an unknown colour or holds a bad `#RRGGBB` throws `FormatException`.

## `BThemeRead`

Answers the escape of a role, or the escape of `text` when the role is unknown.
The constructor reads every role of the spare table once, so a read never fails afterwards.
A role the source lacks takes the escape of `text`, but a missing `text` throws.

## `BThemeSpare`

The built-in `console-blue` palette, used when the theme file cannot be loaded.
A test pins it equal to `themes/console-blue.json`, so the two cannot drift apart.
`BThemeDefault` names that theme as the one a start loads.

## Stamp

Commit: `92c4426f8506`
- `BContract.cs` `d250c5a54a36eb89`
- `BLocalizationCatalog.cs` `6b3081293256d764`
- `BTheme.cs` `bd98e705df60123b`
