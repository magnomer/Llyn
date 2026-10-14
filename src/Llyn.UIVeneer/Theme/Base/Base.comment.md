# Base

The base theme holds the roles every other theme part builds on.
They are palette, input, panel, popup, scroll, search and navigation.
The theme is one dictionary per role rather than one file for every role at once.
Each part names the styles of a single subject and merges only the parts it draws from.
A part therefore resolves its own StaticResource and BasedOn references without reaching sideways.
Hover, press, chosen, check and disabled looks are switched by `QLook` in the deportment.

## Theme root

This file merges the parts in dependency order and holds no style of its own.
It names only veneer parts, since the deportment holds no markup at all.
`PBootstrap.xaml` keeps merging this one file, so the theme has one address.
The Press part follows the Command part and rests on the palette and the scroll part.
The Mention, Etymology, Gloss and Paradigm parts are merged late, on the palette and at most the input part.
The Script, Fanqie, Diwei and Xiesheng parts close the list, each on the palette and at most two more parts.

## `PThemeDefault`

Gives windows the default canvas, ink and font.
Gives text blocks without an explicit foreground the default ink.

## `PThemePalette`

Defines the shared colour brushes and font families, taken from the active theme.
Colour families are ink and muted, canvas, roof and navigation, surface and raised surface, line and strong line.
The accent family has hover, press, edge and soft variants.
The helper family marks secondary card fields, and the situation family marks situation fields.
The warning family covers caution, soft fills and destructive emphasis.
Favorite and representative are semantic colours for those entries.
Disabled line and ink colour disabled controls, and `Theme.Pending` marks content or actions that remain pending.
`Theme.Font` selects the interface font with system fallbacks.
`Theme.Phonetic` selects fonts for phonetic transcription.
`Theme.Han` selects fonts for Han characters with regional and system fallbacks.

## `PThemeInput`

Every template part takes its control's values through `QLook` rather than a template binding.
`Theme.Input.Field` switches undo off in every field.
`QChronicle` takes Ctrl+Z and Ctrl+Y for the entry's own history, and a field's private undo would fight it.
The TextBox content host must be named `PART_ContentHost`, a WPF template contract.
That name is authored in code in `QEnvelope`, so it stays out of the audited XAML surface.
`Theme.Input.Choice` has a toggle over the whole field that opens the dropdown, its check bound by `QLook`.
The arrow names its asset by pack URI, since a markup extension from code would be a hook.
The dropdown shows at once with no fade, so the pick never looks slow to answer.
`Theme.Input.Tab` is a tab of the editor's strip, whose selected or idle cue `QLook` reads from `QLook.QLookCueProperty`.
The tab style sets no tag, so the editor tags each tab when its strip attaches.
`Theme.Input.Headword` has the size and weight of `Theme.Text.Headword`, with no frame or padding.
So the headword stands still when the reader turns to writing.
`Theme.Input.Bare` is a field with no frame, for writing straight over what the reading side draws.
Its frame is drawn outward on hover and focus by `Theme.Input.Field.Bare`, so the text never moves.
`Theme.Input.Caret` is a bare field that takes no room while it is empty and unfocused.
`QLook` then zeroes its width and sets `Theme.Input.CaretMargin`, whose negative right side cancels the left one.
On focus `QLook` drops both values, so the field's own width and margin return.
`Theme.Text.Reading` collapses a reading with no text, through `QLook`.

## `PThemeNavigation`

Holds the shared focus, tab, voyage, divider and gap resources for navigation.
`Theme.Navigation.Tab` targets a plain `Button`, so the tab is a styled button and carries no state of its own.
`Theme.Navigation.Voyage` styles the back and forward buttons that revisit navigated records.
`Theme.Navigation.Gap` adds transparent spacing between groups without drawing a line.

## `PThemePanel`

`Theme.Panel.Surface` is the region a panel gives a whole screen of its own, drawn on the plain surface ground.
It carries no outline.
That is because a region is not an object and a one-pixel outline could not say that it was.
The outline it once carried was the only thing marking the region, which is why the region read as unfinished.
What separates one region from the next is a seam, not a box drawn round each of them.
`Theme.Panel.Helper` is the same region shape on the helper ground, one step cooler and deeper than the canvas.
A helper is an aid to the work rather than the work.
A reader must see that before reading a word of it.
It stays inside the program's own blue-grey, because a helper must not out-colour a choice or a verdict.
`Theme.Page.Title` heads a page, the read or edit area a panel gives one thing on its own.
The colophon and the settings pages share it, so a page reads the same wherever it opens.
`Theme.Page.Helper` is the one-line purpose under a page heading in the muted ink.
`Theme.Page.Note` is the small muted line under a control's label.
`Theme.Page.Rule` parts one row of a page from the next, laid only between rows and never around them.
`Theme.Panel.SeamRow` is the hairline across a panel where its controls end and its contents begin.
It is drawn at the foot of its row and bled past the panel's margin.
So it runs the width of the window.
`Theme.Panel.SeamColumn` runs down the gutter between two columns.
It is set against the right column's leading edge, pulled back into the middle of the gutter.
It is bled past the panel's foot.
A seam marks a division between two regions, never an enclosure around one.

## `PThemePopup`

`Theme.Popup.Surface` is the floating ground a picker popup stands on.
It is the same white as a field.
So the rows inside need no card and the list reads as one surface.
`Theme.Popup.Title` says what the popup is, once and quietly, since the rows are the content.
`Theme.Popup.Notice` is the one line shown while there are no rows, either searching or nothing found.
`Theme.Popup.Progress` shows the search running as a hairline under the title rather than a box of its own.
`Theme.Popup.ProgressBar` is the sliding bar inside it, shared because the phonetician and the downloader run the same search.
`QLook` starts the slide when the bar loads.
`Theme.Popup.ProgressBar.Sweep` is registered under its contract ID, which the deportment starts while the bar shows.
It starts at the bar's resting offset, so the bar never jumps when the sweep begins.
`Theme.Popup.RowSurface` is one row with no border and no fill at rest, and it takes no click.
A row can offer more than one action, so every action is its own button.
Phonetician and downloader rows are the same row.
`Theme.Popup.RowChoice` is the taking button, a pill carrying one word, and its template binds nothing.
The look sheet copies the content into `PLabel` as text, and the fill and edge into `PSurface`.
So it cannot carry an image, which is why `Theme.Popup.RowReading` exists.
`Theme.Popup.RowReading` takes one reading on a phonetician row and presents whatever content the reading gives it.
`Theme.Popup.IconAction` is a row's second action, carrying an icon instead of a word.

## `PThemeScroll`

Every rail in the app is one design at one size, upright or sideways.
A page, a catalog, a popup and a text field all show the same thread.
`QIndicator` registers the two control templates and the lane template.
WPF requires a `PART_Track` name, and that name must stay out of the audited XAML naming surface.
The sideways size and template are set by `QLook`, not by a trigger.
The lane template is the default for every scroll viewer, not an opt-in.
A viewer the app never names would otherwise keep the platform rail at a size nobody chose.
Examples are viewers inside a dropdown or a text field.
`Theme.Scroll.Gutter` is the name a page uses to ask for the lane it already has.
`Theme.Scroll.Lane` is a band that scrolls sideways and never up.

## `PThemeSearch`

`Theme.Search.Bar` is the ground every panel's ordering control and search field stand on together.
Every searching panel stands on it, the two duplex wings included, so a search reads alike everywhere.
The outline is clear at rest and appears under the pointer or on keyboard focus.
An edge therefore marks what the control is doing rather than that it exists.
It is held at one pixel throughout.
So a bar that gains its edge does not shift the row beneath it.
The bar is the placement target of the ordering dropdown, so the dropdown falls from the whole control.
A bar with no name hangs the dropdown from its dropper instead, offset back to the bar's own edge.
`Theme.Search.Dropper` is the ordering button inside the bar with no frame or ground of its own.
It takes a soft ground only under the pointer or while its dropdown is open.
It is unfocusable, so tabbing into the bar reaches the field the user came to type in.
`Theme.Search.Field` is the search field on one line without a frame, on the plain template that draws the placeholder.
A field whose matches drop beneath the bar hooks its own key and focus events itself.
`Theme.Search.Helper` is a folding control standing next to the bar, serving a panel the bar does not own.
It borrows the bar's height and corner, so the two shapes read as one row of controls over the catalog.
It carries a marked label like the command row does.
The mark is left out when no icon is named.
The label is left out when no content is named, so it stands as a square mark alone.
An open panel is shown by the soft accent ground the mode buttons use.
That is because the fold is a state and not an action.
`QLook` leaves out the mark and label and switches every look.
`Theme.Search.Helper.MarkInset` is the icon margin of a helper without a label, so the icon sits centred.
`Theme.Search.Helper.MuteInset` is the padding of such a helper, even on both sides of its icon.

## Stamp

Commit: `c6e60141a172`
- `PTheme.xaml` `80319431283b47c1`
- `PThemeDefault.xaml` `f10311a6336a0666`
- `PThemeInput.xaml` `ab744099ece5f95a`
- `PThemeNavigation.xaml` `117b631efd938fd7`
- `PThemePalette.xaml` `be59d4de986fccd8`
- `PThemePanel.xaml` `ab2e3967da3cb3d9`
- `PThemePopup.xaml` `ac36471dcdc422e9`
- `PThemeScroll.xaml` `fbe237bdd4bdd4e3`
- `PThemeSearch.xaml` `fb9e04cba9a51ef9`
