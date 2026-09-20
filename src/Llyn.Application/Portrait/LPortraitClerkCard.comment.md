# LPortraitClerkCard.cs

## `public static class LPortraitClerkCard`

The card-side sections of the entry page: the meaning and collocation bands, the incoming links and the note.

## `public static void LPortraitBandAdd(List<LPortraitSection> sections, string heading, IReadOnlyList<LPortraitSection> cards)`

A band of cards under `heading`, added only when there are cards.

## `public static void LPortraitIncomingAdd(List<LPortraitSection> sections, IReadOnlyList<LUsage> incoming, LPortraitLabel label)`

One usage row per entry that translates into this one, each linking back to its owner.

## `public static void LPortraitNoteAdd(List<LPortraitSection> sections, LEntryDraft draft, LPortraitLabel label)`

The note as a prose section, when there is one.
