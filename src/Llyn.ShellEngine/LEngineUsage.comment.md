# LEngineUsage.cs

## `public sealed partial class LEngine`

The usage half of the engine, over every object a card references rather than owns.
An Example and a Situation are both independent data quoted from any number of cards.
Both are browsed by a panel that must say how often one is quoted and by what.
So the question is asked once here for either kind, rather than twice under two names.

`LOwner` names the kind being asked about, not the referring side.
The referring sides come back inside the answer.

## `public IReadOnlyDictionary<string, int> LEngineUsageRead(LOwner owner)`

The reference count of every object of that kind at once.
The catalog shows the figure on every row, and it decides which delete may be offered.
Reading it once per catalog fill costs one statement rather than one per row.

## `public IReadOnlyList<LUsage> LEngineUsageRead(string id, LOwner owner)`

Where one object is used, itemized.
A count says how many, this says which Entry, Meaning or Collocation, and under which Entry.
The relationship is what the shell must keep.
An Example reached through one Meaning is not a property of the whole Entry.
