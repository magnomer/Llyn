# LEngineExample.cs

## `public sealed partial class LEngine`

The Example half of the engine.
An Example is created, read, rewritten and let go of.
So are the references a Meaning or a Collocation holds to it.
An Example is independent data owned by nothing.
So both card sides may reference the same one.
Each holds its own order over the Examples it references.

Two sides is why the side arrives as an `LOwner` and not in the method's name.
An Entry is not one of them — it reaches an Example only through its cards.
A seam handed a side no association table serves refuses rather than picking one.

The Source an Example cites is set through its own seam.
Only the citation moves there.
The Reference row is never created, changed or deleted by pointing an Example at it.
Pointing one away from it changes nothing either.

## `public LExample LEngineExampleCreate(LExample example)`

Creates `example` and returns it with its assigned id.

## `public LExample? LEngineExampleRead(string id)`

Reads the Example for `id`, or `null` when no Example has that id.

## `public IReadOnlyList<LExample> LEngineExampleRead(string ownerId, LOwner owner)`

Reads the Examples the Meaning or Collocation identified by `ownerId` references.
They arrive in the order that side holds them.

## `public void LEngineExampleUpdate(LExample example)`

Rewrites the language, the sentence and the translation of the Example `example` identifies.
The Source it cites is not touched here — that is `LEngineExampleUpdate(string, string?)`.

## `public void LEngineExampleUpdate(string exampleId, string? referenceId)`

Sets or clears the single Reference the Example identified by `exampleId` cites — pass `null` to clear it.
Only the citation moves: the Reference row itself is neither created, changed, nor deleted here.

## `public void LEngineExampleAttach(string ownerId, string exampleId, int position, LOwner owner)`

References the Example identified by `exampleId` from the side identified by `ownerId`.
That side is a Meaning or a Collocation, and the reference goes in at `position`.
The set is renumbered around it so the positions stay contiguous.

## `public void LEngineExampleDetach(string ownerId, string exampleId, LOwner owner)`

Removes one side's reference to an Example.
The Example and its other references survive.
That is the rule a card edit follows.
Dropping an Example from a card leaves what other cards quote.

## `public void LEngineExampleRemove(string ownerId, string exampleId, LOwner owner)`

Removes one side's reference to an Example and deletes the Example when that was its last reference.
An Example quoted by nothing is unreachable data, so the reference going takes it.
An Example another card still quotes stays as it is.

The detach, the count and the delete share one session.
So the row is judged against the references as they stand at that moment.
That is why this is one seam and not three the shell composes.
Between any two steps the answer to "does anything still quote this" can change.

## `public void LEngineExampleDelete(string id)`

Deletes the Example identified by `id`.
Refused while any Meaning or Collocation still references it.
`LEngineExampleRemove` is the seam that deletes one as its last reference goes.
A Reference it cited is left standing.

## `public IReadOnlyList<LExample> LEngineExampleRead()`

Every Example in the workspace, for the panel that browses the shared stock of sentences itself.
It is not a view of one card's references, so an Example nothing quotes is in it.

## `public void LEngineExampleDelete(string id, bool detach)`

Deletes the Example, dropping every reference to it first when the user asked for that.
Without `detach` it is the refusing delete above.
With it the detaching, the count and the delete are one operation rather than a sequence a caller composes.
Between any two steps the answer to what still quotes this can change.

## `public IReadOnlyList<LCatalogExample> LEngineExampleFind(string query, LCatalogOrder order)`

The Examples answering `query`, in `order`, as rows already carrying their cited name and quotation count.
An Example is matched over its sentence, its translation and the name of the Source it cites.
The name is resolved here, because matching on an id the reader never sees would answer the wrong question.

## `public IReadOnlyDictionary<string, string> LEngineCitationRead()`

The name every stored Source is shown under, by id.
One read serves a whole catalog fill, because resolving a name per row would be one query per row.
