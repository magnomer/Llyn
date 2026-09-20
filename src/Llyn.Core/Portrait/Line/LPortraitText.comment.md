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

## `public static IReadOnlyList<string> LPortraitTextRead(LPortraitPage page)`

Every string the page shows, in reading order, headed lines and chips before the sections.
A writer that carries the page whole prints every one of them.
So a coverage test reads its expectations here and never from the draft.

## `public static IReadOnlyList<string> LPortraitTextRead(LPortraitSection section)`

Every string one section shows, children recursed.
A new node field joins here beside the record.
The writer tests then turn red until every format carries it.
Empty strings are skipped, since a writer prints nothing for them.
A band or card prints its heading.
A chip, link or quote section is dressed by its role and never prints one.
An image is a picture and not text, so its address and caption are not listed.
A video is a link drawn as text, so its address and span are.
