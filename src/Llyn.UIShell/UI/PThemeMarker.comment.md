# PThemeMarker.xaml

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
