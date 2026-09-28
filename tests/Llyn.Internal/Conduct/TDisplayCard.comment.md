# TDisplayCard.cs

## `public sealed class TDisplayCard`

Covers the reading view's card reads and click gates on a real workspace.
The wing opens the entry, so each case stands on its display's area.

## `public void DisplayCardRead_ShownEntry_AnswersOrderLinksAndSections()`

The converters get the language's sentence order and each linked entry named, and only the Meaning section shows.

## `public void DisplayCardRead_NothingShown_AnswersNoLinksAndNoSections()`

With nothing open, no link, byline or section is answered.

## `public void DisplayIncomingRead_NothingChosen_AnswersNone()`

With no entry chosen, no usage is listed.

## `public void DisplayEtymologyRead_SourceLink_ShowsTheFieldWithItsNamedLink()`

A source link alone shows the field and the section, with the link named.

## `public void DisplayEtymologyRead_Narrative_ShowsTheFieldWithItsText()`

A narrative alone shows the field and the section, with its text.

## `public void DisplayEtymologyRead_NoEtymology_HidesTheFieldAndTheSection()`

An entry with no etymology hides both.

## `public void DisplayChipOpen_StoredRecord_RaisesTheTabItsKindPicks()`

A stored situation, register or tag raises its own tab.
A link chip raises its entry in the library tab.

## `public void DisplayChipOpen_UnsavedRecord_OpensNothing()`

A record never saved, a link to no entry and an empty click open nothing.

## `public void DisplayMentionFind_TextWithoutLanguage_ReadsItInTheShownLanguage()`

A clicked word in a text naming no language is found among the shown entry's language.
The same word read as French finds nothing, and no failure is shown.

## `public void DisplayCardFind_ShownCards_AnswersTheListAndThePlace()`

Each card answers its list and place, and an unknown card or an empty view answers nothing.

## `private static bool TDisplayChipOpen(CDisplay area, object? chip, long? link, List<string> opened)`

Clicks `chip` while noting in `opened` each tab and record the chip raises for the navigation.

## `private static CWing TDisplayWingPrepare(CAtelier atelier, List<string> asked)`

A wing on its left side whose envoy notes every question and failure in `asked`.

## `private static LEntry TDisplayEntrySave(LEngine engine, string headword)`

Stores one English entry with one Meaning.
