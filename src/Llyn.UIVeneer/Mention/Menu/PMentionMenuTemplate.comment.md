# PMentionMenuTemplate.xaml

## `Theme.Mention.List` and `Theme.Mention.Item`

The list and row chrome of the word menu.
They are the editor's popup list and row restated, because the editor's dictionary is merged only under the editor.
The window owns this popup and cannot see those keys.

## `Theme.Mention.Row`

One row of the menu: the name, with the flag and language beside it.
The flag and language show only when the row stands for an Entry.
A Sense-mode row carries no language, and the trigger folds that column away rather than leaving a gap.
The row is indented by its depth, so a sub-sense sits under its parent as the display nests its cards.
The row is a plain surface rather than a button, so the list beneath it keeps its own selection.

## `PMentionMenuTemplate.xaml.cs`

The dictionary forwards the row's click to the window that owns the popup.
The Prospect dictionary does the same for the editor's rows.
