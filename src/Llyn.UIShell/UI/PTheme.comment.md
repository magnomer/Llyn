# PTheme.xaml

## `<Style TargetType="ScrollBar">`

Scroll viewers use a clear, comfortably sized rail instead of the platform scrollbar.
Its leading margin leaves a gutter between the document and the rail.
It disappears with the scrollbar when no scrolling is needed.
The two control templates are registered by PIndicator.
WPF requires a PART_Track name.
That name must stay out of the audited XAML naming surface.

## `<Setter Property="Template" Value="{DynamicResource Theme.Input.Field.Template}" />`

The TextBox content host must be named "PART_ContentHost" (a WPF template contract).
That name is authored in code (PField) so it stays out of the audited XAML surface.

## `<Style x:Key="Theme.Input.Choice" TargetType="ComboBox">`

A field typed into that also offers what has been saved before it.
It reads as a plain field, because a value nothing ships is written far more often than it is picked.
The arrow is hidden while nothing has been saved, so an empty list never offers an empty menu.
Text search is off and the drop stays open while typing, so the list never overwrites what is being written.
WPF requires the editing box to be named "PART_EditableTextBox" and the drop "PART_Popup".
Both are template contracts and are held out of the audited XAML naming surface.

## `<Style x:Key="Theme.Choice.Row" TargetType="Button">`

One row the user picks from.
It is a whole row that takes the click, used by the entry index and by the ordering dropdown.
Keeping it templated avoids falling back to the platform's square, grey Button chrome.
This is not a picker-popup row.
Those carry no click of their own and offer their actions separately.
They are Theme.Popup.RowSurface.

## `<Setter Property="Background" Value="{StaticResource Theme.Surface}" />`

The rows sit on the panel's own ground, so they carry a card of their own.
The hover trigger below still repaints it.

## `<Style x:Key="Theme.Popup.Surface" TargetType="Border">`

The floating ground a picker popup stands on.
It is the same white as a field, so the rows inside need no card of their own.
The list reads as one surface instead of a stack of tiles.

## `<Style x:Key="Theme.Popup.Title" TargetType="TextBlock">`

What the popup is, said once and quietly: the rows below are the content, not this line.

## `<Style x:Key="Theme.Popup.Notice" TargetType="TextBlock">`

The one line the popup says while it has no rows to show: searching, or nothing found.

## `<Style x:Key="Theme.Popup.Progress" TargetType="Border">`

The search running, as a hairline under the title rather than a box of its own.

## `<Style x:Key="Theme.Popup.ProgressBar" TargetType="Border">`

The sliding bar inside that hairline.
The transcriber and the downloader run the same search, so they share one bar rather than declaring two.

## `<Style x:Key="Theme.Popup.RowSurface" TargetType="Border">`

One row of a picker popup.
It carries no border and no fill of its own.
A row can offer more than one action, so every action is its own button.
The row itself takes no click.
Transcriber and downloader rows are the same row.

## `<Style x:Key="Theme.Popup.RowAction" TargetType="Button">`

The action that takes a row's offer.
One of these sits on every row, so it is quiet until it is pointed at.
It is text alone, tinted only on hover.

## `<Style.Resources>`

The app-wide implicit TextBlock style outranks the button's inherited Foreground.
So the label is bound back to the button and follows its enabled and hover states.

## `<Style x:Key="Theme.Popup.IconAction" TargetType="Button">`

A row's second action, carrying an icon instead of a word.
Same quiet-until-pointed-at treatment as the taking button beside it.

## `<Style x:Key="Theme.Pronunciation.Surface" TargetType="Border">`

The row a pronunciation is read and written in.
It carries no border and no fill, so the brackets stand alone against the page.
Its height matches the playback group beside it, so the two sit on one line.
Without a box the reading has to hold the eye on its own, so it is set larger than the buttons around it.

## `<Style x:Key="Theme.Pronunciation.Bracket" TargetType="TextBlock">`

The two brackets that hold the reading.
They are set in the muted ink, so they frame the reading without competing with it.

## `<Style x:Key="Theme.Pronunciation.Text" TargetType="TextBlock">`

The pronunciation as it reads.
It takes exactly the width its text needs, up to a ceiling that keeps a long reading from pushing the row wide.

## `<Style x:Key="Theme.Pronunciation.Measure" TargetType="TextBlock">`

An unseen twin of the field, carrying the same text in the same face.
The field is sized by this twin, so the box grows and shrinks with what is typed.
When nothing is typed the twin carries the placeholder instead, so the empty field is still wide enough to read it.

## `<Style x:Key="Theme.Pronunciation.Field" TargetType="TextBox">`

The same pronunciation with a caret in it.
It holds no floor width and fills the width its unseen twin measures.
It carries the read size and is pulled a pixel left, because a WPF text box keeps that pixel for the caret.
So a reading sits in the same place whether it is being read or being typed.

## `<Style x:Key="Theme.Playback.Action" TargetType="Button">`

The play button, drawn as a command inside the playback tray rather than as a control of its own.
It is the icon toggle's treatment without the held state, because playing is done the moment it is asked for.
Both views draw it, because a recording is played where it is heard and where it is chosen.

## `<Style x:Key="Theme.Volume.Rail" TargetType="RepeatButton">`

The two halves of the volume track, the taken one filled in the accent and the remaining one left clear.
They are repeat buttons because that is what a WPF track is built from, and a click on either walks the volume toward it.

## `<Style x:Key="Theme.Volume.Thumb" TargetType="Thumb">`

The grip the volume is carried by: a ring of the accent around the surface, filling as it is pointed at and solid while it is dragged.
It is a ring rather than a dot, so the track it sits on stays readable underneath it.

## `<Style x:Key="Theme.Volume.Slider" TargetType="Slider">`

The volume of a played recording, from silence to full over its own width.
It runs zero to one, which is the range a media player takes, so nothing between the grip and the sound rescales it.
A click anywhere on the track moves the grip there, because a volume is chosen by where it should be rather than nudged toward it.

## `<Style x:Key="Theme.Speech.Chip" TargetType="Border">`

One part of speech as it reads.
It matches the editor's marker chip apart from the removal button that chip carries.
The parts of speech take a row of their own, below the pronunciation rather than beside it.

## `<Style x:Key="Theme.Marker.Field" TargetType="Border">`

The pill a new part of speech is typed into, drawn as the chips beside it are drawn.
It carries their height and their radius, so the row reads as chips ending in an empty one.
The border is a hairline until pointed at or typed in, which is what tells the two apart.

## `<Style x:Key="Theme.Marker.Text" TargetType="TextBox" BasedOn="{StaticResource Theme.Input.Field}">`

The typing inside that pill, sized and coloured as a chip's name is.
What is typed and what is committed then look alike, so committing moves nothing.

## `<Style x:Key="Theme.Marker.Switch" TargetType="ToggleButton">`

The chevron that opens the presets, held inside the pill rather than beside it.
It fills with the accent while the menu stands open, so the pill says the menu is showing.

## `<Style x:Key="Theme.Marker.Menu" TargetType="Border">`

The presets menu, lifted off the form by the same shadow the command tray wears.
A menu that hovers over a form needs to read as above it and nothing more.
The heavier popup shadow belongs to windows that cover their page, which this one does not.

## `<Style x:Key="Theme.Language.Chip" TargetType="Border">`

The blue pill an entry's language is named on.
The reading view wears it and the editor's language toggle copies it.
Both are defined here, so the two views cannot drift apart.

## `<Style x:Key="Theme.Language.Toggle" TargetType="ToggleButton">`

The editor's language pill, drawn as the reading view's pill is drawn.
It stays a toggle, because a language is chosen here and only reported there.
The border is transparent until pointed at or opened, which is the only hint of the menu it holds.

## `<Style x:Key="Theme.Command.Group" TargetType="Border">`

The tray a panel's commands sit in, and the same tray the input header's pair sits in.
One surface holds them, so a row of actions reads as one thing rather than five.
The shadow stays when the tray is disabled, because a tray that drops it looks like a different control.

## `<Style x:Key="Theme.Command.IconToggle" TargetType="ToggleButton">`

A command inside the tray that carries an icon alone and holds a menu open.
It is tinted while its menu stands open, so the tray says which picker is showing.

## `<ControlTemplate x:Key="Theme.Command.Segment.Template" TargetType="ButtonBase">`

One command inside the tray, drawn for a button and for a mode radio alike.
The icon is the button's Tag, masked into a rectangle that takes its colour from Foreground.
A command with no Tag collapses the icon and keeps its word alone.
The label is a TextBlock the template owns, because the app-wide TextBlock style outranks an inherited colour.
State is set on the button rather than on the surface, so a style below can overrule it.

## `<Style x:Key="Theme.Command.Primary" TargetType="Button">`

The one command in a tray that commits, filled so the eye finds it without reading.
It drops to the quiet treatment when it cannot be pressed, because a filled slab reads as pressable.

## `<Style x:Key="Theme.Command.Mode" TargetType="RadioButton">`

The two halves of the reading and writing switch, which is a choice rather than two commands.
The chosen half is tinted, so the panel says which half it is on before it is asked.
A disabled switch keeps that tint, because the mode it stands on is still true.

## `<sys:Double x:Key="Theme.Card.TitleSize">`

The measurements a card is drawn to, held here rather than in either card dictionary.
A card must read the same whether it is being written or being read, and the two are drawn by dictionaries that never meet.
So the numbers live where both can reach them, and neither can drift from the other by an edit to one file.

## `<Style x:Key="Theme.Card.Shell" TargetType="Border">`

The card itself, its heading strip, its number and the box its fields stand in.
Both modes take these, so a card keeps its outline when the mode under it changes.

## `<Style x:Key="Theme.Card.Gutter" TargetType="ColumnDefinition">`

The strip kept clear down the right of every card body.
Writing a card needs handles beside each line, and reading it needs none.
Reserving the strip in both modes makes a line wrap identically whether or not the handles are there.

## `<Style x:Key="Theme.Card.Reach" TargetType="Grid">`

A row inside the body reaching back out into that strip.
The row's text keeps the body's width and its handles hang in the gutter beside it.

## `<Style x:Key="Theme.Card.Pellet" TargetType="Border">`

A translation, a situation and a tag as chips.
A translation is tinted blue, a situation tinted amber and a tag outlined.
A row of chips is therefore never read as the wrong kind.

## `<Style x:Key="Theme.Card.Situation" TargetType="Border">`

A situation chip in its own amber, on the reading and the writing side alike.
Amber is warm against the blue a translation takes and carries none of the meaning green or red would.

## `<Style x:Key="Theme.Input.Bare" TargetType="TextBox">`

A field with no frame of its own, for writing straight over what the reading side draws.
Its frame is drawn outward on hover and focus by "Theme.Input.Field.Bare", so the text never moves.

## `<Style x:Key="Theme.Card.Title" TargetType="TextBox">`

Each of a card's written fields at the size, weight and face the reading side gives that same field.
A writer therefore sees the card as it will be read, with handles added rather than a form put in its place.

## `<Style x:Key="Theme.Card.Handle" TargetType="Button">`

The small marks that add and drop a row, sized to sit in the gutter without pushing the line down.

## `<Style x:Key="Theme.Card.Switch" TargetType="ToggleButton">`

The mark that opens a frame on an Example row that carries none.
It sits in the gutter with the row's other handles, so opening a frame costs the line nothing until one is opened.

## `<Style x:Key="Theme.Card.Ghost" TargetType="TextBlock">`

A copy of what a field holds, drawn invisibly behind it so the field is exactly as wide as its text.
An inline field has to be measured this way; left to itself a drop-down takes the width of its widest offer.

## `<Style x:Key="Theme.Choice.Bare" TargetType="ComboBox">`

The frame fields of an Example: typed into like text, offering what the language has saved, and framed only under the pointer.
The frame is drawn outward, so a field being pointed at or written in never moves the sentence beside it.
Its frame is measured by "PFieldConverter", the same measure a written text field is framed by.
A marker and the sentence beside it are therefore framed at one height though they are written at different faces.
