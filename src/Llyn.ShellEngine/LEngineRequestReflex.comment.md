# LEngineRequestReflex.cs

## `public sealed partial class LEngine`

The reflex rows of a draft, an ordered list the draft holds by id.
It is edited one request at a time like the transcription rows.
A row is added with its language and kind and filled afterwards.
So a blank row is held and named, and the commit leaves it out.

## `private LEntryDraft LEngineReflexApply(LEntryDraft content, LRequest request)`

Routes every reflex request to its handler, and hands any other request on to the card lists.

## `private LEntryDraft LEngineReflexAdd(LEntryDraft content, LRequestReflexAddition request)`

A new reflex row for the language and kind sent and a minted id, at the place asked for.

## `private LReflexDraft LEngineRespellingResolve(LReflexDraft row)`

Fills the row's respelling from its reading through the respelling groups of the row's own language.
It runs whether or not the switch is on, so both forms are always held and the switch only picks.
A row of a phonemic language has its square brackets turned into slashes, because the brackets sit inside the reading.
A blank reading, a blank language or a language without groups leaves the respelling blank.
The groups are asked with no variety, so only the unscoped ones apply.

## `private static LEntryDraft LEngineReflexChange(`

Changes the reflex row named, and refuses when the draft holds none by that id.

## `private static LEntryDraft LEngineReflexApply(`

Replaces the whole reflex list of the draft with what the change made of it.
