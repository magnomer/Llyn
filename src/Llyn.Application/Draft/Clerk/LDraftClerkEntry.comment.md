# LDraftClerkEntry.cs
Hash: `982b38632e000d4e`

## `public sealed class LDraftClerkEntry`

The entry-level fields of a draft: headword, language, note, parts of speech and lexical unit.
Each is held by the entry itself and replaced whole by its request, outside any card or row list.

## `public LDraftClerkEntry(LLanguageCache languages)`

Holds the pack cache a language change rebuilds respellings, anatomy and unit through.

## `public LEntryDraft? LEntryApply(LEntryDraft content, LRequest request)`

Routes every entry-field request to its field, and answers null for any other request.
The clerk then tries the other row clerks on that request.
A language change derives every pronunciation respelling again and recuts every reflex row's anatomy under the new pack.
It also settles the lexical unit to one the new language offers.
A null text is read as empty, so a request can never leave a null where the draft holds text.
A headword or language change drops no recording here.
The engine does that after the apply, since only it can read the stored entry the draft edits.

## `public static string LNoteResolve(string text)`

The one owner of the note's trim, read by the set and by the check.
A text box carries trailing line breaks that the note never keeps.

## `public static bool LNoteCheck(string text, string note)`

Answers whether a typed text already holds the note, by the same trim the set applies.
A driver asks it before painting, so a line break just typed is not overwritten.
