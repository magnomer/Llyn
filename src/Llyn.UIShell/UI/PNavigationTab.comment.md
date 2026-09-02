# PNavigationTab.cs

## `public partial class PWindow`

Which panel the window shows.
The navigation buttons pick exactly one.
The chosen button wears the selected style.
Every other panel is collapsed out of the layout.

## Inline notes

### `if (PList.IsVisible && selectedButton != PNavigationList && !PWindowDiscardConfirm(PList.PListChangeCheck()))`

Leaving the list panel while it is being written in is leaving the editing state.
So it is asked about here.
A tab that is switched away from keeps its editor.
But the correction the user typed would sit out of sight until they came back.

## `internal void PWindowEntryShow(string id)`

Switches to the list panel and opens one Entry there.
It is the way a referring side listed under a shared record reaches the Entry holding it.
Panel switching is the window's to do, so a panel asks for it rather than reaching into `PNavigation` itself.

Every browse-style panel that holds an editor is asked the same question before the tab changes.
The list stays one guard per panel rather than one loop, because each panel names its own check.
