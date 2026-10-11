# LSheetLineage.cs
Hash: `06b6b6a43367472e`

## `internal static class LSheetLineage`

Writes the incoming, etymology and note sections of an exported entry body.
They follow the decks in the order of `PDisplay.xaml`.
Their content and frame are the portrait bands as they are.

## `internal static void LSheetIncomingAppend(StringBuilder sheet, LPortraitPage portrait, LPortraitLabel label)`

Writes the incoming band, the entries that link here, when the page has one.

## `internal static void LSheetEtymologyAppend(StringBuilder sheet, LPortraitPage portrait, LPortraitLabel label)`

Writes the etymology band when the page has one.

## `internal static void LSheetAnnotationAppend(StringBuilder sheet, LPortraitPage portrait, LPortraitLabel label)`

Writes the note band when the page has one, its heading and card whole.
`LSheetNote.LSheetNoteAppend` writes only one markdown body, so this one takes a name of its own.
`LSheetEntry` calls it straight after etymology, where the view places the note.
