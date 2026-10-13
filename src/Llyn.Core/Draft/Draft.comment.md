# Draft

Draft holds the tentative records held in the workspace folder before they reach the database.
Every record here is plain data that knows nothing of files or the database.

## `LClaim`

One running program's hold on a tentative record, written where every launch can read it.
A held draft is otherwise known only to the copy of the program that started it.
A second copy would report that live work as a leftover of a crash and offer to recover it.
A process id is reused once its process is gone, so the start time tells the holder from its successor.

## `LCourt`

One tentative link, waiting for the record it points at to become real or be dropped.
The court is the register these links live in, so one row is one court entry.
A file of another shape is skipped and swept, as a draft file is.

## `LDraft`

One tentative record, carrying its own throwaway id, discarded once the record becomes real.
One draft holds one kind of work, told apart by which content field carries it.
A draft naming no example, no situation and no source holds an entry.
One naming an example holds a sentence, a situation a context, and a source a citation.
A held author holds that author's name.
The kind is read from the content rather than from a tag beside it, so the two cannot disagree.
The input panel and the library can each hold one at the same time.
A draft of another kind carries a blank entry id, because its work lives in its own field.
A file the current build did not write is not a draft.
So a reader that finds another version skips it.
There is no compatibility read, because a field renamed underneath an old file would commit as a new entry.
The author list is empty for a draft holding no source, and never null.
A credit with a minted id names an author commit creates before attaching.
An author draft carries no source and no credits, because an author is a shared row edited alone.
`LDraftStored` is the id of the stored record the draft edits, or null for a new one.
A new one has id zero.
`LDraftCreditRead` gives the credits as editor rows knowing their place and whether they can move.
The source editor splices its own blank row in, so it never counts the credits itself.
`LDraftEtymologyFind` gives the etymology span the given span lies inside, or null.
`LDraftMentionFind` gives the Mention the span lies inside, in the example the ids name, or none.
`LDraftExampleRead` gives the example a Mention request with these ids would edit.
Card and sentence both zero name the example the draft holds itself, as the corpus panel sends.
Other ids name a sentence row on a meaning or a collocation, at any depth.
It locates the row exactly as the draft clerk does, so a find and its request agree.
`LDraftNormalize` drops every unreadable value to unspecified.
It is called only after the user agreed to lose what the store could not read.
`LDraftAuthorName` is empty while the draft holds no author, so a name field writes without branching.
`LDraftMentionLanguage` is named on the Example for a sentence draft, and on the entry contents for any other.

## Inline notes

### LDraft.cs: `private static LSentenceDraft? LDraftSentenceRead(IReadOnlyList<LCardDraft> cards, long cardId, long sentenceId)`

It walks the cards and their children for the card, then answers its sentence row.
A card found without that row ends the walk, since card ids are unique.

## `LOutcome`

What one draft commit produced.
The map is an output of the commit, never an input to anything.
Pairs sit in the order the engine created rows.

## `LTenureState`

What a panel holding a draft needs to know to light its buttons, read in one call.
The engine owns every answer in it, so the panel keeps no dirty flag, halted flag or chronicle.
It is a record, so two readings compare by value and a tenure raises its bulletin only when something moved.

## Stamp

Commit: `c2f335c331fe`
- `LClaim.cs` `4bf25b6887736f62`
- `LCourt.cs` `9660b9f2ce0f287e`
- `LDraft.cs` `81fc63ccf63b77dc`
- `LOutcome.cs` `a1f449a024fa30b3`
- `LTenureState.cs` `cf447781d6e742b6`
