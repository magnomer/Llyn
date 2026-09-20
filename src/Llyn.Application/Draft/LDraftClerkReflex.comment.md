# LDraftClerkReflex.cs

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
A remark request replaces that one text of the named row, while the region only comes from the fetch.
An anchor request ties or unties the named row and one fanqie row.
The row's other anchors stand as they were.

## `private LEntryDraft LReflexAdd(LEntryDraft content, LRequestReflexAddition request)`

A new reflex row for the language and kind sent and a minted id, at the place asked for.

## `private static LEntryDraft LReflexChange(`

Changes the reflex row named, and refuses when the draft holds none by that id.

## `private static LEntryDraft LReflexApply(`

Replaces the whole reflex list of the draft with what the change made of it.
