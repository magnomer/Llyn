# PRepertoireDialog.cs

## `public partial class PRepertoire`

What the picture and video rows of the situation editor ask the panel for.
That is a row opened, a row dropped, a file chosen, and a location or span written.
The rows are the card's own rows, so the panel answers as the entry editor answers.
A Situation has no card, so every request names card zero and the engine routes it by the draft.
Every request goes through the tenure, which keys what waits by `LRequestKey`.
So the panel keeps no map of its own of which row is still waiting.

## `private void PScenarioImageShow(IReadOnlyList<LImageDraft> rows)`

Redraws the picture rows from the held Situation, adding, dropping, and moving rows to match.
What was waiting is written before the read, so a redraw never lands over a newer keystroke.

## `private void PScenarioVideoShow(IReadOnlyList<LVideoDraft> rows)`

Redraws the video rows the same way, location and span apart.

## `private PImage PScenarioImageCreate(LImageDraft draft)`

A picture row holding the engine's location, and nothing typed.

## `private PVideo PScenarioVideoCreate(LVideoDraft draft)`

A video row holding the engine's location and span, and nothing typed.

## `private void PScenarioImageChange(object sender, TextChangedEventArgs e)`

Turns a location typed into any picture row into a deferred request for that row, as the entry editor does.
The change bubbles up from the row's field to the list, so the panel names no field.
Only a field the keyboard is in has been typed into.

## `private void PScenarioVideoChange(object sender, TextChangedEventArgs e)`

Turns a location or span typed into any video row into a deferred request for that row.
Which of the two was typed is read from what the field binds to.

## `private void PImageAddHandle(object sender, RoutedEventArgs e)`

Opens a picture row at the end of the list, for the add button under the rows.

## `private void PVideoAddHandle(object sender, RoutedEventArgs e)`

Opens a video row at the end of the list.

## `public void PImageRemoveHandle(object sender, RoutedEventArgs e)`

Drops the row the click came from.

## `public void PVideoRemoveHandle(object sender, RoutedEventArgs e)`

Drops the row the click came from.

## `public void PImageOpenHandle(object sender, RoutedEventArgs e)`

Chooses a picture file and sends its path as the row's location at once.

## `public void PVideoOpenHandle(object sender, RoutedEventArgs e)`

Chooses a video file and sends its path as the row's location at once.
