# Request

This folder holds the tests of the requests a form sends to edit a held draft.
They cover content, gloss, list, mention and situation.

## `TRequest.cs`

Covers the requests a form sends to edit a held draft, and what the engine answers.
A headword request lands in the file, so a re-read sees it without the returned draft.
An added card comes back with a minted id, numbered one, and raises one draft bulletin.
A shift or removal renumbers the whole list from one.
A field request changes the card it names and leaves its neighbour as it was.
A card the draft does not hold is refused with the card reason, and so is card id zero.
A collocation cannot hold a card, so an addition naming one as parent is refused with the collocation reason.
A request for a draft this engine does not hold is refused with the draft reason.
A fetched recording belongs to one spelling and one language, so a new headword or language drops it.
A recording that came with the stored entry is its own, so a corrected headword keeps it.
A recording fetched over the stored one is dropped like any fresh one.
So is a new language over a stored one.
A language sent again as it stands drops nothing.
A text sent again exactly as held changes nothing.
The draft version stays and no bulletin is raised, so a repeated send does not wake every surface.
Title, description and kind requests change the panel's Situation, which keeps its id throughout.

## `TRequestGloss.cs`

Covers the Gloss requests against a held draft, on a card's sentence row and on the corpus panel's own Example.
It covers the minted negative id, the per-row text and language edits, and the removal.
It also covers the commit that stores the rows in order.
It also covers the in-place update of a shared sentence's Glosses.

## `TRequestList.cs`

Covers the list requests inside a card and the panel lists, and what commit makes of them.
A new sentence row is named at once and quotes nothing.
A draft holding only such a row is unchanged.
The first text on a row is the moment its Example is named.
A picked stored Example keeps its positive id and its text.
A typed Situation that matches nothing stored is a new row with a minted id.
A picked Situation arrives with every stored field under its positive id, and picking it twice adds nothing.
A Situation is one row, so retitling it reaches both cards that link it.
Item id zero or an id the card does not hold is refused with the item reason.
Register, Tag, Image and Video additions mint the way Situations do.
A translation picked twice is linked once.
The blank row the form keeps reaches the store as nothing at all.
A fixture sent as requests commits to the same rows the fixture holds, every list included.
Its sentence and Situation rows carry positive ids afterwards.
A source draft crediting a new and a stored author attaches both in order, and marks authorship known.
Credits move and drop by id like any other list.
A typed author name the store holds credits that Author, folded and trimmed.
A name the draft already credits adds no second row.
The author search reads every stored Author the typed text matches anywhere in the name.

## `TRequestMention.cs`

Covers the Mention requests and what commit makes of them.
A word is linked on a card row and on the corpus panel through the same three requests.
A span overlapping a Mention replaces it, and a Meaning of another Entry is refused.
A zero Meaning clears the choice, and a Mention linking nothing refuses one.
A text edit shifts, keeps or drops the Mentions by where the edit fell.
Commit writes the rows and maps the minted ids.
A Mention-only change edits the stored row in place, and a text change forks a shared row.
The draft file round-trips a minted negative id and a Mention standing for nothing.
The editor's four gesture sequences are run as the editor sends them.
A selection past a surrogate pair carries code-point offsets.
Link then choose then unlink leaves nothing.
Silence then choose is refused, and a word linked in the scribe is found at its offset once committed.

## `TRequestSituation.cs`

Covers the image and video requests a Situation draft takes, the same a card takes, with the card id ignored.
The panel sends a card id of 0, because a Situation draft carries no card.
A removal leaves the other picture in place and touches no card.
A location change keeps the row and its id and swaps the path.
A removal naming a row the Situation does not hold is refused as an item, exactly as on a card.
An Entry draft is untouched by the Situation branch, and its picture lands on the card named.
A card id of 0 on an Entry draft is still refused as a card, so the drafts stay apart.

## Inline notes

### TRequest.cs: TRequestRecordingSave

It writes one byte of audio under the workspace, so a saved entry resolves back to the same path.

### TRequestMention.cs: LDraft edited = TMentionTextApply(

Rewriting the letters inside a word drops its Mention and leaves the one after it where it was.
Only a change of length moves the later spans.

## Stamp

Commit: `9377e657b395`
- `TRequest.cs` `82f995465716dab5`
- `TRequestGloss.cs` `79d804e1a57abd67`
- `TRequestList.cs` `f2472021678351ec`
- `TRequestMention.cs` `60240776dab6ce7f`
- `TRequestSituation.cs` `ec54f86c0786f306`
