# LEngineReflexFetch.cs

## `public sealed partial class LEngine`

The fetch side of the reflexes: an Entry with none is filled once from the rules its language pack names.
The fill runs in the background and never blocks the display that asked for it.
What it stores are ordinary rows, so the user may then correct or drop them.

## `public void LEngineReflexStart(long entryId)`

Begins a background fill for the Entry identified by `entryId` and returns at once.
Nothing starts when a fill runs already or the sources missed it this session.
Nothing starts either when the Entry is missing, its pack lists no rule, or it already keeps rows.
So an Entry the user emptied on purpose is asked again only at the next start.

## `public void LEngineReflexRebuild(long entryId)`

Drops the stored rows of the Entry and every held draft's rows, then begins a fill as `LEngineReflexStart` does.
The reading view and the drafts are told at once, so they empty before the fill answers.
A miss recorded this session is forgotten, so the sources are asked again.
Nothing happens while a fill runs already, or when the Entry is missing or its pack lists no rule.

## `public bool LEngineReflexCheck(long entryId)`

Whether a fill for the Entry is running, so a surface may show its loading line.

## `public async Task<IReadOnlyList<LReflexDraft>> LEngineReflexFind(`

Asks the rules of `language` for `headword` and returns what they read, stored nowhere.

## `private async Task<(IReadOnlyList<LReflexDraft> LReflexFound, bool LReflexReached)> LEngineReflexScan(`

The fetch behind the find, one rule at a time in written order.
Every Han character of the headword is asked under each rule.
The rule's interval holds the next fetch back from any rule, since one site serves several rules.
The second value says whether any site was reached at all.
A headword every site reached yet none knew is a miss, while sites that never answered are not.
A headword of several characters has its rows merged, since the rows are one entry's.
Every merged row then has its respelling derived from its own language's pack, whatever the switch says.
The text stays as fetched, so both forms are stored and the switch only picks which is shown.

## `private static IReadOnlyList<LReflexDraft> LEngineReflexResolve(IReadOnlyList<LReflexDraft> rows)`

Rows of one language, kind and note folded into one, their texts joined by a space in character order.
The folded row is main when any of its parts was.

## `private void LEngineReflexClear()`

Cancels every pending fill and forgets every miss.
Called under the gate when the workspace changes or the engine is disposed.

## `private async Task LEngineReflexRun(LEntry entry, CancellationTokenSource fetch)`

The fill itself, run outside the gate for the whole fetch.
At most two fills fetch at once.
The rows are written only when the fill was not cancelled.
The Entry must also still stand as fetched, with no rows of its own.
A fill writes no revision row, because a machine fill is not a user edit.
Every exception is swallowed, since a missing reading is not an error the user can act on.
The bulletins are raised outside the gate after the write, one for the entry and one per draft filled.

## `private List<long> LEngineReflexPropagate(long entryId, IReadOnlyList<LReflex> saved, bool sweep = false)`

Hands the stored rows to every held entry draft of this Entry that has none yet.
Returns the ids of the drafts filled.
So an editor open on the entry shows the fetched readings without a reload.
Its own rows are never overwritten, unless `sweep` is set, which a rebuild sets to empty them too.
