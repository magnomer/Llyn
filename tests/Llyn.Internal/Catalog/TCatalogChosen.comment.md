# TCatalogChosen.cs

## `public sealed class TCatalogChosen`

Catalog rows carry the selected identity and disambiguated display names.
Clearing a selection clears every row's chosen flag.
The tests cover Examples, Situations, Sources, Authors, Tags, and Registers.
Duplicate and unknown Example labels receive consistent numbers.
A child query depends on its selected parent Tag and its own query.
The resulting Entry is chosen only while both filters match.

## `private static LVista TCatalogVistaCreate(LEngine engine, string tab, long id)`

Starts a catalog view and selects the supplied row, giving the cases a consistent selected-view setup.
