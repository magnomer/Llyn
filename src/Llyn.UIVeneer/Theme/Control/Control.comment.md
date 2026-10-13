# Control

The control theme holds the looks of the shared interactive controls.
They are catalog rows, choices, commands, pills, paradigm tables, playback, print and volume.
Their hover, press, check and chosen states are switched by `QLook` rows in the deportment.
This is since the styles carry no trigger.

## Catalog styles

`Theme.Catalog.Row` is one row of a panel's catalog, drawn on nothing until it is pointed at.
The catalogs carry no card of their own, so a bordered row would stack a frame inside a frame.
A row shows its subject over a quieter line of what tells it apart.
The panel tags the row it stands on as chosen, and the `Chosen` cue is a `QLook` state.
The tag paints the soft accent ground and turns the subject accent, as the reading compass marks its current row.
One ground and one ink say chosen together, so no rail or edge repeats them.
`Theme.Catalog.Epithet` is the epithet run after a listed headword.
It is the reading the language pack names, such as `희롱할 롱` after `弄`.
It sits on the headword's baseline, two steps smaller and muted, so the headword stays the row's one bold word.
No binding fills it, since the row fill in the deportment writes its text.
That fill prepends an en space, so the run needs no margin of its own.
The row's title keeps its ellipsis, since both runs share one text block.
`Theme.Catalog.Pellet` is the count at the far end of a row, muted rather than accented.
That is because the accent marks the chosen row.
`Theme.Catalog.Empty` says a catalog holds nothing, lying over the rows that are not there.
It never takes the pointer, so a click through it still reaches the catalog.

## Catalog frame and scrolling

`Theme.Catalog.HeaderMargin` brings each catalog's sort and search row to the same horizontal edges as its list rows.
It offsets the panel's 34-pixel inset, leaving the shared 6-pixel outer gutter.
It ends where the row surface ends before the scroll rail.
`Theme.Catalog.Frame` offsets the 34-pixel panel margin to leave a 6-pixel left gutter.
`Theme.Catalog.Middle` offsets the seam 16 pixels before an interior column to leave the same 6-pixel gutter.
Both end at the next seam, where `Theme.Catalog.Scroll` opens its lane once the rows outrun the panel.
Library, Sound, Tags, Situations, Examples, Sources and Favorites share these styles.
The document and editor scroll viewers hold the same lane at the same width.

## Choice styles

`Theme.Choice.Row` is one row the user picks from, a whole row that takes the click.
The entry index and the ordering dropdown use it, and templating it avoids the platform's square, grey Button chrome.
It is not a picker-popup row, because those carry no click of their own and offer their actions separately.
Those are `Theme.Popup.RowSurface`.
The rows sit on the panel's own ground.
So they carry a card of their own, and the hover row in `QLook` still repaints it.
`Theme.Choice.Order` is one ordering in a sort dropdown.
`QChoice` builds these radio buttons in code, so no markup names the style.
The tick stays collapsed until `QLook` shows it on the checked one.
`Theme.Choice.Filter` is one language row of a filter dropdown.
It is drawn like an ordering row with a box in place of the tick.
A shown language is boxed in accent and inked, and a hidden one is bare and muted.
So a dropdown with every box ticked reads at a glance as hiding nothing.
`Theme.Choice.Bare` styles the frame fields of an Example, which are typed into like text.
They offer what the language has saved and are framed only under the pointer.
The frame is drawn outward, so a field being pointed at or written in never moves the sentence beside it.
The frame is closed onto the text it rings, clearing it by a hairline and no more.
A marker and a role are parted by one space, so a frame drawn outward crosses the word beside it.
The dropdown shows at once with no fade, so the pick never looks slow to answer.
`Theme.Choice.Switch` is the on-off switch of the settings pages.
It carries no trigger, so `QLook` rows colour the track and slide the knob when checked.
`Theme.Choice.Switch.KnobInset` is the knob's margin while a switch is on, so it rests against the right edge of the track.

## Command styles

`Theme.Command.Group` is the tray a panel's commands sit in, and the same tray the input header's pair sits in.
It has no ground, edge or shadow of its own.
So the commands read as commands rather than as a boxed group.
Each command keeps its own hover and press, which is where a button says it is a button.
A disabled tray is dimmed whole, there being no edge left to pale.
`Theme.Command.IconToggle` is an icon-only command that holds a menu open.
It is tinted while its menu stands open, so the tray says which picker is showing.
`Theme.Command.Segment.Template` draws one command inside the tray, for a button and for a mode radio alike.
The icon is the button's `QLook.QLookIconProperty`, drawn by a `QIconImage` part.
A command with no icon collapses the icon part and keeps its word alone.
The label is a TextBlock the template owns, because the app-wide TextBlock style outranks an inherited colour.
Deportment fills the icon, the label and every state through `QLook` rows on the named parts.
State is set on the button rather than on the surface, so a style below can overrule it.
`Theme.Command.Primary` is the one command in a tray that commits, filled so the eye finds it without reading.
It drops to the quiet treatment when it cannot be pressed, because a filled slab reads as pressable.
`Theme.Command.Mode` is the two halves of the reading and writing switch, which is a choice rather than two commands.
The chosen half is tinted, so the panel says which half it is on before it is asked.
A disabled switch keeps that tint, because the mode it stands on is still true.

## Language styles

`Theme.Language.Inset` is the room between the pill's edge and its face, written once for the chip and the toggle.
`Theme.Language.Chip` is the blue pill an entry's language is named on.
The reading view wears it, and the editor's toggle copies it.
Both are defined here, so the two views cannot drift apart.
It carries the toggle's one-pixel border in its own fill, so the two pills measure the same.
`Theme.Language.Face`, `Theme.Language.Flag` and `Theme.Language.Expand` shape the row of flag, name and chevron a pill shows.
The chip and the toggle both fill it from these styles, so their faces are one shape.
The chevron says the editor's pill opens a menu.
Its icon is a `QLook` row, since the icon extension is logic.
The reading view draws the chevron hidden, so its pill keeps the chevron's room.
The heart and the stars after the pill then stand still when the mode switches.
`Theme.Language.Toggle` is the editor's pill, drawn as the reading view's pill is drawn.
It stays a toggle, because a language is chosen here and only reported there.
The border matches the fill until the pill is pointed at or opened, then turns accent.

## Paradigm styles

`Theme.Paradigm.Box` is the frame the paradigm rows sit in, above the first meaning.
The inflection switch and both inflection tables sit in the same frame below the rows.
It draws a hairline outline with the corner and padding of the link cards.
So it reads as one of the cards on the page.
Its ground stays clear, so the table's own rules carry the structure and no tint competes with the chips.
It hugs its table rather than stretching.
That is because a stretched box would park the switch under the contents popup at the page's far right.
`Theme.Paradigm.Part` is the part of speech heading a group of rows, in the accent like the part chips above.
Empty headings keep their text block because this style defines no visibility trigger.
The text styles pin the interface font, since the box itself carries the headword font for the forms.
`Theme.Paradigm.Name` is the name of the form, muted so the form itself stays the mark.
An inflection table uses `Theme.Paradigm.Label` instead, which keeps this look with table spacing.
The label's vertical margin matches the forms, so every cell of a line shares one band.
`Theme.Paradigm.Group` is the group name of an inflection table, such as a mood, beside its group's first line.
It is accent and semibold, like the particle and dependence labels of a sentence.
So the row headings read as headings.
`Theme.Paradigm.Text` is the form, inked and in the headword font the box inherits from `QFontRefine`.
The size is fixed here, so the headword's own size does not carry into the rows.
The template carries no bindings, so `QParadigm` writes each row's text and tip.
A form that is not text draws a placeholder mark in the muted ink.
Its tooltip names the lookup state the mark stands for.
`Theme.Paradigm.Form` is one form in an inflection table, a step larger, with room above each rule and between columns.
`Theme.Paradigm.Header` is a column header, and its deeper bottom margin sets the header row off from the closing rule.
`Theme.Paradigm.Rule` is the faint dotted line under a table line that another line of the same group follows.
`Theme.Paradigm.Close` is the solid line under the header row and under the last line of a group.
It closes a block, so the eye finds where each group ends without a plate.
Both rules never take the mouse, so the cells beneath keep their tooltips.
`Theme.Paradigm.Switch` is the track of the switch between the short and the full table.
Its tinted ground and round corners make the two words read as one segmented control.
It sits at the table's right edge, and the bottom margin parts it from the table.
`Theme.Paradigm.Fold` is one side of the switch, whose template part `PSurface` is a clear thumb.
The state sheet gives the checked side the page ground, a faint outline and accent ink, since markup never branches.
The default focus visual stays, so a keyboard user still sees which button holds focus.
`Theme.Paradigm.Marked` holds letters the rules did not predict, in `Theme.Warning` so they stand out in red.
`Theme.Paradigm.Cut` is the faint hyphen between root and ending of a divided form, in `Theme.Muted`.
So it does not read as a letter.
`Theme.Paradigm.Row` has three columns for the part, name and form.
The part and name columns share their widths across rows, so the forms line up.
Each text block is named, so Deportment finds it in a realized row.

## Playback styles

`Theme.Playback.Action` is the play button.
It is drawn as a command inside the playback tray rather than as a control of its own.
It is the icon toggle's treatment without the held state, because playing is done the moment it is asked for.
Both views draw it, because a recording is played where it is heard and where it is chosen.
`Theme.Playback.Action.PressScale` is the shrink a pressed action takes, set by key so the deportment holds no size.

## Press styles

The part merges the palette for its brushes and the scroll part for the canvas base style.
`Theme.Press.Column` is the settings column of the print window, at a fixed width beside the canvas.
Its 370 leaves the labels their width beside the window's 232 wide controls.
It has no ground of its own, so the settings sit on the window like the settings page.
`Theme.Press.Canvas` is the page canvas on the raised surface, so white paper stands out in any palette.
It builds on the app scroll viewer, so its bars match every other list.
`Theme.Press.Paper` is one printed sheet, white in every palette because the printed output is white.
Its shadow is the popup shadow, so the sheet floats the way a popup does.
`Theme.Press.Shade` dims a sheet that the chosen range leaves out.
The page stays in place and in order, so page numbers never shift.
`Theme.Press.Badge` is the small tag on a dimmed sheet that says it is not printed.
`Theme.Press.Veil` is the translucent cover over the canvas while pages render or after rendering failed.
The last pages show through it, so the reader keeps their place.
`Theme.Press.Bar` is the strip under the canvas for page moves and zoom, with a hairline on top.

## Volume styles

`Theme.Volume.Tray` is the bare tray the volume slider stands in, beside the primary pronunciation row.
It takes the reading row's height, so the two sit on one line in both views.
It wears no surface, so the slider reads as part of the row.
It starts collapsed, and the view shows it while a row has a recording.
`Theme.Volume.Rail` is the two halves of the volume track.
The taken half is filled in the accent and the remaining one is clear.
They are repeat buttons because that is what a WPF track is built from.
A click on either walks the volume toward it.
The slider names both, so a `QLook` row hands each its large-step command.
`Theme.Volume.Thumb` is a ring of the accent around the surface, filling as it is pointed at and solid while dragged.
It is a ring rather than a dot, so the track it sits on stays readable underneath it.
`Theme.Volume.Slider` is the volume of a played recording, from silence to full over its own width.
It runs zero to one, the range a media player takes.
Nothing between the grip and the sound rescales it.
A click anywhere on the track moves the grip there.
That is because a volume is chosen by where it should be rather than nudged toward it.

## Stamp

Commit: `c2f335c331fe`
- `PThemeCatalog.xaml` `f51d9c83102a77d5`
- `PThemeChoice.xaml` `168b1d5620f22041`
- `PThemeCommand.xaml` `cfac497267cf2e60`
- `PThemeLanguage.xaml` `65e4d1c6ffa6c7d2`
- `PThemeParadigm.xaml` `61034931bc36ccb6`
- `PThemePlayback.xaml` `d516781fe3e24f8b`
- `PThemePress.xaml` `e55c56f07808710c`
- `PThemeVolume.xaml` `f99ff1e4696d5eee`
