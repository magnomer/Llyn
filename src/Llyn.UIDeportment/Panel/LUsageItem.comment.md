# LUsageItem.cs

## `public sealed class LUsageItem`

Presentation item for one referring side in the guild's citations and the view's incoming usages.
Carries the Entry the referring side belongs to, what names that side, and whether it quotes an Example.
The relationship is kept rather than flattened.
The row says a Meaning or a Collocation carries the Situation, never the whole Entry.
The Entry id is the way from this row to the panel holding that Entry.

## `public LUsageItem(LUsage usage, string owner, string unknown, string unnamed, string epithet = "")`

Builds the row from one read referring side.
The owner text is the word for that kind in the user's language, handed in rather than decided here.
A side that names itself nowhere shows the unnamed text, and one marked as not known shows the mark.
The row finds its own flag once for the language through `LEnsignImage`.

## `public bool LUsageItemQuoted { get; }`

Whether the side quotes an Example rather than naming an Entry.
A click reads it to decide whether the row leads to an Entry or to an Example.

## `public string LUsageItemEpithet { get; }`

The epithet the row prints after the headword, small and muted, in the reading the language pack names.

## `public static IReadOnlyList<LUsageItem> LUsageItemBuild(IReadOnlyList<LUsage> usages)`

A plain copy loop over the citing places, wording each owner and taking the epithet the row carries.
The labels read through `LLocalizationCatalog`, so the deportment words them without the veneer.

## `public void LUsageItemShow(Func<long, bool> exampleSeam, Func<long, bool> entrySeam)`

Leads to the place: the Example when the usage quotes one, else the Entry.
The window opens the place through the two seams, since the deportment cannot reach the window.
