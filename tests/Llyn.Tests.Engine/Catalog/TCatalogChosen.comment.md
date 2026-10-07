# TCatalogChosen.cs
Hash: `11fa574e87a195bd`

## `public sealed class TCatalogChosen`

Catalog rows carry the selected identity and disambiguated display names.
Clearing a selection clears every row's chosen flag.
The tests cover Examples, Situations, Sources, Authors, Tags, and Registers.
Duplicate and unknown Example labels receive consistent numbers.
A child view's rows depend on its parent's selected Tag and on its own query.
The resulting Entry is chosen only while both filters match.
A child view's rows follow the child's ordering, and equal headwords go by language.

## `private static LVista TCatalogVistaCreate(LEngine engine, string tab, long id)`

Starts a catalog view and selects the supplied row, giving the cases a consistent selected-view setup.

## `private static LVista TCatalogTagSave(LEngine engine, params (string, string)[] entries)`

Saves one entry per pair, each carrying the one Tag the first save mints.
It returns a parent view with that Tag selected.
