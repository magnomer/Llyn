# PArticulation.xaml

## `<UserControl.Resources>`

One glyph is one small square button, and one header is one quiet label.
The glyph is not focusable, so pressing it leaves the caret where the reader put it.
That is what lets the character land at the caret rather than at the end.

## `<Border Padding="18,14" Style="{StaticResource Theme.Input.Card}">`

The aid is shown and hidden whole by the panel that hosts it.
It holds no fold control of its own, because folding it is the panel's layout decision.

The two tables scroll sideways together.
The consonant table is wider than the panel on a narrow window.
Wrapping it would break the place-and-manner grid that makes it readable.

## `<Grid x:Name="PVowel" HorizontalAlignment="Left" />`

The table is empty here and filled in code.
Its shape is a chart, not a layout, so it is written where the chart is written.
