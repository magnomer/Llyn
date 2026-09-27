# QQuill.cs

## `public sealed class QQuill`

The gate for the text rows of one desk.
A driver hands it the texts it gathered, and the gate builds the request and every written state inside.
It is sealed, so its public members name only .NET types and Conduct shapes.
It takes the `Q` prefix until the gates move to Conduct, where it becomes `CQuill`.

## `internal QQuill(LDesk desk)`

Only the desk builds one, over itself.

## `public void QQuillSituationChange(string title, string description, string kind)`

Defers the scenario's title, description and kind as one body, as the replaced typing request did.
The draft is read through `LDeskDraft` without a flush, so a pending keystroke keeps its delay.
A flush here would raise the draft bulletin mid-keystroke and redraw the field under the caret.

## `private void QQuillSituationDefer(CSituationDraft? held, string title, string description, string kind)`

Builds the body over the held situation, taken as a parameter so no engine answer sits in a local.

## `private static LStateWritten QQuillWrittenRead(string text, CStateValue? held)`

An empty field whose held value is unknown stays unknown.
Any other text is written as it stands.
