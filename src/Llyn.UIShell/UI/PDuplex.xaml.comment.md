# PDuplex.xaml.cs

## `<Rectangle ... Style="{StaticResource Theme.Panel.SeamRow}" />`

The seams that part the two search fields from the pair of entries below them, and one side from the other.
The candidate list each side shows takes a seam of its own rather than a box, and appears and goes with the list it closes.
Neither side is a card here, so nothing is enclosed and the two entries read as two pages side by side.

## `public partial class PDuplex : UserControl`

The duplex panel as a control: what it is made of, and when it starts and stops.
Browsing itself lives in the file beside this one.
That is the two queries, their matches, and the Entry each side stands on.

## `internal void PDuplexAttach(PWindow host, LEngine engine)`

Puts the panel to work on `engine`, the workspace the window opened.
It binds both match lists and reads nothing yet.
Each side fills itself once a query is typed into it.
Neither side subscribes: the display on each is its own subscriber, and it is the display that must stay current.

## `internal void PDuplexRestore(LWorkspaceState state)`

Puts the panel back on the workspace open now, standing on the Entries `state` names.
Both queries are emptied and both sides are cleared first.
A different workspace has its own database.
So the Entries the panel was comparing came from a workspace no longer open.
The ones it stands on come from the workspace that is.
A side the state names nothing for stays empty.

## `internal void PDuplexClose()`

Releases what both sides hold open.
The display on each side owns a media player.
