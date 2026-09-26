# PAutograph.xaml

## `<UserControl.Resources>`

The title, field, heading and chip styles of the edit area.
The chips are built on the speech chip, so the control merges the speech theme to reach it.
The union rows come from `PGuildTemplate.xaml`, merged here.
The markup carries no class, and the edit area control loads it and wires its named parts.

## `<StackPanel Margin="9,0,20,30">`

The edit area, the reading with bare fields in place of its values.
The name is a bare field at the head of the page, and the chips beneath it stay as read.
The union section folds this Author into another: a typed name lists the Authors it matches, and one click folds.
The notice stands in its place while the Author is unsaved, because nothing can be folded into an unstored row.
It carries no buttons of its own, saving through the rail and deleting through the panel bin.

## `<ItemsControl x:Name="PAutographUnionList" ...>`

The Authors the typed name matches, drawn with the union row.
The edit area control fills each row and takes the list's clicks.
