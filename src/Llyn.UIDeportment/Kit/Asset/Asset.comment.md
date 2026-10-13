# Asset

The veneer's static material that the deportment draws with: scaffold parts, icons, words and palette.
It stands apart because the deportment reaches the veneer only through contract IDs and ports.

## `QContract`

The deportment's only door to the scaffold the veneer holds.
Each part is pulled by its contract ID alone, so the deportment never names the veneer.
A missing part throws at once, since a broken contract must not fail silently later.
`QContractFind` looks for the element carrying the ID as its `x:Name`.
It asks the name scope of the given scope first, then walks the logical tree.
The walk is needed because a nested page keeps its names in its own scope.
`QContractSheetFind` by ID alone finds an application resource.
A page marked `x:Shared="False"` comes back as a fresh instance on every call.
The overload with a scope walks up through its parents.
That finds a page-local dictionary merged by its markup, which is no application resource.
`QContractResolve` is the one rule every find shares.
The part must exist and carry the asked type.
Otherwise it throws, naming the contract kind, the ID and the asked type.
The kind is "element" for a named element and "resource" for a keyed resource.

## `QIcon`

Turns one icon asset into a frozen image an `Image` can display.
SharpVectors preserves separate fills, strokes, gradients and opacity instead of flattening every path to one geometry.
The asset file alone defines each symbol, so C# holds no path or palette data.
The veneer embeds the assets, and the deportment never names where.
`QIconVector` lists the icons that always load from their SVG source, never from a PNG.
`QIconResolve` loads a symbol on first use and keeps the frozen image under its name.
The name is the asset's file name without its extension.
The size validates the intended square at the call site, and the receiving `Image` supplies width and height.
WPF then scales the drawing uniformly.
It also builds cached grayscale images when an icon image reports an inherited disabled state.
`QIconAssetLoad` reads the asset under the folder the contract resource `PIconRoot` names.
The veneer supplies that resource, so no pack URI of the veneer appears here.
A PNG wins when one exists, and the SVG is the fallback.

## `QIconImage`

Displays an icon in its original colors while enabled and in grayscale while disabled.
It listens for effective enabled-state changes inherited from itself or any ancestor.
So a disabled parent button updates its icon automatically.
`QIconSource` holds the original icon apart from the displayed source, so repeated state changes preserve its colors.

## `QLocalizationCatalog`

The interface language read by key, so the deportment words what it shows without the veneer.
It is a bindable lookup rather than a resource reference.
A binding cannot carry a dynamic resource, so text inside a multi binding would freeze at load.
This one object stands in for the catalog and announces every key at once when the language changes.
`QLocalizationCatalogCurrent` is the one instance every binding names, so one change event reaches them all.
`QLocalizationCatalogMerged` remembers the catalog dictionary last merged into each resource dictionary.
It is a weak table, so a dropped resource dictionary does not keep its catalog alive.
The indexer answers empty on a miss, so a binding never shows a raw key.
`QLocalizationTextRead` answers the key itself when no locale declares it.
`QLocalizationTextFind` answers null instead.
A caller cannot tell the key itself from a translation, which is why the find exists.
`QLocalizationCatalogApply` publishes the texts of a loaded catalog into the resources and announces the change.
Each resource write walks the whole live tree, so one write per key froze launch and every settings toggle.
It returns untouched when every text already equals its resource.
Otherwise it fills a fresh dictionary off the tree with the previous texts and the new ones.
Carrying the previous texts keeps a key the new catalog lacks on its old text.
Swapping that dictionary into the merged list costs exactly one walk.
It stays the last merged dictionary, so it beats the theme dictionaries merged before it.
No locale text lives in veneer XAML, so no own key of the resources shadows it.
The caller loads the catalog, through the engine or, before one exists, through the localization port.

## `QTheme`

Fills the application resources with the palette an `LTheme` record carries.
The bootstrap loads the record once, so the panels and an exported page share one palette.
`QThemeColors` says which theme key fills which resource, and every one is required.
`QThemeApply` converts each named colour text into a `Color` resource under its resource key.
The reader is the theme's own colour lookup, handed in by the bootstrap so the veneer names no theme type.
The theme refuses a missing key itself, so the loop holds no branch over what it read.
A palette missing a key, or holding a text that is no colour, stops the launch before a panel draws.

## Inline notes

### QLocalizationCatalog.cs: `new PropertyChangedEventArgs("Item[]")`

The name WPF listens for when an indexer changes and no single index is named.

## Stamp

Commit: `c2f335c331fe`
- `QContract.cs` `850ec3785efc9939`
- `QIcon.cs` `c0795d7d784c7473`
- `QIconImage.cs` `e66b7e69f1938b90`
- `QLocalizationCatalog.cs` `444b1b62ac35c4ec`
- `QTheme.cs` `ed317c5b44388cc9`
