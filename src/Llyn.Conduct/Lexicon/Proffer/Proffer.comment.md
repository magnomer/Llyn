# Proffer

This folder holds the offer dropdowns a typed field opens.
Each offers rows split around the typed word, and the arrow keys step its lit row.

## `CAuthor`

One author the byline dropdown offers for a typed credit, its name already split around the typed word.
The lead is the whole name when the word is not found in it.
The mark is empty in that case.

## `CProffer`

What a card field keeps after a gate, and the dropdown of stored rows it offers.
One shape serves every field the shared dropdown opens for, the Tag field among them.
The corpus transcript's citation drawer shows the same shape, since it offers the same citation find.
The driver paints it as it comes and decides nothing about the rows.
The text is what the entry keeps once the completed parts went onto the card.
A citation field writes nothing, so its text comes back unchanged and is not painted.
The rows come in the order the engine offers them.

## `CProfferRow`

One stored row the dropdown offers, already split around the typed word.
The mark is drawn in weight.
The count is the ready number of cards or Examples already using the row, empty where none is shown.

## `CProspect`

What a translation field keeps after a gate, and the dropdown of Entries it offers.
The text is what the entry keeps once the gate has linked what it could.
The word is the one the dropdown offers a fresh Entry for in each of the languages.
The rows are the stored Entries the word matches, whole headwords first.
`CProspectChosen` is the engine's verdict that the first row stands chosen, since one Entry answers the word.
The languages are the ones a fresh Entry is offered in, one create row each, the draft's own last.

## `CRegister`

One register a search found, as the Proffer dropdown lists it.

## `CTag`

One tag a search found, as the Tag dropdown lists it.

## `CLantern`

The lit row of an offer dropdown, shared by every medium that steps one with the arrow keys.
It keeps no state, since each medium holds its own lit row and offered count.
Down or Up lights the next or former row, wrapping at either end.
Down from no lit row lights the first row, and Up from no lit row lights the last.
An empty offer lights nothing and answers null, so the key stays unhandled.

## `CProfferLantern`

The lit row of one card's open dropdown, kept in Conduct so both faces step the same row.
A new offer shows no lit row, so the dropdown opens unlit and an arrow key lights the first.
The steps go through `CLantern`, and a refused step keeps the lit row.
A set lights a row only inside the offered count, which lets a chosen offer light its first row.
The lit state tells Enter whether to pick the row or to add the typed text.

## Stamp

Commit: `f3a687d70b47`
- `CAuthor.cs` `5f9d35007ec20787`
- `CLantern.cs` `571f5e22e9961578`
- `CProffer.cs` `8b6860f7cafb1ed0`
- `CProfferLantern.cs` `df40ba9a6571f4ae`
- `CProfferRow.cs` `87a509fbfbd29b73`
- `CProspect.cs` `ff7d458631523cb8`
- `CRegister.cs` `6046704730b3906a`
- `CTag.cs` `2f2ddfeffcfb75c8`
