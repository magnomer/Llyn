# Sentence

This folder holds the persistence ports for what hangs on a Meaning or Collocation card.
That covers sentences, mentions, translations, tags, registers and situations.
It also holds the entry-level favorite and note.
Each port lists exactly what the engine asks of its storage, and nothing about how rows are kept.
Every adapter is a `{Base}Archive` in Infrastructure over the workspace database.
Meanings and Collocations are the two referrers, so most ports repeat each method for both.

## `LSentenceVault`

The persistence port for the Sentence rows, what a card holds over its Examples.

## `LSentenceMeaningRead`

Reads what a Meaning holds over its Examples, in the order that Meaning gives them.
Each row carries the frame the Meaning reads it under, and the Example itself when the row cites one.
The Example is joined loosely, because a row may state a frame and cite no Example.
`LSentenceCollocationRead` reads a Collocation on the same terms.

## `LSentenceMeaningSave`

Writes a Meaning's whole set at once, in the order given.
A row the list names by a positive id of this Meaning is rewritten in place and keeps that id.
A row named by no id, or by an id this Meaning does not hold, is inserted fresh.
A row the list stopped naming is deleted, so the list handed in is the list that stands.
The answer lists the ids in the order given, so the caller can map a draft id to its row.
A row has content of its own, so a caller holding its id finds the same row after the save.
`LSentenceCollocationSave` writes a Collocation on the same terms.

## `LSentenceParticleRead`

Reads every marker already saved under an Entry written in the language, without duplicates.
Nothing ships a marker, so this is the only list the shell has to offer.
A store holding none returns none.
`LSentenceDependenceRead` reads the roles on the same terms.

## `LSentenceLoad`

Reads the example order the language pack declares.
A missing pack or section yields the default order.

## `LMentionVault`

The persistence port for the Mention rows, spans of an Example that point at an entry and sense.

## `LMentionExampleRead`

Every Mention of the Example, ordered by where each starts.

## `LMentionExampleSave`

Rewrites the Mention list of one Example and answers the written ids in input order.
A span past the end of the stored text is refused, and so is a pair of spans that overlap.
The refusal names the offending span and throws before any row is touched.
Existing rows are shelved by shifting their start far up, so the unique (example, start) index never trips.
A row whose positive id survives is rewritten in place, the rest are inserted, and the shelf is deleted.
This is the same shape as `LSentenceMeaningSave`.

## `LMentionSenseClear`

Drops the sense from every Mention citing the meaning, so each lands on its entry alone.
The answer names each affected Example once, so a caller can report what lost its sense.
The markup Replace path uses it before the target's senses are rewritten.

## `LMentionSenseSet`

Points a Mention at a sense and answers whether a row changed.
A Mention with no entry is left alone, since a sense without an entry breaks the row's check.
The markup import uses it once every entry of a file is stored and the cited senses exist.

## `LTranslationVault`

The persistence port for the Translation rows, the links a card makes to other Entries.

## `LTranslationMeaningRead`

Reads the links a Meaning makes, in the order that Meaning gives them.
`LTranslationCollocationRead` reads a Collocation on the same terms.

## `LTranslationMeaningSave`

Writes a Meaning's whole Translation line, replacing whatever it carried.
Ids are trimmed, blanks and repeats are dropped, and what survives is numbered from zero.
`LTranslationCollocationSave` writes the same for a Collocation.

## `LTranslationTargetRead`

Reads the headword and language of every Entry named, in the order the ids were given.
A whole card's links resolve in one query rather than one query each.
An id no Entry answers is skipped rather than reported, because a caller wants what it can show.

## `LTranslationIncomingRead`

Reads the Meanings and Collocations that point at this Entry.
Each one is a Usage, so an Entry shows what renders it the way an Example shows what quotes it.
Meanings come before Collocations, each group ordered by headword and then by place on its Entry.

## `LTagVault`

The persistence port for the Tag rows a card carries.

## `LTagMeaningRead`

Reads the Tags a Meaning carries, in the order that Meaning gives them.
`LTagCollocationRead` reads a Collocation on the same terms.

## `LTagMeaningSave`

Writes a Meaning's whole Tag line, replacing whatever it carried.
Each Tag is resolved to a row by id, or by trimmed text when it carries no stored id.
Blanks and repeats are dropped, and what survives is numbered from zero in the order given.
The answer holds one row id per Tag handed in, zero for a blank.
The caller maps a draft id to its row from it.
`LTagCollocationSave` is the same write for a Collocation.

## `LTagRead`

Reads one Tag by id, null when no row carries it.

## `LTagResolve`

The id of the row reading the text, created when no row reads it yet.

## `LTagCatalogRead`

Reads every Tag row, in alphabetical order.
That is the Tag list itself.

## `LRegisterVault`

The persistence port for the Register rows, the formality marks of a card.

## `LRegisterCreate`

Inserts a register with a fresh id when it carries none, and returns the stored Register.
The new Register is referenced by nothing until it is attached to a referrer.

## `LRegisterDefaultCreate`

Writes the rows a language pack names, keyed by name.
A name already present keeps its row id and gains the built in flag.
So a name the user wrote before any pack named it becomes built in, and every mark on it stays.
Seeding is therefore safe to run on every read of a language's shelf.

## `LRegisterRead`

Reads the Register by id, or null when no such Register exists.
The overload without an id reads the whole shelf, the built in rows first and the written rows after.

## `LRegisterMeaningRead`

Reads the Registers a Meaning is marked with, in the order that Meaning gives them.
`LRegisterCollocationRead` reads a Collocation on the same terms.

## `LRegisterNameUpdate`

Rewrites the visible name of a written Register and never its id.
A row a language pack ships is left as it stands, because the pack owns its wording.

## `LRegisterReferenceRead`

Counts every referenced Register in one read, so a shelf never asks once per row.
A Register nothing references is absent rather than present as zero.

## `LRegisterMeaningAttach`

Marks a Meaning with a Register at the index the caller names.
`LRegisterCollocationAttach` does the same for a Collocation.

## `LRegisterMeaningDetach`

Drops one Meaning's mark and renumbers what that Meaning still carries.
`LRegisterCollocationDetach` does the same for a Collocation.

## `LRegisterLoad`

Reads the default registers the language pack declares, unstored and in pack order.
An unknown language or a pack without registers yields an empty list.

## `LSituationVault`

The persistence port for the Situation rows, the settings a card is used in.

## `LSituationCreate`

Inserts a situation with a fresh opaque id and returns the stored Situation with that id filled in.
The new Situation is referenced by nothing until it is attached to a referrer.
Its Images and Videos are not written here.
The engine settles them after the row exists, as it does for a card.

## `LSituationRead`

Reads the Situation by id, or null when no such Situation exists.
The overload without an id gives every Situation the workspace holds, in the order they were written.
It includes one nothing references, which is reachable nowhere else.

## `LSituationMeaningRead`

Reads the Situations a Meaning references, in the order that Meaning gives them.
`LSituationCollocationRead` reads a Collocation on the same terms.

## `LSituationUpdate`

Rewrites the visible title, description and kind of the Situation.
The Images and Videos are the engine's to settle, through the attach and detach the media stores offer.
The id and every reference pointing at it are untouched.
So an update never changes where the Situation appears or in what order.
It throws when no Situation carries that id.

## `LSituationReferenceRead`

Counts the references that still point at the Situation.
It is the number `LSituationDelete` refuses a delete over.
A caller that has just detached one reference reads this to learn whether it was the last one.
No store of its own has to know which association tables exist.
The overload without an id counts the whole shelf in one statement.
A panel needs the figure on every row.
A Situation nothing references is absent rather than present as zero.

## `LSituationUsageRead`

The referring sides of one Situation, named rather than counted.
A Meaning is named by its title and, standing without one, by its definition.
A Collocation is named by its title and, standing without one, by its expression.
The Entry each side belongs to is read with it, so a row is legible without a second query.

## `LSituationDelete`

Deletes the Situation, first dropping every reference to it when detach is asked for.
Detaching, counting and deleting share one session.
Between any two of them the answer to whether something still references the row can change.
Without detach the count still refuses the delete, which is the guard a card edit relies on.

## `LSituationMeaningAttach`

References an existing Situation from a Meaning at a position in that Meaning's order.
`LSituationCollocationAttach` does the same for a Collocation.

## `LSituationMeaningDetach`

Removes a Meaning's reference to a Situation.
The Situation and its other references survive.
`LSituationCollocationDetach` does the same for a Collocation.

## `LFavoriteVault`

The persistence port for the Favorite rows, the entries a user marked.

## `LFavoriteSave`

Marks the entry a favorite, stamping the moment.
Marking an entry that is already marked keeps the earlier stamp.

## `LFavoriteDelete`

Unmarks the entry.
The entry itself stands untouched.

## `LFavoriteCheck`

Reports whether the entry is marked.

## `LFavoriteFind`

Finds the marked entries whose headword carries the query, folded for case and accent.
An empty query returns every marked entry.
Rows come back by headword, and the caller reorders them.

## `LNoteVault`

The persistence port for the Note rows, one free note per entry.

## `LNoteSave`

Writes the note as the entry's Note, replacing any Note the entry already had.
The text is stored exactly as given, and the engine hands it over as normalized Markdown.

## `LNoteRead`

Reads the entry's Note, or null when it has none.

## `LNoteDelete`

Removes the entry's Note, if it has one.
The entry itself is untouched.

## Stamp

Commit: `c2f335c331fe`
- `LFavoriteVault.cs` `4c5d23ba49bbe0be`
- `LMentionVault.cs` `88bb6bed978dbd37`
- `LNoteVault.cs` `9afbdddc12dd482d`
- `LRegisterVault.cs` `48def68f298b92d3`
- `LSentenceVault.cs` `475c9e9fd8ab11e5`
- `LSituationVault.cs` `f9705b191a4d7d75`
- `LTagVault.cs` `270ffc4836960341`
- `LTranslationVault.cs` `865b7298102e5363`
