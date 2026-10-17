# Library

This folder holds the library panel's gates, its vista among them.
It also covers the panel's markup import and its customs declaration.

## `TCustoms`

Covers the customs gate the import window drives, with no window.
It covers the mode and target a row starts with, and when a row is ready.
A row keeps its pick across mode changes, and only a Replace with a target words a loss.
The loss carries the target's ready card counts, and the rows read back in file order.

## `TLibrary`

Covers the library panel's Conduct end to end on a real workspace.
Its rows, query, order and filter follow the vista, which a fresh area already stands on.
The empty verdict follows the last rows read.
A chosen row is marked, reports its id and names the file, and a deleted one reports zero again.
The entry create gate answers true only when the entry saved, which needs a headword.
The entry delete gate answers true only for a stored id.
A missing id deletes nothing.
The entry export gate answers the written path for a stored id, and none for a missing id.
An entry jump through the navigation opens the entry in the area, asking nothing.
A row click on the tab records the station it leaves, and an empty click records nothing.
A row open with the scribe on opens the entry in the editor.
A quit while editing asks once and stores the entry.
The portrait export writes only a chosen entry, asks for the file, and shows the failure when the write fails.
A new order of none keeps the order.
The markup import lives in `TLibraryMarkup`.

## `TLibraryPrepare`

Builds the library panel over the atelier with the given envoy, and its marshal runs each notice at once.

## `TLibraryMarkup`

Covers the library panel's markup import on a real workspace.
It builds its panel through `TLibrary.TLibraryPrepare` and its envoy through `TLibraryEnvoyCreate`.
The markup import asks for the file first, and a cancelled pick asks nothing more.
It then asks the customs question once and stores under the declared rows.
The question shows each entry with its ready targets, and a clean file reports nothing after.
A merge row appends to its target, so the declared mode reaches the engine unchanged.
A clean file answers the count of entries stored.
A declined question stores nothing and answers none.
A malformed file shows the import failure.

## `TLibraryEnvoyCreate`

An envoy that answers the file question with a given file and the customs question through a given function.
It records every question it is asked.
The function drives the handed gate as a window would, then answers whether the user accepted.

## `TLibraryVista`

Covers the library panel's vista restore, its observers and its close on a real workspace.
A second restore carries the held query into the fresh vista.
After a second restore a reflex notice raises the rows once, through the marshal, from the fresh vista only.
A create alone restores once, so the same notice after only a create raises once too.
A workspace notice closes the chosen entry and tells the driver once.
A settings notice raises the rows once.
The menu offers four orderings.
The window's exit gate closes the editor and stops the recording.
The rows load answers the stored rows and the languages once the fill has run.

## Stamp

Commit: `52c21d68b4c4`
- `TCustoms.cs` `ff66ae9d6ea56f78`
- `TLibrary.cs` `710723af57107f0a`
- `TLibraryMarkup.cs` `4329941ca4943e9f`
- `TLibraryVista.cs` `3ea7f5cc6a857b72`
