# LMarkupLoader.cs

## `internal sealed class LMarkupLoader`

Reads one stored entry into the markup record the writer formats.
It is the sibling of `LEntryLoader`, which answers the same rows as a draft.
Every id a stored row carries is translated into the words a reader could type.
It sits beside the engine because it needs every archive and answers only the engine.

## `public LMarkupLoader(LDatabase database)`

Holds the database every archive is opened over.

## `public LMarkupEntry? LMarkupLoad(long id)`

Loads the entry draft for `id` and translates every id-bearing field into names.
Null means no entry has that id.
Draft ids and positions are dropped, because list order carries the positions.
Grasp, frequency, favorite and timestamps are never read.
Forms and syllables pass whole, since they hold no id worth hiding.
A reflex also drops its anatomy and anchors, since the receiving engine derives the one and owns the other.

## `private LMarkupInflection LMarkupInflectionCreate(LInflection inflection)`

Replaces the speech value id and morphology ids with their stored names.
A morphology that no longer exists is skipped rather than written as a number.

## `private IReadOnlyList<LMarkupCard> LMarkupCardCreate(IReadOnlyList<LCardDraft> cards)`

Translates meaning or collocation cards, recursing into child senses.
Every leaf draft is copied with its id set to zero.

## `private IReadOnlyList<LMarkupTranslation> LMarkupTranslationCreate(IReadOnlyList<long> ids)`

Reads the headword and language of every translated entry.
A translation whose entry is gone is skipped.

## `private LMarkupExample LMarkupExampleCreate(LExampleDraft example)`

Copies the example with its glosses, translated mentions and resolved reference.

## `private LMarkupMention LMarkupMentionCreate(LMentionDraft mention)`

Names the mentioned entry by headword and language and its sense by position path.
A mention with no entry, or whose entry is gone, keeps its span and names nothing.

## `private string LMarkupMeaningResolve(long entryId, long meaningId)`

Walks from `meaningId` up to the root of the entry's meaning tree.
The path is one-based positions joined by dots, so the first sub-sense of the first meaning is `1.1`.
No sense yields the empty string.

## `private LMarkupReference? LMarkupReferenceCreate(LStateAnchor anchor)`

Reads the cited reference and its credited authors in their stored order.
An unspecified or unknown anchor, or a reference that is gone, yields null.
