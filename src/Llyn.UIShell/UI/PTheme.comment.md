# PTheme.xaml

## `<Style TargetType="ScrollBar">`

Every rail in the app is one design at one size, upright or sideways.
A page, a catalog, a popup and a text field all show the same thread.
The two control templates are registered by PIndicator.
WPF requires a PART_Track name.
That name must stay out of the audited XAML naming surface.

## `<Style TargetType="ScrollViewer">`

The lane template is the default for every scroll viewer, not an opt-in.
A viewer the app never names, inside a dropdown or a text field, would otherwise keep the platform one.
That is how a rail nobody styled used to appear over the content at a size nobody chose.
PIndicator registers the template.

## `<Style x:Key="Theme.Scroll.Gutter" TargetType="ScrollViewer">`

The name a page uses to ask for the lane it already has.
It says in the markup what the default would give it anyway.

## `<Style x:Key="Theme.Scroll.Lane" TargetType="ScrollViewer">`

A band that scrolls sideways and never up.
It holds the same rail as everything else, turned on its side.

## `<Setter Property="Template" Value="{DynamicResource Theme.Input.Field.Template}" />`

The TextBox content host must be named "PART_ContentHost" (a WPF template contract).
That name is authored in code (PField) so it stays out of the audited XAML surface.

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

## `<Style x:Key="Theme.Popup.IconAction" TargetType="Button">`

A row's second action, carrying an icon instead of a word.
Same quiet-until-pointed-at treatment as the taking button beside it.

## `<Style x:Key="Theme.Pronunciation.Surface" TargetType="Border">`

The row a pronunciation is read and written in.
It carries no border and no fill, so the brackets stand alone against the page.
Its height matches the playback group beside it, so the two sit on one line.
Without a box the reading has to hold the eye on its own.
It is set larger than the buttons around it.

## `<Style x:Key="Theme.Pronunciation.Bracket" TargetType="TextBlock">`

The two brackets that hold the reading.
They are set in the muted ink, so they frame the reading without competing with it.

## `<Style x:Key="Theme.Pronunciation.Text" TargetType="TextBlock">`

The pronunciation as it reads.
It takes exactly the width its text needs.
A ceiling keeps a long reading from pushing the row wide.

## `<Style x:Key="Theme.Pronunciation.Measure" TargetType="TextBlock">`

An unseen twin of the field, carrying the same text in the same face.
The field is sized by this twin, so the box grows and shrinks with what is typed.
When nothing is typed the twin carries the placeholder instead.
The empty field is still wide enough to read it.

## `<Style x:Key="Theme.Pronunciation.Field" TargetType="TextBox">`

The same pronunciation with a caret in it.
It holds no floor width and fills the width its unseen twin measures.
It carries the read size and is pulled a pixel left.
A WPF text box keeps that pixel for the caret.
So a reading sits in the same place whether it is being read or being typed.

## `<Style x:Key="Theme.Playback.Action" TargetType="Button">`

The play button, drawn as a command inside the playback tray rather than as a control of its own.
It is the icon toggle's treatment without the held state, because playing is done the moment it is asked for.
Both views draw it, because a recording is played where it is heard and where it is chosen.

## `<Style x:Key="Theme.Volume.Rail" TargetType="RepeatButton">`

The two halves of the volume track, the taken one filled in the accent and the remaining one left clear.
They are repeat buttons because that is what a WPF track is built from.
A click on either walks the volume toward it.

## `<Style x:Key="Theme.Volume.Thumb" TargetType="Thumb">`

The grip the volume is carried by is a ring of the accent around the surface.
It fills as it is pointed at and is solid while it is dragged.
It is a ring rather than a dot, so the track it sits on stays readable underneath it.

## `<Style x:Key="Theme.Volume.Slider" TargetType="Slider">`

The volume of a played recording, from silence to full over its own width.
It runs zero to one, the range a media player takes.
Nothing between the grip and the sound rescales it.
A click anywhere on the track moves the grip there.
A volume is chosen by where it should be rather than nudged toward it.

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

## `<Style x:Key="Theme.Favorite.Mark" TargetType="ToggleButton">`

The star carries the mark alone, with no surface or border behind it.
A framed button would read as a control among the headword's own text.
Hover and press dim it instead, so the button still answers the pointer.

## `<Style x:Key="Theme.Panel.Surface" TargetType="Border">`

The region a panel gives a whole screen of its own, drawn on nothing.
It carries no outline, because a region is not an object and an outline at one pixel could not say that it was.
The outline it once carried was the only thing marking the region, which is why the region read as unfinished.
What separates one region from the next is a seam, not a box drawn round each of them.

## `<Style x:Key="Theme.Panel.Helper" TargetType="Border">`

The region a helper stands on, the same region shape carried on the helper ground instead of the panel's own.
A helper is an aid to the work rather than the work, and a reader must be able to see that before reading a word of it.
The ground is one step cooler and deeper than the canvas, which is enough to read as another kind of region.
It stays inside the program's own blue-grey, because a helper is a quieter thing than a choice or a verdict and must not out-colour either.

## `<Style x:Key="Theme.Panel.SeamRow" TargetType="Rectangle">`

The hairline laid across a panel where its controls end and its contents begin.
It is drawn at the foot of the row it belongs to and bled past the panel's own margin, so it runs the width of the window.
A seam marks a division between two regions, never an enclosure around one.

## `<Style x:Key="Theme.Panel.SeamColumn" TargetType="Rectangle">`

The hairline laid down the gutter between two columns of a panel.
It is set against the leading edge of the right column and pulled back into the middle of the gutter.
It is bled past the panel's foot so it reaches the bottom of the window, as the row seam reaches both sides.

## `<Style x:Key="Theme.Command.Group" TargetType="Border">`

The tray a panel's commands sit in, and the same tray the input header's pair sits in.
It has no ground, no edge and no shadow of its own, so the commands read as commands rather than as a boxed group.
Each command keeps its own hover and press, which is where a button says it is a button.
A disabled tray is dimmed whole, there being no edge left to pale.

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
A card must read the same whether it is being written or being read.
The two are drawn by dictionaries that never meet.
So the numbers live where both can reach them.
Neither can drift from the other by an edit to one file.

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
A writer therefore sees the card as it will be read.
Handles are added rather than a form put in its place.

## `<Style x:Key="Theme.Card.Ordinal" TargetType="TextBox">`

The number in a card's badge, written into rather than only read.
It stands read-only and untouched by the pointer until a double click opens it.
The badge above it therefore keeps taking the drag that moves the card.
The badge takes an accent ring while it is open, because a number being written must read as one.

## `<Style x:Key="Theme.Card.Handle" TargetType="Button">`

The small marks that add and drop a row, sized to sit in the gutter without pushing the line down.

## `<Style x:Key="Theme.Card.Switch" TargetType="ToggleButton">`

The mark that opens a frame on an Example row that carries none.
It carries a glyph rather than a drawing and holds no size of its own.
It stands on the line's baseline as the text beside it does.
A drawn mark of a fixed size sits where the row puts it.
That is never quite where the eye reads the line.

## `<Style x:Key="Theme.Card.Ghost" TargetType="TextBlock">`

A copy of what a field holds, drawn invisibly behind it.
The field is then exactly as wide as its text.
An inline field has to be measured this way.
Left to itself a drop-down takes the width of its widest offer.

## `<Style x:Key="Theme.Card.Dot" TargetType="TextBlock">`

The mark an example line opens with, drawn in the interface face it shares a baseline with.
It is not hit tested, because it is punctuation and not a place to write.

## `<Style x:Key="Theme.Card.Frame" TargetType="TextBlock">`

The parentheses a frame is written inside, set bold in the interface face and the accent colour.
No room is kept for the ring a written field is framed by, because the reading view keeps none either.
A frame the writing view widened would set the sentence further along than the card reads it.
The example keeps the reading face and its own weight.
The frame is never read as part of the sentence.
The invisible copies behind the written fields carry the same weight.
A bold field would otherwise outgrow the room measured for it.

## `<Style x:Key="Theme.Choice.Bare" TargetType="ComboBox">`

The frame fields of an Example are typed into like text.
They offer what the language has saved and are framed only under the pointer.
The frame is drawn outward, so a field being pointed at or written in never moves the sentence beside it.
The frame is closed onto the text it rings, clearing it by a hairline and no more.
A marker and a role are parted by one space, so a frame drawn outward crosses the word beside it.

## `<Style x:Key="Theme.Search.Bar" TargetType="Border">`

The ground every panel's ordering control and search field stand on together.
The outline is clear at rest and appears under the pointer or on keyboard focus, so an edge marks what the control is doing rather than that it exists.
It is held at one pixel throughout, so a bar that gains its edge does not shift the row beneath it.
The bar is the placement target of the ordering dropdown, so the dropdown falls from the whole control.

## `<Style x:Key="Theme.Search.Dropper" TargetType="ToggleButton">`

The ordering button as it stands inside the bar, with no frame and no ground of its own.
It takes a soft ground only under the pointer or while its dropdown is open.
It is unfocusable, so tabbing into the bar reaches the field the user came to type in.

## `<Style x:Key="Theme.Search.Field" TargetType="TextBox">`

The search field inside the bar, on one line and without a frame.
It takes the plain template, which draws the placeholder but no frame of its own.
The bar around it already frames the dropper, the divider and the field as one control.

## `<Style x:Key="Theme.Search.Helper" TargetType="ToggleButton">`

A folding control standing next to the bar rather than inside it, for a panel the search bar does not own.
It borrows the bar's height and corner, so the two shapes read as one row of controls over the catalog.
It carries a marked label like the command row does, and the mark is left out when no icon is named.
An open panel is shown by the soft accent ground the mode buttons use, because the fold is a state and not an action.

## `<Style x:Key="Theme.Catalog.Row" TargetType="Button">`

One row of a panel's catalog, drawn on nothing until it is pointed at.
The catalogs carry no card of their own, so a bordered row would stack a frame inside a frame.
A row shows its subject over a quieter line of what tells it apart.
The row the panel stands on is painted by the panel, which tags it as chosen.
The tag raises a straight accent rail inset along the leading side.
A left border would bend around the rounded corners and read as a curved tip.

## `<Style x:Key="Theme.Catalog.Pellet" TargetType="Border">`

The count at the far end of a catalog row, held in a quiet capsule.
It is muted rather than accented, because the accent is what marks the chosen row.

## `<Style x:Key="Theme.Catalog.Empty" TargetType="TextBlock">`

The line that says a catalog holds nothing, lying over the rows that are not there.
It never takes the pointer, so a click through it still reaches the catalog.

## Catalog frame and scrolling styles

Theme.Catalog.HeaderMargin brings each catalog's sort and search row to the same horizontal edges as its list rows.
It offsets the panel's 34-pixel inset, leaving the shared 6-pixel outer gutter, and ends where the row surface ends before the scroll rail.
Theme.Catalog.Frame offsets the 34-pixel panel margin to leave a 6-pixel left gutter.
Theme.Catalog.Middle offsets the seam 16 pixels before an interior column to leave the same 6-pixel gutter.
Both end at the next seam, where Theme.Catalog.Scroll opens its lane once the rows outrun the panel.
Library, Sound, Tags (including matching entries), Situations, Examples, Sources and Favorites share these styles.
The document and editor scroll viewers hold the same lane at the same width.
