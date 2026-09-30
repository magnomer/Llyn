# TDisplayCard.cs

## `public sealed class TDisplayCard`

Covers the reading view's card reads and click gates on a real workspace.
The wing opens the entry, so each case stands on its display's area.

## `public void DisplayCardRead_ShownEntry_AnswersReadyCardsAndSections()`

A Meaning card arrives ready, and only the Meaning section shows.
An unknown title carries the unknown mark's key, and a never-written expression arrives muted.
Its link arrives named, and its stored tag arrives worded and ready to open.

## `public void DisplayCardRead_SentenceRow_AnswersTheReadyLine()`

A sentence row arrives with its frame, sentence and language ready.
An unknown role reads the engine's unknown mark inside the frame's brackets.

## `public void DisplayCardRead_NothingShown_AnswersNoCardsAndNoSections()`

With nothing open, no card or section is answered.

## `public void FolioImageRead_RowNobodyLocated_CarriesTheEmptyVerdict()`

A picture or video row with no location carries the empty verdict, and a located one does not.

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

## `public void DisplayCardFind_ShownCards_AnswersTheListAndThePlace()`

Each card answers its list and place, and an unknown card or an empty view answers nothing.

## `private static CLeafChip TDisplayChipCreate(long id, CSubject subject, bool stored)`

A ready chip of `subject` naming `id`, stored or not.

## `private static bool TDisplayChipOpen(CDisplay area, CLeafChip? chip, long? link, List<string> opened)`

Clicks `chip` while noting in `opened` each tab and record the chip raises for the navigation.

## `private static CWing TDisplayWingPrepare(CAtelier atelier, List<string> asked)`

A wing on its left side whose envoy notes every question and failure in `asked`.

## `private static LEntry TDisplayEntrySave(LEngine engine, string headword)`

Stores one English entry with one Meaning.
