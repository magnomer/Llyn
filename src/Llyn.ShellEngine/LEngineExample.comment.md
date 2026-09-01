# LEngineExample.cs

## `public sealed partial class LEngine`

The Example half of the engine: an Example with its translations created, read, rewritten and let go of, and the references an Entry, a Meaning or a Collocation holds to it. An Example is independent data owned by nothing, so three sides may reference the same one and each holds its own order over the Examples it references.

Three sides is why the side arrives as an `LOwner` and not in the method's name; unlike a Tag, an Example may hang from an Entry as well, and a seam handed a side no association table serves refuses rather than picking one.

The Source an Example cites is set through its own seam. Only the citation moves there: the Reference row is never created, changed or deleted by pointing an Example at it or away from it.

## `public LExample LEngineExampleCreate(LExample example)`

Creates `example` with its translations as its ordered child rows and returns it with its assigned id.

## `public LExample? LEngineExampleRead(string id)`

Reads the Example for `id` with its translations in stored order, or `null` when no Example has that id.

## `public IReadOnlyList<LExample> LEngineExampleRead(string ownerId, LOwner owner)`

Reads the Examples the Entry, Meaning or Collocation identified by `ownerId` references, in the order that side holds them, each with its translations.

## `public void LEngineExampleUpdate(LExample example)`

Rewrites the text, its local rendering and the translations of the Example `example` identifies. The Source it cites is not touched here — that is `LEngineExampleUpdate(string, string?)`.

## `public void LEngineExampleUpdate(string exampleId, string? referenceId)`

Sets or clears the single Reference the Example identified by `exampleId` cites — pass `null` to clear it. Only the citation moves: the Reference row itself is neither created, changed, nor deleted here.

## `public void LEngineExampleAttach(string ownerId, string exampleId, int position, LOwner owner)`

References the Example identified by `exampleId` from the Entry, Meaning or Collocation identified by `ownerId` at `position` in that side's order, renumbering the set around it so the positions stay contiguous.

## `public void LEngineExampleDetach(string ownerId, string exampleId, LOwner owner)`

Removes one side's reference to an Example. The Example and its other references survive — the rule a card edit follows, where dropping an Example from a card leaves what other cards quote.

## `public void LEngineExampleRemove(string ownerId, string exampleId, LOwner owner)`

Removes one side's reference to an Example and deletes the Example, with its translations, when that was its last reference. An Example quoted by nothing is unreachable data, so the reference going takes it; an Example another card still quotes stays as it is.

The detach, the count and the delete share one session, so the row is judged against the references as they stand at that moment. That is why this is one seam and not three the shell composes: between any two steps the answer to "does anything still quote this" can change.

## `public void LEngineExampleDelete(string id)`

Deletes the Example identified by `id` together with its translations. Refused while any Entry, Meaning or Collocation still references it; `LEngineExampleRemove` is the seam that deletes one as its last reference goes. A Reference it cited is left standing.
