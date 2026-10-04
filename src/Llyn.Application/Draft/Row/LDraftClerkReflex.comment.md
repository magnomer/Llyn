# LDraftClerkReflex.cs
Hash: `6f8f9116225f8d3a`

## `public sealed class LDraftClerkReflex`

The reflex rows of a draft, an ordered list the draft holds by id.
It is edited one request at a time like the transcription rows.
A row is added with its language and kind and filled afterwards.
So a blank row is held and named, and the commit leaves it out.

## `public LDraftClerkReflex(LLanguageCache languages, LIdentity identity)`

Holds the pack cache a row is respelled and cut through and the issuer that names a new row.

## `public LEntryDraft? LReflexApply(LEntryDraft content, LRequest request)`

Routes every reflex request to its handler, and answers null for any other request.
The clerk then hands that request on to the card lists.
A language, text or respelling request respells the row and recuts its anatomy under the entry's own language.
A romanization or note request replaces that one text of the named row.
A meaning request also marks the row as user-owned so the meaning survives a rebuild.
The region only comes from the fetch.
An anchor request ties or unties the named row and one fanqie row.
The row's other anchors stand as they were.

## `public static LReflexDraft? LReflexFind(LEntryDraft content, long id)`

The reflex row `id` names in the draft, or null.
The list's own find matches the id, so an id of zero is refused as there.

## `public static LReflexDraft LReflexDefaultRead(LEntryDraft content, long reflex)`

The language and kind a row added from row `reflex` starts with.
It copies the pressed row's, and a zero or gone id starts blank.

## `public static int LReflexPositionRead(LEntryDraft content, long reflex)`

The place a row added from row `reflex` takes, right below it.
A zero or gone id places it last.

## `private LEntryDraft LReflexAdd(LEntryDraft content, LRequestReflexAddition request)`

A new reflex row for the language and kind sent and a minted id, at the place asked for.

## `private static LEntryDraft LReflexChange(LEntryDraft content, long reflexId, Func<LReflexDraft, LReflexDraft> change)`

Changes the reflex row named, and refuses when the draft holds none by that id.

## `private static LEntryDraft LReflexApply(LEntryDraft content, Func<IReadOnlyList<LReflexDraft>, IReadOnlyList<LReflexDraft>> change)`

Replaces the whole reflex list of the draft with what the change made of it.
