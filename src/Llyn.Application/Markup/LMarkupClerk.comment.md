# LMarkupClerk.cs

## `public sealed class LMarkupClerk`

An entry translated to and from markup: the cargo read, the per-entry load and the export.
The import lives in `LMarkupClerkIntake`, since it composes the entry clerk and its own resolvers.

## `public LMarkupClerk(LRig rig)`

Reads the six ports the translation reads through out of `rig`, the same instances the engine holds.
It opens no database of its own, so a test can hand it in-memory ports.

## `public LMarkupCargo LMarkupClerkRead(string path)`

The file at `path` parsed, entries in a language the workspace cannot load losing it with an omission.
Twin names are applied so two entries with one headword are told apart in the intake list.

## `public void LMarkupClerkExport(IReadOnlyList<long> ids, string path)`

The entries loaded in one session, formatted as one tree and written through the markup port.
An id no entry carries refuses the export.

## `public LMarkupEntry? LMarkupLoad(long id)`

Loads the entry draft for `id` and translates every id-bearing field into names.
Null means no entry has that id.
Draft ids and positions are dropped, because list order carries the positions.
Grasp, frequency, favorite and timestamps are never read.
Forms and syllables pass whole, since they hold no id worth hiding.
A reflex also drops its anatomy and anchors, since the receiving engine derives the one and owns the other.

## `private LMarkupEtymology? LMarkupEtymologyCreate(LEtymologyDraft etymology)`

The narrative etymology with each span's target named by headword and language.
Nothing when the etymology carries no text, since only one shape travels.
A span whose target no longer stands is left out, since an etymology span always names an entry.

## `private IReadOnlyList<LMarkupEtymon> LMarkupEtymonCreate(LEtymologyDraft etymology)`

The direct source links named by headword and language, in stored order.
A link whose target no longer stands is left out.

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
