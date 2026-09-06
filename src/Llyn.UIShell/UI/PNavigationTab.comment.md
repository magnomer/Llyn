# PNavigationTab.cs

## `public partial class PWindow`

Which panel the window shows.
The navigation buttons pick exactly one.
The chosen button wears the selected style.
Every other panel is collapsed out of the layout.
The tab chosen is pushed downstream as it changes, so the window opens on it next time.

## Inline notes

### `if (PLibrary.IsVisible && selectedButton != PNavigationLibrary && !PWindowDiscardConfirm(PLibrary.PLibraryChangeCheck()))`

Leaving the library panel while it is being written in is leaving the editing state.
So it is asked about here.
A tab that is switched away from keeps its editor.
But the correction the user typed would sit out of sight until they came back.

## `internal void PNavigationRestore(LWorkspaceState state)`

Puts the window back on the tab `state` names, and puts that tab back on the side it was left standing on.
It goes through the ordinary tab switch, so the restored tab is selected exactly as a click selects it.
A state naming no tab, or one this build no longer offers, leaves the window on the tab it opens with.

## `private (string Mode, Button Button, FrameworkElement Panel, Action<bool>? Scribe)[] PNavigationTabRead()`

The tabs of the window, each with the name it is stored under and the way to put it back on its editor.
The switch and the restore read the same table, so a tab cannot be stored under one name and restored under another.
A tab holding no editor offers no way to restore one.

## `internal void PWindowEntryShow(string id)`

Switches to the library panel and opens one Entry there.
It is the way a referring side listed under a shared record reaches the Entry holding it.
Panel switching is the window's to do, so a panel asks for it rather than reaching into `PNavigation` itself.
The library is asked first, because the tab guard above passes over a library that is already the target.
Opening the entry would otherwise cancel the draft being written in that panel without a word.

## `internal void PWindowExampleShow(string id)`

Switches to the corpus panel and opens one Example there.
A Source citing row names an Example rather than an Entry, so it needs the counterpart of the entry switch.
The corpus panel is asked first, for the same reason the library is.

Every browse-style panel that holds an editor is asked the same question before the tab changes.
The list stays one guard per panel rather than one loop, because each panel names its own check.
