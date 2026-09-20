# LUsageClerk.cs

## `public sealed class LUsageClerk`

The clerk over usage, asked of every object a card references rather than owns.
An Example, a Situation, and a Reference are all independent data cited from any number of places.
Each is browsed by a panel that must say how often one is cited and by what.
So the question is asked once here for every kind, rather than once under each name.
An Author is asked the same question, answered by tracing the Sources they are credited on.

`LOwner` names the kind being asked about, not the referring side.
The referring sides come back inside the answer.

## `public LUsageClerk(LRig rig)`

Reads the author, entry, example, reference and situation ports out of `rig`.

## `public IReadOnlyDictionary<long, int> LUsageClerkRead(LOwner owner)`

The reference count of every object of that kind at once.
Reading it once per catalog fill costs one statement rather than one per row.
Only an Example, a Situation and a Reference are counted this way, and any other side is refused.

## `public IReadOnlyList<LUsage> LUsageClerkRead(long id, LOwner owner, bool epithet)`

Where one object is used, itemized.
A count says how many, this says which side cites it, and which id the row is followed by.
An Author is traced rather than looked up, through Source, Example and card up to the Entry.
Nothing stores that trace, so no row can contradict the chain it is read from.

## `public static IReadOnlyList<LUsage> LUsageClerkResolve(IReadOnlyList<LUsage> rows, LEntryVault entries, bool epithet)`

Twins the headwords and fills each row's epithet when `epithet` asks for it.
The translation clerk names its incoming rows through it too.

## `private static ArgumentOutOfRangeException LUsageOwnerRaise(LOwner owner)`

The refusal for a kind nothing here counts.
