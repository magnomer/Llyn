# LHeadquarter.cs

## `public static class LHeadquarter`

How the product menu the logo opens looks, and what its lines show.
It decides nothing, so its public lines are refines of the menu or the window.

## `public static void LHeadquarterMenuRefine(Popup menu, MouseButtonEventArgs e)`

Opens the menu when it is shut and shuts it when it is open.

## `public static void LHeadquarterAboutRefine(Window window, Popup menu, MouseButtonEventArgs e, string key)`

Puts the menu away and shows the product's about notice.
Conduct chooses the version line's wording `key`.
The product's name titles the notice, as every notice of the window is titled.

## `public static void LHeadquarterExitRefine(Window window, Popup menu, MouseButtonEventArgs e)`

Puts the menu away and asks the window to close rather than ending the process.
That is the same path the caption close takes, so unsaved work is still offered back.
The leave question is asked by the window's closing, so no gate stands here.

## `private static string LHeadquarterVersionRead()`

Reads the build number the assembly carries.
It stays with the driver, because Conduct is framed away from reflection.
`version.json` at the repository root is the only place a version is written.
`Directory.Build.props` reads it and stamps every assembly at compile time.
Only the first three parts are shown, because the fourth is always zero.

## Inline notes

### `e.Handled = true;`

The mark sits inside the roof, and the roof drags the window on a left press.
Left unhandled, opening the menu would start a drag under it.

### `menu.IsOpen = false;`

A line puts the menu away before it acts.
A message box over an open popup would leave the popup standing behind it.
