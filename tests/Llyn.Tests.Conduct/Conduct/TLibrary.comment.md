# TLibrary.cs
Hash: `2f98baa702fe7710`

## `public sealed class TLibrary`

Covers the library panel's Conduct end to end on a real workspace.
Its rows, query, order and filter follow the vista, which a fresh area already stands on.
The empty verdict follows the last rows read.
A chosen row is marked and reported as the station, and a deleted one reports zero again.
An entry jump through the navigation opens the entry in the area, asking nothing.
A row click on the tab records the station it leaves, and an empty click records nothing.
A row open with the scribe on opens the entry in the editor.
A quit while editing asks once and stores the entry.
The portrait export writes only a chosen entry, asks for the file, and shows the failure when the write fails.
A new order of none keeps the order.
The markup import lives in `TLibraryMarkup`.

## `internal static CLibrary TLibraryPrepare(CAtelier atelier, CEnvoy envoy)`

Builds the library panel over the atelier with the given envoy, and its marshal runs each notice at once.

## `internal static LEntry TLibraryEntrySave(LEngine engine, string headword, string language)`

Stores one entry with a single card under the headword and language.
