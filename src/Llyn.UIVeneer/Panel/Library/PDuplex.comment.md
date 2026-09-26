# PDuplex.xaml

## `<Rectangle Grid.Row="0" ... Style="{StaticResource Theme.Panel.SeamRow}" />`

The seam that parts the two search bars from the pair of entries below them.
It is bled past the panel margin so it meets the navigation's own edge.
Its row is fixed to the bar's height and gap, since the bars belong to the wings.
The column seam sits in the middle of the gutter and parts one side from the other.
Neither seam encloses anything, and the two entries read as two pages side by side.

## `<local:PWing x:Name="PLeftWing" ... />`

Each side is one wing: bar, matches and display.
The two wings are peers and neither drives the other.
A wing spans both rows, so its bar lines up with the seam the panel draws.
Only the gutter margin differs between the two.
The wing stays a Deportment control, loaded here by its tag and driven by its own class.
