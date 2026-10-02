# PDuplex.xaml
Hash: `53f7bb0809856f2e`

## `<Rectangle Grid.Row="0" ... Style="{StaticResource Theme.Panel.SeamRow}" />`

The seam that parts the two search bars from the pair of entries below them.
It is bled past the panel margin so it meets the navigation's own edge.
Its row is fixed to the bar's height and gap, since the bars belong to the wings.
The column seam sits in the middle of the gutter and parts one side from the other.
Neither seam encloses anything, and the two entries read as two pages side by side.

## `<veneer:PWing x:Name="PLeftWing" ... />`

Each side is one wing: bar, matches and display.
The two wings are peers and neither drives the other.
A wing spans both rows, so its bar lines up with the seam the panel draws.
Only the gutter margin differs between the two.
The wing is a veneer page placed twice, and the duplex driver gives each place its own `QWing`.
