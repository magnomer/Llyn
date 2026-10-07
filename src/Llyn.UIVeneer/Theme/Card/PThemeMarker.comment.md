# PThemeMarker.xaml
Hash: `3c84c1074a2d6ff2`

## `<Style x:Key="Theme.Marker.Surface" TargetType="Border">`

The editor's part-of-speech chip, at the height, margin and radius of `Theme.Speech.Chip`.
So a part of speech keeps its box between reading and writing.
Its edge is painted in its fill, so the accent edge on hover widens nothing.

## `<Style x:Key="Theme.Marker.Field" TargetType="Border">`

The pill a new part of speech is typed into, drawn as the chips beside it are drawn.
It carries their height and their radius, so the row reads as chips ending in an empty one.
The border is a hairline until pointed at or typed in, which is what tells the two apart.
Deportment switches the border and the ground through `QLook` rows.

## `<Style x:Key="Theme.Marker.Text" TargetType="TextBox" BasedOn="{StaticResource Theme.Input.Field}">`

The typing inside that pill, sized and coloured as a chip's name is.
What is typed and what is committed then look alike, so committing moves nothing.

## `<Style x:Key="Theme.Marker.Switch" TargetType="ToggleButton">`

The chevron that opens the presets, held inside the pill rather than beside it.
It also heads a folded sound part, where it opens and shuts the part's body.
The chevron turns over while the switch stands open, so it points the way the next click goes.
The ground behind it warms to the soft accent, never the accent itself.
The mark carries its own blue, so a full accent ground would swallow it whole.
Deportment turns the chevron and warms the ground through `QLook` rows on `PSurface` and `PSurfaceMark`.

## `<Style x:Key="Theme.Marker.Menu" TargetType="Border">`

The presets menu, lifted off the form by the same shadow the command tray wears.
A menu that hovers over a form needs to read as above it and nothing more.
The heavier popup shadow belongs to windows that cover their page, which this one does not.

## `<Style x:Key="Theme.Unit.Dropper" TargetType="ToggleButton">`

The unit chooser, a chip outlined rather than filled.
The outline tells a choice of one apart from the filled chips of the parts of speech.

## `<Style x:Key="Theme.Unit.Label" TargetType="TextBlock" BasedOn="{StaticResource Theme.Speech.Name}" />`

The unit's name inside the chooser, written as a chip's name is.
