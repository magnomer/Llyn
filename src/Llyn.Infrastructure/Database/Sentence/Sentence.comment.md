# Sentence

This folder persists Examples, the sentences cards quote, and the links that reach them.
An Example is the first entity that no Entry, Meaning or Collocation owns.
It is created once with an opaque id and referenced by any number of owners.
Each class has one responsibility.
The Example itself, the quoting side, the owner association, the Mentions and the Translation links each have a file.

## `LExampleArchive`

Persists Examples.
Each association carries the position the Example takes for that referrer alone.
So attaching and detaching only write association rows and leave the Example and its other references untouched.
`LExampleDelete` refuses to run while any reference remains.
The Glosses are rows kept by `LGlossArchive`, rewritten with the Example and removed with it.
The single Source an Example cites is a reference only.
It is never created, updated or deleted from here.
The Mentions are the Example's own rows, kept by `LMentionArchive`.
Every read here fills them, and every create and update writes them in the same session.
The association tables live in `LExampleLink`, which owns attaching, detaching and reading a set by referrer.

## `LExampleCreate`

Inserts the Example with a fresh opaque id and returns it with the id filled in.
The new Example is referenced by nothing until it is attached.
An Example's language is optional and an empty one is stored as written.
A sentence may be written in two languages, so no one language can be demanded of it.

## `LExampleRead`

Reads one Example by id, or null when none exists.
The parameterless overload answers every stored Example in insertion order, in one statement for the whole table.

## `LExampleUpdate`

Rewrites language, text, Source reference, Glosses and Mention list.
The Example's own id and every reference pointing at it are untouched.
So an update never changes where the Example appears or in what order.
It throws when no Example carries that id.

## `LExampleTextUpdate`

Rewrites only the text.
A Mention whose span no longer fits the new text is dropped, and the rest are kept as they stand.
Shifting a span to follow an edit is the engine's work, not the store's.
It throws when no Example carries that id.

## `LExampleSourceUpdate`

Sets the single Source the Example cites, or records that it has none.
Only the reference moves.
The Source row itself is never created, changed or removed here.
It throws when no Example carries that id.

## `LExampleReferenceRead`

Counts the references that still point at an Example, which is the number `LExampleDelete` refuses a delete over.
A caller that has just detached one reference learns whether it was the last.
No store of its own has to know which association tables exist.
The parameterless overload counts every Example at once across the two association tables, for the browsing panel.

## `LExampleSingleRead`

Reads one Example on a connection the caller already holds.
It is shared with `LExampleLink`, which resolves a whole referrer's set at once.

## `LExampleUsageRead`

Answers the cards quoting the Example through `LExampleLink`, so the port has one adapter.

## `LExampleDelete`

Deletes the Example, clearing every reference first when detach is set.
The clearing, the count and the delete share one session.
So the row is judged against the references as they stand at that moment.
The Source it cited is left standing, because a citation is a pointer and never ownership.

## `LExampleLink`

Reads the quoting side of an Example, meaning which Meanings and Collocations quote it.
A Meaning and a Collocation reach their Examples through `LSentenceArchive`, because that association carries data.
This class owns no association table of its own.
Reporting who quotes an Example is a different responsibility from storing one.
Nothing here creates, changes or deletes an Example.
An Entry does not quote an Example.
It reaches one only through its cards, so there is no Entry arm to read, attach or detach.

## `LExampleUsageRead` in `LExampleLink`

Itemizes every card quoting one Example rather than counting them.
Each row names the Entry it belongs to and carries the entry id it is followed through.
So it stays followable after the text it shows is edited.
A quoting card with no wording of its own falls back to the definition or expression beneath it.
Card rows sharing a headword go by language, then entry id, then place on the Entry.
Those are the keys `LCatalogEntry` breaks entry ties by, so storage order never decides a place.

## `LExampleLinkClear`

Drops every reference to one Example from every association table.
The Meaning and Collocation sides go to `LSentenceArchive`, which also clears it where it stands as a revision.
It takes the caller's connection, so the clearing and the delete that follows commit together.

## `LSentenceArchive`

Owns the association that reaches an Example from a Meaning or a Collocation.
It stands apart from `LExampleLink` because this association carries data.
An owner's hold on an Example states the frame it reads the Example under.
That fact does not belong on the Example, which is shared and owned by nothing.
Nothing here creates, changes or deletes an Example.

Each row carries the position the Example takes for that owner, and that order is a unique index.
So every save writes the owner's whole set under fresh positions.
A caller names the index it wants and never has to find a free position.

The two owners differ only in table and owner column.
So each public method names its owner and one private implementation does the work.
No caller ever passes a table name.

## `LSentenceMeaningRead`

Reads what a Meaning holds over its Examples, in the order the Meaning gives them.
Each row carries the frame and the Example itself when the row cites one.
The Example is joined loosely, because a row may state a frame and cite no Example.
`LSentenceCollocationRead` does the same for a Collocation.

## `LSentenceMeaningSave`

Writes a Meaning's whole set at once, in the order given.
A row the list names by a positive id of this Meaning is rewritten in place and keeps that id.
A row named by no id, or by an id this Meaning does not hold, is inserted fresh.
A row the list stopped naming is deleted, so the list handed in is the list that stands.
The answer lists the ids in the order given, so the caller can map a draft id to its row.
A row has content of its own, so a caller holding its id finds the same row after the save.
`LSentenceCollocationSave` does the same for a Collocation.

## `LSentenceParticleRead`

Reads every marker already saved under an Entry written in a language, without duplicates.
Nothing ships a marker, so this is the only list the shell has to offer.
A store holding none returns none.
`LSentenceDependenceRead` does the same for roles.

## `LSentenceLoad`

The example order the language pack declares, read through `LSentenceLoader`.

## `LSentenceLoader`

Reads the one thing a language pack says about a Sentence's frame, which field is written first.
The pack states an order and nothing else.
No marker and no role is ever shipped.

## `LSentenceLoaderLoad`

The order comes from languages/`<language>`/vocabulary.json under exampleOrder.
A pack that is missing, unknown or silent on the key leaves the marker written first.
So does a name `LLanguageNameValidate` refuses.
That fallback is a layout choice the form needs to draw at all, and never a claim about the language.
A key naming only one of the two fields is not an order, so it is left as none stated.

## `LMentionArchive`

Persists Mentions, the words of an Example that stand for an Entry.
A Mention is the Example's own row, so it is read and written by Example.
`LExampleArchive` and `LSentenceArchive` call the static readers here to fill every Example they return.
`LExampleClerk` in the Application layer reads them through the vault.

## `LMentionExampleRead`

Reads every Mention of one Example, ordered by where each starts.

## `LMentionExampleSave`

Rewrites the Mention list of one Example and answers the written ids in input order.
A span past the end of the stored text is refused, and so is a pair of overlapping spans.
The refusal names the offending span and throws before any row is touched.
Existing rows are shelved by shifting their start far up, so the unique (example, start) index never trips.
A row whose positive id survives is rewritten in place, the rest are inserted, and the shelf is deleted.
This is the same shape as `LSentenceOwnerSave`.

## `LMentionSenseClear`

Drops the sense from every Mention citing a meaning, so each lands on its entry alone.
The answer names each affected Example once, so a caller can report what lost its sense.
The markup Replace path uses it before the target's senses are rewritten.

## `LMentionSenseSet`

Points a Mention at a sense and answers whether a row changed.
A Mention with no entry is left alone, since a sense without an entry breaks the row's check.
The markup import uses it once every entry of a file is stored and the cited senses exist.

## `LMentionExampleSweep`

Deletes every Mention whose span no longer fits the stored text.
A text update calls it after the new text is written.
The rows that still fit are kept, and nothing is shifted here.

## `LMentionTextRead`

Returns the stored text length in code points, or zero when the text is not stated.
It throws when no Example carries the id.
The count matches the span arithmetic, so a surrogate pair is one character on both sides.

## `LTranslationArchive`

Persists the Translation links a Meaning or a Collocation makes to other Entries.
A Translation carries no words of its own.
It is the id of the Entry that renders the card, and the place that id takes on the card.
The words are read back from that Entry, so a headword renamed once reads renamed everywhere.
The two association tables are the whole of the storage.

A card's Translation line is written as a whole rather than edited a link at a time.
The old rows go and the new ones are numbered from zero in the order given.
Zero ids and repeats are dropped on the way.
So saving the same list twice leaves the same rows, and positions stay contiguous by construction.

Links are stored one way, from the card to the Entry.
The Entry finds them by looking back through the same rows.
Deleting an Entry takes its incoming rows with it, leaving the cards that pointed at it standing.

## `LTranslationTargetRead`

Reads the headword and language of every Entry named by the ids, in the order given.
A whole card's links resolve in one query rather than one each.
Zero and repeated ids are dropped first.
An id no Entry answers is skipped rather than reported, because a caller wants what it can show.
The ids travel as one JSON parameter unpacked with json_each.
So no id is composed into the statement, and the list has no ceiling.

## `LTranslationIncomingRead`

Reads the Meanings and Collocations that point at this Entry.
Each one is a Usage, so an Entry shows what renders it the way an Example shows what quotes it.
Meanings come before Collocations, each group ordered by headword.
Card rows sharing a headword go by language, then entry id, then place on the Entry.
Those are the keys `LCatalogEntry` breaks entry ties by, so storage order never decides a place.

## Inline notes

### LExampleArchive.cs: internal static IReadOnlyList<LExample> LExampleListLoad(

It gives every Example in a list its Glosses and Mentions, each read in one statement for the whole list.
The sentence reader shares it, so a card's sentences are filled in one round trip.

### LExampleArchive.cs: private static int LExampleReferenceRead(SqliteConnection connection, long id)

It is the same count on a connection the caller already holds.
So a guard and the delete it guards run in one transaction.
Nothing can attach the row between them.

### LSentenceArchive.cs: internal static void LSentenceExampleClear(SqliteConnection connection, long exampleId)

It drops every owner's hold on one Example before that Example is deleted.
Both tables are swept, because a Meaning and a Collocation may each cite it.
It renumbers each owner's remaining set through `LDatabaseOrder`, so no gap is left.
It takes the caller's connection, so the clearing and the delete that follows commit together.

### LSentenceArchive.cs: private IReadOnlyList<string> LSentenceFrameRead(string column, string language)

Both owner tables are read at once, because a marker written on a Collocation is a marker the language uses.
The language is the owning Entry's, not the cited Example's.
A frame may stand with no Example at all.
Reading the language off the Example would hide exactly those rows.
Only rows stating a value are offered, so an unknown or an unwritten field adds nothing.
The column name is a store-owned literal named by the two public methods and never caller input.

### LSentenceArchive.cs: private static IReadOnlyList<LSentence> LSentenceOwnerRead(

The table and owner column are store-owned literals, never caller input.
So composing them into the statement text opens no injection seam.
Every value still travels as a parameter.

### LSentenceArchive.cs: private static IReadOnlyList<LSentence> LSentenceListLoad(

It fills the Glosses and Mentions of every cited Example after the rows are read, in two further statements.
A row citing nothing is passed through as it stands.

### LSentenceArchive.cs: private static bool LSentenceChange(

It rewrites one row in place and answers whether a row of this owner carried that id.
The owner is part of the match.
An id that belongs to another owner is not touched and reads as new.

### LSentenceArchive.cs: private static void LSentencePositionAdjust(

It shifts every position of one owner's set far up before the rewrite.
The unique (owner, position) index would otherwise reject a row moving onto a slot another row still holds.
A row the rewrite reaches is brought back down to its new slot, and the rest stay shelved.

### LSentenceArchive.cs: private static void LSentenceOwnerClear(

It deletes every row still shelved after the rewrite, which is every row the list stopped naming.

### LMentionArchive.cs: internal static IReadOnlyDictionary<long, IReadOnlyList<LMention>> LMentionExampleRead(

It reads every Mention of a set of Examples in one statement, keyed by Example.
The ids travel as one JSON parameter, so the whole corpus fits in one statement.
An Example with no Mention has no key, and the caller reads that as an empty list.

### LMentionArchive.cs: internal static IReadOnlyList<long> LMentionExampleSave(

It is the rewrite on a connection the caller already holds.
So an Example insert and its Mentions commit together.

### LTranslationArchive.cs: private void LTranslationReferrerSave(string table, string column, long referrerId, IReadOnlyList<LTranslation> translations)

The two association tables differ only in name and card column, so the write is one implementation.
Both identifiers are store-owned literals chosen by the public methods, never caller input.
So composing them into the statement text opens no injection seam.
Every value still travels as a parameter.

## Stamp

Commit: `c2f335c331fe`
- `LExampleArchive.cs` `b9bb4d64c0d8097f`
- `LExampleLink.cs` `c43d8a5f1605fbb1`
- `LMentionArchive.cs` `96d0fccc401a7bd6`
- `LSentenceArchive.cs` `3e860d98972f8c9b`
- `LSentenceLoader.cs` `1821d5b08d619e05`
- `LTranslationArchive.cs` `b21dd0173eb4f998`
