# QRepertoireDialog.cs

## `internal sealed partial class QRepertoire`

What the picture and video rows of the situation editor hear and paint.
The same `QImage` and `QVideo` drivers as the card editor's hear a row's browse and remove buttons.
Here they hold the repertoire's image and video gates instead of the editor's.
This part hears a location or span written and the add buttons outside any row.
A Situation has no card, so each add names card zero, and the draft routes it.
The panel keeps no count and no map of which row is still waiting.

## `private void QScenarioImageRefine(IReadOnlyList<CImageDraft> rows)`

Redraws the picture rows from the held Situation, adding, dropping, and moving rows to match.
What was waiting is written before the read, so a redraw never lands over a newer keystroke.

## `private void QScenarioVideoRefine(IReadOnlyList<CVideoDraft> rows)`

Redraws the video rows the same way, location and span apart.

## `private QImageItem QScenarioImageCreate(CImageDraft draft)`

A picture row holding the engine's location, and nothing typed.
The vignette builds its read rows here too.

## `private QVideoItem QScenarioVideoCreate(CVideoDraft draft)`

A video row holding the engine's location and span, and nothing typed.
The vignette builds its read rows here too.

## `private void QImageLocationObserve(object sender, TextChangedEventArgs e)`

Hands a location typed into a picture row to the picture driver's location observer.
Only a field the keyboard is in has been typed into.

## `private void QImageAddObserve(object sender, RoutedEventArgs e)`

Hands the add button's press to the add gate, which appends the row below.

## `private void QVideoAddObserve(object sender, RoutedEventArgs e)`

Hands the add button's press to the video add gate.

## `private void QImageItemRefine(FrameworkElement container, object item, string? name)`

The situation editor's picture row fill.
It draws the row through the picture driver's fill with the location's text change unsubscribed.
So a redraw from the draft never reaches the gate as typing.
The driver's fill subscribes the browse and remove observers.

## `private void QVideoItemRefine(FrameworkElement container, object item, string? name)`

The situation editor's video row fill.
It unhooks the video driver's location and span observers, then draws the row through the driver's fill.
The driver's fill hooks them again after drawing, so a redraw from the draft never reaches the gate as typing.
The driver's fill also subscribes the browse and remove observers.
