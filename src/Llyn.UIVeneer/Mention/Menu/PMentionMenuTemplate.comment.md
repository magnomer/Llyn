# PMentionMenuTemplate.xaml

## `ResourceDictionary`

The list, row chrome and row template of the word menu, as markup alone.
The Deportment class of the same name loads this markup and forwards the row click to the window.
It restates the editor's popup list and row, since the editor's dictionary is merged only under the editor.

## `<Style x:Key="Theme.Mention.List" TargetType="ListBox">`

A bare list that never takes the focus, so typing stays in the text being written.

## `<Style x:Key="Theme.Mention.Item" TargetType="ListBoxItem">`

The row surface, lit under the pointer and tinted while the row is selected.
The look sheet switches both states on the surface.

## `<DataTemplate x:Key="Theme.Mention.Row">`

One row of the menu: the name, with the flag and language beside it.
The flag and language show only when the row stands for an Entry.
A Sense-mode row carries no language, and the fill folds that column away rather than leaving a gap.
The fill indents the row by its depth, so a sub-sense sits under its parent.
The row is a plain surface rather than a button, so the list beneath it keeps its own selection.
