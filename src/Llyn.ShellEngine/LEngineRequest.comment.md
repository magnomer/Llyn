# LEngineRequest.cs

## `public sealed partial class LEngine`

The engine owns the draft.
A form sends one edit at a time as an `LRequest`, and the engine applies it, saves, and announces.
The form never assembles an `LEntryDraft` from its controls, so the engine's file is the only truth.
Each request kind is applied by a pure function, one per kind, returning a new draft or new content.
Cards and the rows inside them are addressed by id wherever they nest, never by place.
So a form and the engine cannot disagree about which item is meant.
The entry-level fields and the card body are applied here.
The lists inside a card are applied in `LEngineRequestList.cs` and the files beside it.
The sentence, situation and source panels are applied in `LEngineRequestPanel.cs` and `LEngineRequestChip.cs`.

## `public LDraft LEngineRequestApply(LRequest request)`

Applies one request to the held draft it names and returns the draft as saved.
The draft must be held by this engine, so a request for a leftover or another copy's draft is refused.
The content is normalized after the change, so anything the change left unnamed is named before the write.
A request that leaves the draft equal to what was held writes nothing and raises nothing.
So a form may send the whole body of a panel on every pause, and only a real change is announced.
The bulletin is raised outside the gate, after the file is written.
So a subscriber that re-reads on the bulletin reads what was announced, and never deadlocks on the gate.
The saved draft is returned as well, so a caller can read a minted id without waiting for the bulletin.

## Inline notes

### `private LDraft LEngineRequestApply(LDraft draft, LRequest request)`

The switch over the kinds that reach past the entry content.
The example and source panels edit their own field of the draft, and the credits edit its author list.
The body records lay every field of a panel over the held one, so the engine decides what changed.
A situation field request may mean the panel's situation or a chip, so it is routed by id.
Everything else is a change to the entry content and falls through to the content switch.

### `private LEntryDraft LEngineRequestApply(LEntryDraft content, LRequest request)`

One switch over the entry and card kinds, ending in the list switch.
A kind no switch knows is a programming error, not a refusal, since no form can send one.
A null text or value is read as empty.
So a request can never leave a null where the draft holds text.

### `private static LStateValue LEngineValueRead(LStateValue? value)`

The value sent, or the unspecified one where the form sent null.

### `private static LPronunciationDraft LEngineSoundRead(LEntryDraft content)`

The pronunciation as held, or a blank one to write over.
The reading and the recording are two requests, and either may arrive first.

### `private LEntryDraft LEngineCardInsert(LEntryDraft content, LRequestCardAddition request)`

Mints the id of the new card here rather than leaving it to the normalize.
The answer must name the card, and a card named at birth cannot be confused with one named later.

### `private static LEntryDraft LEngineCardInsert(`

A collocation takes no parent, because only a meaning names a parent in the store.
A parent the meanings do not hold is a missing card, and is refused as one.

### `private static LEntryDraft LEngineCardMove(LEntryDraft content, LRequestCardShift request)`

The card is taken out of wherever it sits, then put back under the parent named.
Its kind is remembered from where it was found, so a card never changes list by moving.
A parent inside the card being moved is gone by the time it is looked for.
So the move refuses rather than loops.

### `private static LEntryDraft LEngineCardChange(LEntryDraft content, long id, Func<LCardDraft, LCardDraft> change)`

Applies one change to the card named, wherever it nests, and refuses when no card carries the id.
Every list request inside a card comes through here, so the card check is written once.

### `private static IReadOnlyList<LCardDraft> LEnginePositionUpdate(List<LCardDraft> cards)`

Renumbers one list from one, which every insert, removal and move leaves to do.
A card already carrying its number is kept as it is, so an untouched card stays the same reference.
