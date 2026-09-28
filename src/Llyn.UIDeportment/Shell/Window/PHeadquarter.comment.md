# PHeadquarter.cs

## `public partial class PWindow`

The product menu the mark opens, and what its two lines do.
The mark is the only opener the window has that is not a caption button.
Clicking it toggles the menu, so a second click on the mark puts it away.
It decides nothing, so each handler is a refine of the menu or the window.

## `private Popup PHeadquarter`

The menu is read through the loaded window's name scope.

## `private void PLogoRefine(object sender, MouseButtonEventArgs e)`

Opens the menu when it is shut and shuts it when it is open.
The mark sits inside the roof, and the roof drags the window on a left press.
Left unhandled, opening the menu would start a drag under it.

## `private void PHeadquarterAboutRefine(object sender, MouseButtonEventArgs e)`

Puts the menu away and shows the product's about notice.
A message box over an open popup would leave the popup standing behind it.
Conduct chooses the version line's wording key.
The product's name titles the notice, as every notice of the window is titled.
The build number is the one the assembly carries, since Conduct is framed away from reflection.
`version.json` at the repository root is the only place a version is written.
`Directory.Build.props` reads it and stamps every assembly at compile time.
Only the first three parts are shown, because the fourth is always zero.

## `private void PHeadquarterExitRefine(object sender, MouseButtonEventArgs e)`

Puts the menu away and asks the window to close rather than ending the process.
That is the same path the caption close takes, so unsaved work is still offered back.
The leave question is asked by the window's closing, so no gate stands here.
