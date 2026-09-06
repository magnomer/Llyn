# LEngineUsage.cs

## `public sealed partial class LEngine`

The usage half of the engine, over every object a card references rather than owns.
An Example, a Situation, and a Reference are all independent data cited from any number of places.
Each is browsed by a panel that must say how often one is cited and by what.
So the question is asked once here for every kind, rather than once under each name.
A Reference is cited by Entries and Examples, which is a different citing side from the other two.

`LOwner` names the kind being asked about, not the referring side.
The referring sides come back inside the answer.

## `public IReadOnlyDictionary<string, int> LEngineUsageRead(LOwner owner)`

The reference count of every object of that kind at once.
The catalog shows the figure on every row, and it decides which delete may be offered.
Reading it once per catalog fill costs one statement rather than one per row.

## `public IReadOnlyList<LUsage> LEngineUsageRead(string id, LOwner owner)`

Where one object is used, itemized.
A count says how many, this says which side cites it, and which id the row is followed by.
The relationship is what the shell must keep.
An Example reached through one Meaning is not a property of the whole Entry.
