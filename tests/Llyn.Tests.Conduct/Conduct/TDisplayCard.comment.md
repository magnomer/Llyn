# TDisplayCard.cs
Hash: `50e526f4f1d8a874`

## `public sealed class TDisplayCard`

Covers the reading view's card, media, etymology and chip reads and click gates.
Most cases run on a real workspace, where the wing opens the entry and the display's area answers.
The media cases call the card reads directly with a real or fake media port.

## `public void DisplayCardRead_ShownEntry_AnswersReadyCardsAndSections()`

A Meaning card arrives ready, and only the Meaning section shows.
An unknown title carries the unknown mark's key, and a never-written expression arrives muted.
Its link arrives named, and its stored tag arrives worded and ready to open.

## `public void DisplayCardRead_SentenceRow_AnswersTheReadyLine()`

A sentence row arrives with its frame and divided sentence ready, and names its stored row.
An unknown role reads the unknown mark inside the head's brackets.
A sentence with no citation or gloss answers an empty citation and no gloss.

## `public void DisplayCardRead_NothingShown_AnswersNoCardsAndNoSections()`

With nothing open, no card or section is answered.

## `public void FolioImageRead_RowNobodyLocated_CarriesTheEmptyVerdict()`

A picture or video row with no location carries the empty verdict, and a located one does not.

## `public void FolioImageRead_LocatedRow_CarriesTheAddressTheEngineResolved()`

An image row carries the address the port answers for its plain location, and none where the port answers none.
The map hands the port the plain text, including a blank one, and settles nothing itself.

## `public void FolioVideoRead_LocatedRow_CarriesTheScreenTheEngineResolved()`

A video row carries the screen the port answers for its plain location, and none where the port answers none.
The map hands the port the plain text and settles nothing itself.

## `public void FolioVideoRead_WrittenSpan_CarriesTheMomentsTheEngineRead()`

A written span carries its start and end moments.
A row with no span starts at zero and has no end.

## `public void FolioVideoRead_HostedOrOtherLocation_CarriesAddressAndEngineFilmId()`

Over the real media outlet, a hosted location carries its film id, and a trimmed one still resolves.
A plain web file carries no film id, and a workspace path that reaches no file carries no screen.
A blank location carries no screen.

## `public void DisplayIncomingRead_NothingChosen_AnswersNone()`

With no entry chosen, no usage is listed.

## `public void DisplayEtymologyRead_SourceLink_ShowsTheFieldWithItsNamedLink()`

A source link alone shows the field, the row of links and the section, aimed at its source Entry.
Its read narrative stays hidden, since no words stand.

## `public void DisplayEtymologyRead_TwoSourceLinks_ShowsTheRowOfLinks()`

Two source links show the row of links with both targets.

## `public void DisplayEtymologyRead_Narrative_ShowsTheFieldWithItsText()`

A narrative alone shows the field, its read face and the section, with its text.
The row of links stays hidden, since no link stands.

## `public void DisplayEtymologyRead_NarrativeText_ShowsTheReadFaceOnlyWithWords(string text, bool expected)`

The read face shows only a narrative with words in it, so blank text keeps it hidden.

## `public void DisplayEtymologyRead_NoEtymology_HidesTheFieldAndTheSection()`

An entry with no etymology hides the field, its read face, its row of links and the section.

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

Clicks `chip` or `link` and notes in `opened` each tab and record the click raises.

## `private static CWing TDisplayWingPrepare(CAtelier atelier, List<string> asked)`

A left wing whose envoy notes every question and failure in `asked`.

## `private static LEntry TDisplayEntrySave(LEngine engine, string headword)`

Stores one English entry with one Meaning.
