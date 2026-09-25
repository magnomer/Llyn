# LPortraitText.cs

## `public static class LPortraitText`

The one place a stored state becomes shown text.
Every writer takes its text from here, so no two formats disagree about an unknown field.

## `public static string LPortraitTextRead(LStateValue? value, string mark)`

A specified value shows its text, an unknown one shows the mark, and an unset one shows nothing.
This is the display's own rule, kept in logic so an export cannot drift from it.

## `public static string LPortraitTitleRead(LStateValue value, string vacant, string mark)`

The head of a page read from a state, falling back to `vacant` when the state shows nothing.
An example, a source and a situation page all head themselves this way.
