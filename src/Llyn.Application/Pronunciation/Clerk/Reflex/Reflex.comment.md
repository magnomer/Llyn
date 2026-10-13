# Reflex

This folder holds the reflex clerk with its rows, epithet and fetch.
It also holds the user-owned glosses kept across a rebuild.

## `LReflexClerk`

Reflexes of an entry, their anchors, their rules and the epithet derived from them.
The row sync serves the entry save, and the epithet follows every write of the rows.
The fetch that fills an entry with no rows lives in `LReflexFetch`, built and held here.

## `LReflexClerk.LReflexAnchorScan`

Stored fanqie rows are marked held and estimated for the anchor dropdown.
The estimated classes are the ones the tones resolve for the reflex language and tone.

## `LReflexClerk.LReflexClerkSort`

It is the one door every view's reflex rows pass.
The rule itself is `LReflexOrderSort`, and this only finds the pack's order for it.
A blank language or a pack without an order sorts by language name, kind name, main flag, text and id.
Stored positions are left alone, since the order is applied on every read.

## `LReflexClerk.LReflexClerkScan`

Stored rows become drafts with every field and the stored id kept, in the order given.
The fetch's copy into held drafts, the epithet and the series members in `LStemClerk` all convert here.
So the copy is written once.

## `LReflexClerk.LReflexWrittenScan`

It keeps the rows that carry written text.
A row the reflex fetch left blank stays out of a reading view.
The entry page and the series page both filter here, so the rule has one owner.

## `LReflexClerk.LReflexFoldedRead`

The folded languages come from the pack's reflex rules.
None is given for a blank language or a pack without folded rules.

## `LReflexClerk.LReflexClerkSync`

Anatomies are filled from the pack.
An unchanged list writes nothing and records no revision, whatever order its rows arrive in.
A positive id naming no stored row refuses the commit.
A change is recorded only when something beside the anatomy moved.

## `LReflexClerk.LReflexClerkReset`

Every positive id is cleared, for a fresh entry that has no stored rows to match.

## `LReflexClerk.LReflexEpithetSave`

A language whose rules declare no epithet clears it.
The stored rows pass `LReflexClerkSort` first, so the pieces follow the pack's declared order.
The stored position is fetch order, so it never decides where a piece stands.

## `LReflexClerkEpithet`

The epithet of an entry derived from its reflex rows under the rules of its language.

## `LReflexClerkEpithet.LReflexEpithetFormat`

Each row is formatted by every epithet rule of its language.
Rows determine the outer order, and matching rules determine the pieces within each row.
The caller passes rows sorted by `LReflexClerk.LReflexClerkSort`, so the epithet reads in the pack's declared order.
A clip pattern that fails to compile or times out leaves the piece unclipped.
Whitespace is folded in each piece.

## `LReflexClerkRow`

These pure helpers keep fetched merging and stored-row comparisons independent of the fetch state.

## `LReflexClerkRow.LReflexRoundResolve`

Rows from later rounds that match an earlier row by language, region, kind and romanization join its text.
A row already joined in the same round stays separate.

## `LReflexClerkRow.LReflexRowMatch`

Anchors are compared as sets.
Each current row is paired with the stored row of its id, so position and list order are ignored.
Every view orders the rows on read, so a stored position is never compared.
A current row without a stored id, or naming one twice, is a change.

## `LReflexClerkRow.LReflexAnatomyClear`

The anatomy is emptied, so a comparison ignores it.

## `LReflexFetch`

The fetch that fills an entry with no reflex rows from the reflex source.
It owns the pending fetches, the remembered misses, the admission gate and the request spacing.
Rules are read and the epithet rewritten through `LReflexClerk`.

## `LReflexFetch.LReflexFetchStart`

It starts only for an entry that has rules, no rows and no remembered miss.

## `LReflexFetch.LReflexFetchRebuild`

It does nothing while a fetch is pending or the language has no rules.
Otherwise it clears the rows and the miss, rewrites the epithet, empties the plain held drafts, and fetches again.
Before clearing, it hands the stored rows to `LReflexGloss` to remember their user-owned meanings.
When the fetch lands, those meanings replace the scraped meanings of matching rows.
The bulletins for the entry and every emptied draft are raised here.

## `LReflexFetch.LReflexFetchClear`

It also drops every remembered meaning.

## `LReflexFetch.LReflexFetchScan`

It sends one request per rule and character, spaced by the rule's interval.
The rounds are merged and every row is respelled and given its anatomy.

## `LReflexFetch.LReflexFetchRun`

A fetch is admitted through a two-wide gate.
The answer is written only when the fetch still stands and the entry still has no rows.
The entry must still read the same headword and language.

## `LReflexFetch.LReflexFetchPropagate`

Saved rows are copied into every plain held draft of the entry that has no reflex rows of its own.
A draft carrying an example, situation, reference or author is skipped.
The `sweep` flag overwrites drafts that have rows, for a rebuild.
Rows are put in the order the draft's language pack declares, not the order the rules ran in.
They become drafts through `LReflexClerk.LReflexClerkScan`, the one copy of a stored row into a draft.

## `LReflexGloss`

The user-owned meanings of reflex rows, remembered per entry across a rebuild.
A rebuild clears the rows and fetches them again, which would lose meanings the user typed.
`LReflexFetch` owns one and reads and writes it only under the engine's gate.

## `LReflexGloss.LReflexGlossRecord`

It remembers every non-empty user-owned meaning before a rebuild clears them.
An entry with no such meaning forgets anything remembered for it before.

## `LReflexGloss.LReflexGlossRestore`

Remembered meanings go to matching fetched rows, and the remembered set is consumed.
A restored meaning is marked user-owned again.

## `LReflexGloss.LReflexGlossClear`

`LReflexFetchClear` calls it when it cancels the pending fetches that would have restored the meanings.

## `LReflexGloss.LReflexGlossFormat`

The language, region, kind and text key identifies a reading across a rebuild.

## Stamp

Commit: `c2f335c331fe`
- `LReflexClerk.cs` `a792f584f32b011e`
- `LReflexClerkEpithet.cs` `8095434e215e4be7`
- `LReflexClerkRow.cs` `e5aa7a54543a2d92`
- `LReflexFetch.cs` `801377ca2badf7b0`
- `LReflexGloss.cs` `ebf49b306ca16881`
