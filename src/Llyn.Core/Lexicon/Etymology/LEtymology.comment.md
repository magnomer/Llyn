# LEtymology.cs

## `public sealed record LEtymology(`

The narrative shape of one entry's etymology.
It holds the prose text and the spans of that text which name other entries.
An entry keeps either this narrative or a list of bare Etymon links, never both.
A span is an `LMention`, so one written word points at one Entry.
The spans count Unicode scalar values, not UTF-16 units, as every Mention does.
An entry without an etymology has no row at all.

**Parameters**

- `LEtymologyId` — Opaque, program-generated stable id, zero for a row the store has not written.
- `LEtymologyEntryId` — The Entry whose origin this narrative explains.
- `LEtymologyText` — The prose of the explanation, empty when the entry keeps links instead.
- `LEtymologyMentions` — The spans of the text that name an Entry, held in offset order.

## `public bool LEtymologyNarrated`

Whether the row carries prose rather than nothing.
A blank narrative is no narrative, and the store writes none for it.
