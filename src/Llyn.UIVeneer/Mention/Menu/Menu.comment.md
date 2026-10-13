# Menu

This folder holds the word menu's list, row chrome and row template, as markup alone.
The window markup merges it by its source, so no Deportment class loads it.
It restates the editor's popup list and row, since the editor's dictionary is merged only under the editor.

## `PMentionMenuTemplate.xaml`

`Theme.Mention.List` is a bare list that never takes the focus, so typing stays in the text being written.
`Theme.Mention.Item` is the row surface, lit under the pointer and tinted while selected, both switched by the look sheet.

### `Theme.Mention.Row`

One row holds the name, with the flag and language beside it.
The flag and language show only when the row stands for an Entry.
A Sense-mode row carries no language, and the fill folds that column away rather than leaving a gap.
The fill indents the row by its depth, so a sub-sense sits under its parent.
The row is a plain surface rather than a button, so the list beneath it keeps its own selection.

## Stamp

Commit: `c2f335c331fe`
- `PMentionMenuTemplate.xaml` `127541dade3ce021`
