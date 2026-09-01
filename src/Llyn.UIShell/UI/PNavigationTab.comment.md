# PNavigationTab.cs

## `public partial class PWindow`

Which panel the window shows: the navigation buttons pick exactly one, and the chosen button wears the selected style while every other panel is collapsed out of the layout.

## Inline notes

### `if (PList.IsVisible && selectedButton != PNavigationList && !PWindowDiscardConfirm(PList.PListChangeCheck()))`

Leaving the list panel while it is being written in is leaving the editing state, so it is asked about here: a tab that is switched away from keeps its editor, but the correction the user typed would sit out of sight until they came back to it.
