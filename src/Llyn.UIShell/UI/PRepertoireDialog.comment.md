# PRepertoireDialog.cs

## `public partial class PRepertoire`

What the picture and video rows of the situation editor ask the panel for.
That is a row opened, a row dropped, a file chosen, and a location or span written.
The rows are the card's own rows, so the panel answers as the entry editor answers.
A Situation has no card, so every request names card zero and the engine routes it by the draft.
Rows are refreshed the way a card's are: one that already reads what the engine holds is left alone.

## `private static string PScenarioRequestFormat(string kind, long rowId, string field)`

The key one row's field waits under, so a second edit replaces the first rather than queueing behind it.

## `private void PScenarioImageShow(IReadOnlyList<LImageDraft> rows)`

Redraws the picture rows from the held Situation, adding, dropping, and moving rows to match.
A row whose location request is still waiting keeps what was typed.

## `private void PScenarioVideoShow(IReadOnlyList<LVideoDraft> rows)`

Redraws the video rows the same way, location and span apart.

## `private PImage PScenarioImageCreate(LImageDraft draft)`

A picture row that reports its location changes to the panel.

## `private PVideo PScenarioVideoCreate(LVideoDraft draft)`

A video row that reports its location and span changes to the panel.

## `private void PScenarioImageChange(object? sender, PropertyChangedEventArgs arguments)`

Turns a typed location into a deferred request for that row, as `PImageAttach` does for a card.

## `private void PScenarioVideoChange(object? sender, PropertyChangedEventArgs arguments)`

Turns a typed location or span into a deferred request for that row, as `PVideoAttach` does for a card.

## `private void PImageAddHandle(object sender, RoutedEventArgs e)`

Opens a picture row at the end of the list, for the add button under the rows.

## `private void PVideoAddHandle(object sender, RoutedEventArgs e)`

Opens a video row at the end of the list.

## `public void PImageRemoveHandle(object sender, RoutedEventArgs e)`

Drops the row the click came from.

## `public void PVideoRemoveHandle(object sender, RoutedEventArgs e)`

Drops the row the click came from.

## `public void PImageOpenHandle(object sender, RoutedEventArgs e)`

Chooses a picture file and writes its path into the row, which then reports the change like typing.

## `public void PVideoOpenHandle(object sender, RoutedEventArgs e)`

Chooses a video file and writes its path into the row.

## `private bool PScenarioRequestCheck(string key)`

Whether a request for the row's field is still waiting, in which case a redraw must not overwrite it.

## `private void PScenarioRequestDefer(string key, LRequest request)`

Holds the request under its key and restarts the same wait typing restarts.
So a location written while the description is being typed reaches the engine in the same write.

## `private void PScenarioRequestSend(LRequest request)`

Sends a request at once, for adding and dropping rows.
Whatever is waiting is written first, so the engine sees the rows in the order they were changed.

## `private void PScenarioRequestPersist()`

Writes every waiting request, forgetting each only once the engine took it.
A request that fails stays in the map for the caller to clear.
Nothing is lost before it is written.
The draft save calls this before the body, so the two never cross.
