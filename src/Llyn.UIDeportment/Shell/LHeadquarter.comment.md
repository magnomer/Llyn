# LHeadquarter.cs

## `public static class LHeadquarter`

The decisions behind the product menu the logo opens.

## Inline notes

### `e.Handled = true;`

The mark sits inside the roof, and the roof drags the window on a left press.
Left unhandled, opening the menu would start a drag under it.

### `menu.IsOpen = false;`

A line puts the menu away before it acts.
A message box over an open popup would leave the popup standing behind it.

### `public static void LHeadquarterExitHandle(Window window, Popup menu, MouseButtonEventArgs e)`

Leaving asks the window to close rather than ending the process.
That is the same path the caption close takes, so unsaved work is still offered back.

### `private static string LHeadquarterVersionRead()`

Reads the build number the assembly carries.
`version.json` at the repository root is the only place a version is written.
`Directory.Build.props` reads it and stamps every assembly at compile time.
Only the first three parts are shown, because the fourth is always zero.
