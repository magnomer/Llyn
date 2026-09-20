# PDisplayIncoming.cs

## `public partial class PDisplay`

The incoming rows of the reading view: the entries whose cards point at the shown one.
The rows take their shape from [PDisplayUsage.xaml](PDisplayUsage.comment.md).

## `private void PDisplayIncomingShow(long id)`

Reads the usages that point at `id` and lists one row per usage.
Each row names the referring Meaning or Collocation in the reader's words, since the engine hands back a kind.
Rows sharing a headword are numbered afterwards, so the reader can tell them apart.
A read that fails leaves the section collapsed, because an empty relationship does not occupy the page.

## `private void PDisplayIncomingHandle(object sender, RoutedEventArgs e)`

Opens the entry that carries the clicked row, because that is where such a link is edited.
