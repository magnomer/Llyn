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
