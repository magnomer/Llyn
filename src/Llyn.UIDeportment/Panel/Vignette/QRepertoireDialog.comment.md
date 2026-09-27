# QRepertoireDialog.cs

## `internal sealed partial class QRepertoire`

What the picture and video rows of the situation editor ask the panel for.
That is a row opened, a row dropped, a file chosen, and a location or span written.
The rows are the card's own rows, so the panel answers as the entry editor answers.
A Situation has no card, so each add and removal names card zero, routed by the draft.
Every request is built by the desk's media gate, and the tenure keys what waits by `LRequestKey`.
So the panel keeps no map of its own of which row is still waiting.

## `private void QScenarioImageShow(IReadOnlyList<CImageDraft> rows)`

Redraws the picture rows from the held Situation, adding, dropping, and moving rows to match.
What was waiting is written before the read, so a redraw never lands over a newer keystroke.

## `private void QScenarioVideoShow(IReadOnlyList<CVideoDraft> rows)`

Redraws the video rows the same way, location and span apart.

## `private PImage QScenarioImageCreate(CImageDraft draft)`

A picture row holding the engine's location, and nothing typed.
The vignette builds its read rows here too.

## `private PVideo QScenarioVideoCreate(CVideoDraft draft)`

A video row holding the engine's location and span, and nothing typed.
The vignette builds its read rows here too.

## `private void QImageLocationHandle(object sender, TextChangedEventArgs e)`

Turns a location typed into a picture row into a deferred location set for that row.
Only a field the keyboard is in has been typed into.

## `private void QVideoLocationHandle(object sender, TextChangedEventArgs e)`

Turns a location typed into a video row into a deferred location set for that row.

## `private void QVideoSpanHandle(object sender, TextChangedEventArgs e)`

Turns a span typed into a video row into a deferred span set for that row.
The span has its own handler, so no field name decides which request goes.

## `private void QImageAddHandle(object sender, RoutedEventArgs e)`

Opens a picture row at the end of the list, for the add button under the rows.

## `private void QVideoAddHandle(object sender, RoutedEventArgs e)`

Opens a video row at the end of the list.

## `public void PImageRemoveHandle(object sender, RoutedEventArgs e)`

Drops the row the click came from.

## `public void PVideoRemoveHandle(object sender, RoutedEventArgs e)`

Drops the row the click came from.

## `public void PImageOpenHandle(object sender, RoutedEventArgs e)`

Chooses a picture for the row the click came from, owned by the window of the clicked button.

## `private void QScenarioImageOpen(long image, Window owner)`

Asks for a picture file and sends its path as the row's location at once.
A cancelled dialog sends nothing.

## `public void PVideoOpenHandle(object sender, RoutedEventArgs e)`

Chooses a video for the row the click came from, owned by the window of the clicked button.

## `private void QScenarioVideoOpen(long video, Window owner)`

Asks for a video file and sends its path as the row's location at once.
A cancelled dialog sends nothing.

## `private readonly PImageTemplate _qImageTemplate`

The picture row dictionary the situation editor merged, whose forwarders its row fill subscribes.

## `private readonly PVideoTemplate _qVideoTemplate`

The video row dictionary the situation editor merged, whose forwarders its row fill subscribes.

## `private void QImageApply(FrameworkElement container, object item, string? name)`

The situation editor's picture row fill.
It draws the row through the shared fill with the location's text change unsubscribed.
So a redraw from the draft never reaches the gate as typing.
It also subscribes the browse and remove forwarders.

## `private void QVideoApply(FrameworkElement container, object item, string? name)`

The situation editor's video row fill.
It draws the row with the location's and the span's text changes unsubscribed, as the picture fill does.
It also subscribes the browse and remove forwarders.
