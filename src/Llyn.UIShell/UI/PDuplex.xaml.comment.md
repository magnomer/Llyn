# PDuplex.xaml.cs

## `public partial class PDuplex : UserControl`

The duplex panel as a control: what it is made of, and when it starts and stops.
Browsing itself lives in the file beside this one.
That is the two queries, their matches, and the Entry each side stands on.

## `internal void PDuplexAttach(PWindow host, LEngine engine)`

Puts the panel to work on `engine`, the workspace the window opened.
Nothing is read yet.
Each side fills itself once a query is typed into it.

## `internal void PDuplexReset()`

Puts the panel back on the workspace open now.
Both queries are emptied and both sides stand on nothing.
A different workspace has its own database.
So the Entries the panel was comparing came from one that is no longer open.

## `internal void PDuplexClose()`

Releases what both sides hold open.
The display on each side owns a media player.
