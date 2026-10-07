# PEstablishment.xaml
Hash: `2769c9c9b6cd91e9`

## `<UserControl`

The status bar is a fixed 26 pixels and takes the navigation shade with a hairline above.
It reads as chrome under the house, like the roof above it, and never as content.
The left margin is narrower than the roof's, so the first button's label lines up with the brand.

## `<StackPanel x:Name="PCourier"`

The Joplin control at the far left, always present, so a push is one click from any panel.
It reads `[Connect] [Sync]` until a token is stored, then `Connected [Sync]`.
The courier's line follows after a slash, so a receipt sits beside the button that made it.

## `<Button x:Name="PCourierWarrant"`

Asks Joplin to accept Llyn, which the user confirms in Joplin itself.
It shows only while no token is stored.
The buttons take the segment style shrunk to the bar's height, so they read as quiet chrome.

## `<TextBlock x:Name="PCourierBadge"`

Stands in for the connect button once a token is stored.
It is muted, because it is a reading and never a call to act.

## `<Button x:Name="PCourierCommand"`

Sends every entry to Llyn's own Joplin notebook in one push.
Its tooltip says where the entries go and that edits made in Joplin are overwritten.

## `<TextBlock x:Name="PCourierSeparator"`

The slash before the receipt, collapsed while there is no line to show.

## `<TextBlock x:Name="PCourierReceipt"`

The courier's line, a waiting or sending note while a run goes, then the last push's receipt.
A connect clears it when done, since the badge or a failure notice shows the outcome.
It trims with an ellipsis, so a long list of failed headwords never pushes the bar's right side away.

## `<StackPanel x:Name="PEstablishmentPending"`

The unsaved line after the Joplin control, collapsed whole while nothing is unsaved.
The mark is a small dot in the pending shade, the colour the shell uses for work not yet settled.

## `<StackPanel Grid.Column="2"`

The entry count and the file size on the right, parted by a middle dot.
Both are muted, because they are a reading and never a call to act.
