# LPortraitClerkCard.cs

## `public static class LPortraitClerkCard`

The card-side sections of the entry page: the meaning and collocation bands, the incoming links and the note.

## `public static void LPortraitBandAdd(List<LPortraitSection> sections, string heading, IReadOnlyList<LPortraitSection> cards)`

A band of cards under `heading`, added only when there are cards.

## `public static void LPortraitIncomingAdd(List<LPortraitSection> sections, IReadOnlyList<LUsage> incoming, LPortraitLabel label)`

One usage row per entry that translates into this one, each linking back to its owner.

## `public static void LPortraitEtymologyAdd(List<LPortraitSection> sections, LEntryDraft draft, IReadOnlyDictionary<long, LPortraitLink> targets, LPortraitLabel label)`

Adds the etymology band, the narrative as a line and its spans as links.
A linked etymology shows its source entries as links and no line.
An etymology with nothing to show adds no band.

## `private static void LPortraitEtymologyAdd(List<LPortraitLink> links, IReadOnlyDictionary<long, LPortraitLink> targets, long id)`

Adds one resolved target once, skipping an id no read could resolve.

## `public static void LPortraitNoteAdd(List<LPortraitSection> sections, LEntryDraft draft, LPortraitLabel label)`

The note as a prose section, when there is one.
