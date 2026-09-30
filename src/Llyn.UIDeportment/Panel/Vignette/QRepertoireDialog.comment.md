# QRepertoireDialog.cs

## `internal sealed partial class QRepertoire`

What the picture and video rows of the situation editor hear and paint.
That is a row opened, a row dropped, a file chosen, and a location or span written.
Each heard action hands its raw value to one gate of `CRepertoireImage` or `CRepertoireVideo`.
A Situation has no card, so each add names card zero, and the draft routes it.
The panel keeps no count and no map of which row is still waiting.

## `private void QScenarioImageRefine(IReadOnlyList<CImageDraft> rows)`

Redraws the picture rows from the held Situation, adding, dropping, and moving rows to match.
What was waiting is written before the read, so a redraw never lands over a newer keystroke.

## `private void QScenarioVideoRefine(IReadOnlyList<CVideoDraft> rows)`

Redraws the video rows the same way, location and span apart.

## `private PImage QScenarioImageCreate(CImageDraft draft)`

A picture row holding the engine's location, and nothing typed.
The vignette builds its read rows here too.

## `private PVideo QScenarioVideoCreate(CVideoDraft draft)`

A video row holding the engine's location and span, and nothing typed.
The vignette builds its read rows here too.

## `private void QImageLocationObserve(object sender, TextChangedEventArgs e)`

Hands a location typed into a picture row to the location gate.
Only a field the keyboard is in has been typed into.

## `private void QVideoLocationObserve(object sender, TextChangedEventArgs e)`

Hands a location typed into a video row to the location gate.

## `private void QVideoSpanObserve(object sender, TextChangedEventArgs e)`

Hands a span typed into a video row to the span gate.
The span has its own handler, so no field name decides which gate hears it.

## `private void QImageAddObserve(object sender, RoutedEventArgs e)`

Hands the add button's press to the add gate, which appends the row below.

## `private void QVideoAddObserve(object sender, RoutedEventArgs e)`

Hands the add button's press to the video add gate.

## `public void PImageRemoveHandle(object sender, RoutedEventArgs e)`

Hands the id of the row the click came from to the remove gate.
It keeps the name `PImageHost` gives it.

## `public void PVideoRemoveHandle(object sender, RoutedEventArgs e)`

Hands the id of the row the click came from to the video remove gate.
It keeps the name `PVideoHost` gives it.

## `public void PImageOpenHandle(object sender, RoutedEventArgs e)`

Asks for a picture file, owned by the window of the clicked button.
The chosen path goes raw to the file gate, which sends it at once.
A cancelled dialog hands nothing.

## `public void PVideoOpenHandle(object sender, RoutedEventArgs e)`

Asks for a video file and hands the chosen path to the video file gate.
A cancelled dialog hands nothing.

## `private readonly PImageTemplate _qImageTemplate`

The picture row dictionary the situation editor merged, whose forwarders its row fill subscribes.

## `private readonly PVideoTemplate _qVideoTemplate`

The video row dictionary the situation editor merged, whose forwarders its row fill subscribes.

## `private void QImageItemRefine(FrameworkElement container, object item, string? name)`

The situation editor's picture row fill.
It draws the row through the shared fill with the location's text change unsubscribed.
So a redraw from the draft never reaches the gate as typing.
It also subscribes the browse and remove handlers.

## `private void QVideoItemRefine(FrameworkElement container, object item, string? name)`

The situation editor's video row fill.
It draws the row with the location's and the span's text changes unsubscribed, as the picture fill does.
It also subscribes the browse and remove handlers.
