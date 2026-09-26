# PArticulation.xaml

## `<UserControl.Resources>`

The glyph chip carries no hover or pressed trigger.
`PLookSound` lights it, once `PArticulation` registers these styles.

One glyph is one borderless chip, and one header is one quiet label.
A chart heading carries the helper ink.
That is the one place the helper hue is written rather than laid down.
A chart of a hundred bordered boxes would read as a grid of controls rather than a table of sounds.
So the chip carries no outline until the pointer is over it.
That is how the search dropper and the catalog rows behave.
The glyph is not focusable, so pressing it leaves the caret where the reader put it.
That is what lets the character land at the caret rather than at the end.

## `<Border Padding="6,14,0,8" Style="{StaticResource Theme.Panel.Helper}">`

The aid is shown and hidden whole by the panel that hosts it.
It holds no fold control of its own, because folding it is the panel's layout decision.
The band takes the helper ground rather than the panel surface.
An aid is therefore never mistaken for the work it assists.
It runs edge to edge like the seams.
The left inset puts its charts where the ordering bar and the catalog rows begin.
Every side of that space is padding rather than margin.
A margin under a coloured band would leave a white strip above the seam.

## `<Grid x:Name="PArticulationRack" HorizontalAlignment="Left">`

The two charts stand side by side while the window is wide enough for both.
A rack of two rows and two columns holds them.
The code moves the consonant chart between the two places.
The consonant table is the one that must stay whole.
Its place and manner grid is what makes it readable.
So it takes a row of its own rather than losing its right edge to the vowel chart beside it.

## `<Style x:Key="Articulation.Chart" ...>`

Each chart is a card of the shared card shape.
The two tables therefore read as two objects rather than one long sheet.
The card hugs its table on the left.
That keeps the narrow vowel chart from stretching to the width of the consonant chart.
The two cards meet at the top.
The vowel chart is one row shorter, and stretching it would leave an empty band under its table.
A card keeps the working surface and takes the helper edge.
A chart therefore reads as a white sheet laid on the helper ground.

The band holds the slim scroll lane rather than the full gutter.
The space under the charts therefore matches the space above them.
The lane still scrolls sideways for the window too narrow for the consonant table alone.

## `<Grid x:Name="PVowel" HorizontalAlignment="Left" />`

The table is empty here and filled in code.
Its shape is a chart, not a layout, so it is written where the chart is written.
