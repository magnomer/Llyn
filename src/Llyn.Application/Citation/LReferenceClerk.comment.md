# LReferenceClerk.cs

## `public sealed class LReferenceClerk`

The clerk over the bibliographic Sources of the workspace.
A Reference is the work an Example is drawn from, independent shared data cited from any number of places.
So a citation is a reference to a row, never ownership of it.
An Example cites at most one, a single column with nothing to order, so attaching there replaces the last.
A Reference no Example cites is deliberate data and goes only when something says to delete it.
The page the sources panel reads is composed here too, so screen and print show one thing.

## `public LReferenceClerk(LRig rig)`

Reads the reference and author ports out of `rig`.

## `public static LReference LReferenceClerkBlank`

What a draft naming no Reference is measured against.
Its author state is unspecified, because nobody has been credited or ruled unknown yet.

## `public LReference LReferenceClerkCreate(LReference reference)`

Creates `reference` and returns it with its assigned id.
Its fields carry the three-state distinction the model gives them: unspecified, deliberately unknown, or a value.

## `public LReference LReferenceClerkCreate(string title)`

Creates a Reference carrying nothing but `title`, for a citation typed where no stored Source answered.
The typed line is resolved like any written value, and a blank line is refused as an argument.

## `public long? LReferenceClerkResolve(string title, long held)`

The stored Reference a typed citation names, zero for a blank line, or `null` when none answers.
Both sides fold the way `LCatalogTextNormalize` folds, so case and edge spaces never mint a twin.
The held Reference wins when its title or byline answers, so re-entering an unchanged line keeps it.
Bylines repeat across works by one author, and only this rule stops a silent switch to a sibling.
Otherwise a title match beats a byline match, since a byline leaves the title out once credits exist.
Ties go to the oldest id, so the same line always lands on the same Reference.

## `public LReference? LReferenceClerkRead(long id)`

Reads the Reference for `id`, or `null` when none has that id.

## `public IReadOnlyList<LReference> LReferenceClerkRead()`

Every Reference the workspace holds.

## `public IReadOnlyList<LReference> LReferenceClerkRead(long ownerId, LOwner owner)`

Reads the single Reference the Example identified by `ownerId` cites, as a list of one or none.
Any side but an Example is refused.

## `public LPortraitPage? LReferenceClerkRead(long id, LPortraitLegend legend)`

The page of one Source, or null when no Source has that id.
The usage tally is counted from the rows citing the Source, as the catalog counts it.

## `public IReadOnlyList<LCatalogReference> LReferenceClerkFind(string query, LCatalogOrder order)`

The Sources answering `query`, in `order`, as rows already carrying their name, credits and citation count.
The credits and the counts are read whole rather than one Source at a time.

## `public IReadOnlyList<LCatalogReference> LReferenceOeuvreFind(long? author, string query, LCatalogFilter kind, LCatalogOrder order)`

Reads the Sources crediting `author` as browsed rows, narrowed by `query` and by the kinds left shown.
Passing `null` lists every Source, as the sources panel lists every Entry under no chosen Source.
Passing zero lists the Sources crediting nobody, so a missing credit can be found and written.
The kinds are matched by their stored word, which is what the filter remembers between sessions.

## `public IReadOnlyDictionary<long, string> LCitationRead()`

The `Author (Year)` line every stored Source is cited under, by id.
One read serves a whole catalog fill, because resolving a name per row would be one query per row.

## `public void LReferenceClerkUpdate(LReference reference)`

Rewrites the fields of the Reference `reference` identifies.

## `public void LReferenceClerkAttach(long ownerId, long referenceId, LOwner owner)`

Cites the Reference identified by `referenceId` from the Example identified by `ownerId`.
Whatever it cited before is replaced.

## `public void LReferenceClerkDetach(long ownerId, LOwner owner)`

Clears the citation the Example identified by `ownerId` holds.
The Reference and its other citations survive.

## `public void LReferenceClerkDelete(long id)`

Deletes the Reference identified by `id` together with the author credits it owns.
Refused while any Example still cites it.
The Authors it credited survive.

## `public void LReferenceClerkDelete(long id, bool detach)`

The same delete, with `detach` clearing every citation first.

## `public static bool LReferenceClerkMatch(LReference one, LReference other)`

Field by field, whether two References say the same thing.
Identity is left out, because a held Reference is named before the record it becomes exists.
The title, the year, the kind, the note and the address each count.
The author state counts too, because ruling the writers unknown is an edit nothing else records.

## `public static LPortraitPage LReferencePageRead(LReference reference, IReadOnlyList<LAuthor> credits, int count, LPortraitLegend legend)`

The title heads the page, and the kind and usage tally are the chips.
Credited authors are joined on one line, and an unknown credit shows the mark alone.
Year, address and note each take a section only when written or marked unknown.

## `private static ArgumentOutOfRangeException LReferenceOwnerRaise(LOwner owner)`

The refusal for a side that cites nothing.
