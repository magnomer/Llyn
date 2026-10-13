# Etymology

Etymology holds the two shapes an entry's origin can take.
An entry keeps either a narrative or a list of bare Etymon links, never both.
An entry without an etymology has no row at all.

## `LEtymology`

The narrative shape: prose text and the spans of it which name other entries.
A span is an `LMention`, so one written word points at one Entry.
The spans count Unicode scalar values, not UTF-16 units, as every Mention does.

## `LEtymologyDraft`

The etymology of one entry while it is being edited, carrying both shapes together.
The editor shows one field for both, and the clerk writes whichever shape the draft ended in.
The draft never decides which shape wins, since a draft may pass through an empty state.
`LEtymologyDraftNarrated` is answered by the text alone, so a narrative with no span still counts.
`LEtymologyDraftEmpty` drafts clear both stored shapes when saved.
`LEtymologyDraftCreate` gives an empty text for a missing narrative and an empty list for missing links.
`LEtymologyDraftResolve` leaves out the link ids, since the two shapes are written apart.
`LEtymologyDraftFind` counts a span with no length as inside when it stands anywhere on the Mention, its ends included.

## `LEtymologyResult`

The engine's answer for an entry's etymology as the reading view shows it.
Named fields keep the narrated and linked verdicts from trading places.
`LEtymologyResultFilled` is true for words in the narrative or a standing link.

## `LEtymon`

One source entry an etymology links to directly, drawn as a chip beside the headword and carrying no prose.
The target is held as the Entry's id, never as text, so a rename follows by itself.
An entry holds its Etymons in the order the user put them in.
A span inside a narrative is an `LMention` instead, never an Etymon.

## Stamp

Commit: `c2f335c331fe`
- `LEtymology.cs` `524b5b07dde54c2b`
- `LEtymologyDraft.cs` `f56817dc554fb15c`
- `LEtymologyResult.cs` `ca0d5ab7dc7d79d7`
- `LEtymon.cs` `8ca04787d3309bda`
