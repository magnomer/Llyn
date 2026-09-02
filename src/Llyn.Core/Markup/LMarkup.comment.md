# LMarkup.cs

## `public static class LMarkup`

Reads Llyn Markup text into entry drafts. Llyn Markup is the plain-text format Llyn imports and exports whole entries in — one file carrying any number of entries as tagged text — and `docs/Format-LlynMarkup.md` is the authority on its syntax and on the meaning of every tag. This class implements that document and decides nothing on its own; where the two disagree the document is right and the code is wrong.

The result is a list of `LEntryDraft`, the same shape the editor produces, so imported text and typed text reach the rest of the program through one door. Reading stops at drafts: nothing here touches the database, the editor, or any store.

## `public static IReadOnlyList<LEntryDraft> LMarkupRead(string text)`

Takes the whole text of a Llyn Markup document and returns one draft per entry it declares, in the order the file declares them. The tag reading itself is not written yet, so the list comes back empty.

**Parameters**

- `text` — The whole Llyn Markup document, as read from an `.llx` file or held in memory.
