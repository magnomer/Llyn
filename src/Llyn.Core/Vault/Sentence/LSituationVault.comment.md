# LSituationVault.cs

## `public interface LSituationVault`

The persistence port for the Situation rows the engine reads and writes.
It lists exactly what the engine asks of situation storage, and nothing about how rows are kept.
`LSituationArchive` in Infrastructure is its adapter over the workspace database.

## `LSituation LSituationCreate(LSituation situation);`

Inserts `situation` with a fresh opaque id and returns the stored Situation with that id filled in.
The new Situation is referenced by nothing until it is attached to a referrer.
Its Images and Videos are not written here.
The engine settles them after the row exists, as it does for a card.
The stored rows come back with their ids.

## `LSituation? LSituationRead(long id);`

Reads the Situation identified by `id`, or `null` when no such Situation exists.

## `IReadOnlyList<LSituation> LSituationRead();`

Every Situation the workspace holds, in the order they were written.
It includes one nothing references, which is reachable nowhere else.

## `IReadOnlyList<LSituation> LSituationMeaningRead(long meaningId);`

Reads the Situations a Meaning references, in the order that Meaning gives them.

## `IReadOnlyList<LSituation> LSituationCollocationRead(long collocationId);`

Reads the Situations a Collocation references, in the order that Collocation gives them.

## `void LSituationUpdate(LSituation situation);`

Rewrites the visible title, description, and kind of the Situation identified by `situation`'s id.
The Images and Videos are the engine's to settle, through the attach and detach the media stores offer.
The id and every reference pointing at it are untouched.
So an update never changes where the Situation appears or in what order.
Throws when no Situation carries that id.

## `int LSituationReferenceRead(long id);`

Counts the references that still point at the Situation identified by `id` — the number `LSituationDelete` refuses a delete over.
A caller that has just detached one reference reads this.
It learns whether the row it detached from was the last one.
No store of its own has to know which association tables exist.

## `IReadOnlyDictionary<long, int> LSituationReferenceRead();`

How many places reference each Situation, the whole shelf in one statement.
A panel listing the catalog needs the figure on every row.
One query per row is a query per row.
A Situation nothing references is absent rather than present as zero.

## `IReadOnlyList<LUsage> LSituationUsageRead(long id);`

The referring sides of one Situation, named rather than counted.
A Meaning is named by its title and, standing without one, by its definition.
A Collocation is named by its title and, standing without one, by its expression.
The Entry each side belongs to is read with it, so a row is legible without a second query.

## `void LSituationDelete(long id, bool detach);`

Deletes the Situation, first dropping every reference to it when `detach` is asked for.
Detaching, counting and deleting share one session.
Between any two of them the answer to whether something still references the row can change.
Without `detach` the count still refuses the delete, which is the guard a card edit relies on.

## `void LSituationMeaningAttach(long meaningId, long situationId, int position);`

References an existing Situation from a Meaning at `position` in that Meaning's order.

## `void LSituationCollocationAttach(long collocationId, long situationId, int position);`

References an existing Situation from a Collocation at `position` in that Collocation's order.

## `void LSituationMeaningDetach(long meaningId, long situationId);`

Removes a Meaning's reference to a Situation.
The Situation and its other references survive.

## `void LSituationCollocationDetach(long collocationId, long situationId);`

Removes a Collocation's reference to a Situation.
The Situation and its other references survive.
