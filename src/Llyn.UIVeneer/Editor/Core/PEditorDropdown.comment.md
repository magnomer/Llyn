# PEditorDropdown.xaml
Hash: `1fb3025a80aa0ef9`

## `UserControl`

The five dropdowns of the shared editor as markup alone.
The editor places it once, and the drivers find each popup by its fixed name.
It merges the dropdown shells of `PEditorPopup.xaml`, so its lists draw as before.
Its row templates come from the editor around it, which already merges them.

## `<Popup x:Name="PNotation" ...>`

The pronunciation menu, declared once beside the other popups the editor owns.
It has no placement target of its own, because the row button that opens it becomes the target.
Closing it, by a pick or a click elsewhere, calls the search off.

## `<Popup x:Name="PClip" ...>`

The audio menu, declared once for the same reason and opened the same way.

## Hooks

The markup carries no hook.
The Deportment drivers `QNotation`, `QClip`, `QProspect`, `QProffer` and `QSlate` open, fill and place the popups.
