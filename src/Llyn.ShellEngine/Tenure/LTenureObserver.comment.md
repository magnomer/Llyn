# LTenureObserver.cs
Hash: `70f81e40c832f8f9`

A tenure subscribes to the engine for its lifetime and owns the shell's subject-specific observers.
An observer is a delegate over a bulletin, so the shell hands a method and implements no contract.
The tenure's own handler is a private method attached as a method group and detached by the same group.
General observers receive every notice of their subject, while draft observers require the held draft identity.
Entry observers require a stored entry identity, because frequency and related notices do not name the draft.
Preparation drops the held draft's own notices, because the caller reads the newest draft from the return value.
A replayed notice would show the same draft twice, and the editor's show is its costliest step.
The scope unwinds on failure and returns the newest draft after successful preparation.
Cancel and successful finish detach the engine subscription and release the observer list.

## `private LTenureState _lTenureLast;`

The state last announced, so the bulletin is raised only when the state moved.

## `private LTenureState? _lTenureState;`

The state last read, kept because one editor refresh asks for it several times over.
An ended or halted tenure never answers it, so the end and the halt need not drop it.
Cancel and finish both announce through the engine as well, which moves the revision past it.

## `private long _lTenureStateRevision;`

The engine revision read before the kept state was computed.
A change landing mid-reading moves the revision past it, so the next reading computes again.

## `public void LTenureObserverAttach(LSubject subject, Action<LBulletin> observer)`

An attach after the tenure ended is dropped, so a late panel holds nothing past cancel or finish.
The draft and entry attaches drop it the same way.

## `public void LTenureEntryAttach(LSubject subject, Action<LBulletin> observer)`

The entry id is read at attach time.
A draft never stored attaches nothing, and storing it later does not attach it.

## `public LDraft? LTenurePrepare()`

The prepare turn with the engine's own completion of an entry draft, so every edit area shows the same rows.
Other subjects complete nothing and only read the draft.

## `private void LTenureDraftPrepare()`

Applies the completion requests and asks again, since a card added in one round needs its sentence in the next.
Three rounds cover cards, then their sentences, then the check that nothing is left.

## `private bool LTenureDraftApply()`

One round: applies what the engine still finds missing and says whether anything was.

## `public LDraft? LTenurePrepare(Action prepare)`

Runs `prepare` holding the turn, so no other turn interleaves with it.
An ended tenure runs nothing and answers null.
The kept draft is dropped afterwards, so a Mention find never trusts a read taken mid-preparation.

## `public LMentionDraft? LTenureEtymologyFind(LMentionDraft span)`

The span of the live draft's etymology that the given span lies inside.
It reads the live draft, since the etymology gates act on what the user sees now.

## `public LMentionDraft? LTenureMentionFind(long cardId, long sentenceId, LMentionDraft span)`

The Mention the span lies inside, in the Example one sentence field holds, or none.
It reads the draft through `LTenureKeptRead`, so a menu asking many times sends nothing.

## `public bool LTenureMentionCheck(long cardId, long sentenceId, string text, int start, int length)`

Whether a field's selection lies inside any Mention of that sentence, so the unlink command may run.
The selection arrives in UTF-16 units and is measured in code points here.

## `public bool LTenureSenseCheck(long cardId, long sentenceId, string text, int start, int length)`

Whether the selection lies inside a Mention that stands for an Entry, so a sense may be chosen.

## `public long? LTenureSenseRead(long cardId, long sentenceId, string text, int start, int length)`

The Entry whose Meanings the sense menu offers, or none when the Mention links nothing.
Pending typing is persisted first, so the Mention is found against the text the field shows.

## `private LDraft? LTenureKeptRead()`

The held draft as the engine stored it, for the Mention find.
The draft is kept until the next draft bulletin or prepare, so command checks read the file once per change.
A read that raced a bulletin is answered but not kept.
A stale draft after a workspace switch is refused, and the read then answers none.

## `private void LTenureKeptClear()`

Drops the kept draft and advances the round, so a read already under way is not kept.
It runs under the gate, since bulletins can arrive off the veneer's thread.

## `private void LTenureStateRaise()`

Reads the state and raises the tenure bulletin when it differs from the last one raised.
