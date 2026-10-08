# QVideo.cs
Hash: `587915ceeca724ef`

## `internal sealed class QVideo`

The driver for Video rows, in a card of the editor and in a situation of the repertoire.
It hears a row opened, a row dropped, and a file chosen from this machine.
The rows sit in a template, so the card list hands their buttons here rather than to the card.
Each click hands one raw value to one video gate, and the engine finds the card a row belongs to.

## `internal void QVideoIntroduce(CVideo video)`

Holds the video gate the row clicks reach, the editor's or the repertoire's.
Each owner builds its own driver and hands it its own gate.

## `internal void QVideoRowIntroduce(ItemsControl row, Button fresh)`

Binds a situation's video list to the rows this driver holds, and its add button to the add gate.
`QScenario` calls it once the playwright's video gate is held.
The rows are drawn through `QVideoApply`, which hooks the location and span observers only after the fill.

## `internal void QVideoRowRefine(IReadOnlyList<CVideoDraft> rows)`

Redraws the video rows from the held Situation the same way as the pictures, location and span apart.
A new row holds the engine's location and span, and nothing typed.
`QVignette` builds its read rows with the same constructor.

## `private void QVideoFreshObserve(object sender, RoutedEventArgs e)`

Hands the situation's add button press to the video add gate.
A Situation has no card, so the add names a null card, and the draft routes it.

## `internal void QVideoAddObserve(object sender, RoutedEventArgs e)`

Hands the id of the card the Extra row belongs to to the add gate, which appends the row.

## `private void QVideoRemoveObserve(object sender, RoutedEventArgs e)`

Hands the id of the clicked row to the remove gate.

## `private void QVideoOpenObserve(object sender, RoutedEventArgs e)`

Asks for a video file, owned by the window of the clicked button.
The chosen path goes raw to the file gate, which sends it as the row's location at once.
A cancelled dialog hands null, and the gate does nothing.
A typed location or span goes the same way through the box's own observer, only deferred.
As with a picture, the field takes a web address just as well.
Nothing is copied into the workspace, because a Video is never kept.

## `private static string? QVideoFileConsult(Window owner)`

Asks the user for a video file on this machine and answers its path, or null when they chose none.

## `internal void QVideoApply(FrameworkElement container, object item, string? name)`

The video row fill, reached from the card templates' list forwarders and the situation's video list.
It draws the row through the shared fill and subscribes the browse and remove observers.
It then hooks the location and span boxes to their own observers, after the fill.
So a write from the fill echoes only where the handlers were already hooked, as in the editor.

## `private void QVideoLocationObserve(object sender, TextChangedEventArgs e)`

Hands a typed location to the location gate.
The location box is hooked to this observer alone, so no box name decides the gate.
Only a box with the keyboard in it reports, since a write from the draft echoes through the same event.

## `private void QVideoSpanObserve(object sender, TextChangedEventArgs e)`

Hands a typed span to the span gate, heard the same way as the location.
