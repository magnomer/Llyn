# LEngineUsage.cs

## `public sealed partial class LEngine`

The usage half of the engine, over every object a card references rather than owns.
An Example, a Situation, and a Reference are all independent data cited from any number of places.
Each is browsed by a panel that must say how often one is cited and by what.
So the question is asked once here for every kind, rather than once under each name.
A Reference is cited by Examples, and the cards holding them are the way up to those citations.
An Author is asked the same question, answered by tracing the Sources they are credited on.

`LOwner` names the kind being asked about, not the referring side.
The referring sides come back inside the answer.

## `public IReadOnlyDictionary<string, int> LEngineUsageRead(LOwner owner)`

The reference count of every object of that kind at once.
The catalog shows the figure on every row, and it decides which delete may be offered.
Reading it once per catalog fill costs one statement rather than one per row.
Only an Example, a Situation and a Reference are counted this way, and any other side is refused.

## `public IReadOnlyList<LUsage> LEngineUsageRead(long id, LOwner owner)`

Where one object is used, itemized.
A count says how many, this says which side cites it, and which id the row is followed by.
The relationship is what the shell must keep.
An Example reached through one Meaning is not a property of the whole Entry.

An Author is traced rather than looked up, through Source, Example and card up to the Entry.
Nothing stores that trace, so no row can contradict the chain it is read from.
`LUsage` is accepted by no write path, so a trace cannot come back as an input.

## `private static IReadOnlyList<LUsage> LEngineUsageResolve(IReadOnlyList<LUsage> rows, LEntryArchive entries, bool epithet)`

Twins the headwords and fills each row's epithet when the setting asks for it.
