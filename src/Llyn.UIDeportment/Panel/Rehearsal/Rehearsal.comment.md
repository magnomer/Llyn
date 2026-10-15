# Rehearsal

The drivers of the flashcard tab placeholders.
The deck, cue and stencil drivers live in its subfolders.

## `QStudy`

Drives the veneer's `PStudy` placeholder page, which the window places.
`QStudyIntroduce` builds `CStudy` and looks up the empty-line key it names.
  It keeps no field for it, because the page has nothing else to ask.

## `QScoreboard`

Drives the veneer's `PScoreboard` placeholder page as `QStudy` drives `PStudy`.

## Stamp

Commit: `2f9a06b8eb95`
- `QScoreboard.cs` `829d51d650a24269`
- `QStudy.cs` `7d22059c98619a03`
