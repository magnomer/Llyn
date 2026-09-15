# PEditorFanqie.cs

## `public partial class PEditor`

The fanqie box of the editor: the same control as the reading view's, folded under its head.

## `internal void PEditorFanqieShow()`

Reads the pack's books and hypothesis for the entry's language and the entry's stored rows.
It asks the engine to fetch what is missing first.
A fetch started here reaches the editor by bulletin like any other.
An unsaved entry has no id and shows nothing.
A failed read hands the box nothing.

## `private void PEditorFanqieRebuild(long entry)`

Asks the engine to fetch the entry's placements again, then shows the box in its fetching state.
The fetched rows land through the fanqie bulletin, as a first fetch does.
