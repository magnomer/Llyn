# PTenor.xaml.cs

## `public partial class PTenor : UserControl`

The tenor panel: the workspace browsed by the Registers its cards carry.
It is the taxonomy panel's shape read through a different question, so it holds the same three columns.
It owns a reader and an editor over one Entry, and answers the engine rather than its own visibility.

## `internal void PTenorAttach(PWindow host, LEngine engine)`

Binds the panel to the window and the engine, and starts listening for what the engine announces.

## `internal void PTenorReset()`

Empties the panel and rebuilds the catalog, for a workspace that has just moved.

## `internal bool PTenorDraftFinish(bool store)`

Stores or discards a standing draft on the way out of the application.

## `internal bool PTenorChangeCheck()`

Whether the editor is the shown side and is holding a change, so leaving would lose work.

## `internal void PTenorClose()`

Stops listening and closes the reader and the editor.
