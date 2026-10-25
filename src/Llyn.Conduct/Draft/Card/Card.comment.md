# Card

This folder holds one card of the held draft and the rows inside it.
The rows are sentences, examples, glosses, images, videos and the register, situation and tag chips.

## `CCardDraft`

One meaning or collocation of an entry, as the editor's card shows it.
The id is positive for stored rows and nonpositive for unsaved cards.
The title is worded with no hint on its field.
The expression is worded with the expression hint, and a meaning card carries an unspecified expression instead.
The definition is worded with the hint its sheet names.
`CCardDraftTranslation` lists the entries the card links to, ready in the card's order.
`CCardDraftFolded` is read from the store and never from the draft.
`CCardDraftStored` is the card draft's own `LCardDraftStored` rule passed on, so the view never reads the id.
Only a stored card holds a fold.
`CCardDraftSense` lists the meanings a collocation card links to, in the card's link order.
Each carries its path and no depth, and a link to a vanished meaning is left out.

## `CExampleDraft`

The example a sentence holds, as the sentence row shows it.
The reference is null when none is cited.
The chips of the entries the text mentions come from `CSentence.CSentenceMentionRead`, not from this record.

## `CGlossDraft`

One gloss of an example, as its gloss row shows it.
`CGlossDraftNamed` is the engine's verdict that a language is chosen.
`CGlossDraftHint` is the key a row shows in place of a language while none is chosen.
It is null once one is chosen.
Conduct chooses the key, so the row only looks it up.
`CGlossDraftWording` words the text for its field and its reading line.
The field's hint is the translation hint, and the line shows the unknown mark in place of an unknown text.

## `CImageDraft`

One image of a card or a scenario, as its image row shows it.
`CImageDraftEmpty` is true when no location was ever recorded, so a reading card folds the image away.
The address is where the preview is read from, or null when the location reaches nothing.
A local file counts only while it exists, which the engine checks.
`CImageDraftName` is the shown name of the picture, the console's line text, from `CFolio.CFolioNameRead`.
`CImageDraftWording` words the location for its field with the location hint.

## `CRegisterDraft`

One register of a card, as the card's register chips show it.
The name is worded for its chip.

## `CSentenceDraft`

One sentence of a card, as the sentence row shows it.
The example is null when the sentence holds none yet.
`CSentenceDraftLine` is the line key, the sentence's position among the shown entry's sentences, counted from one.
The line key is what the UI hands back for mention reads, while the row id names the stored row.
`CSentenceDraftCited` is true when the example cites a reference, so the idle list keeps the citation shown.
The text is worded with the example hint, and empty while no example is held.
The particle and the dependence frame the example, and each is worded with its own hint.

## `CSituationDraft`

One situation, as a card's context chips or the repertoire's scenario and vignette show it.
A card's link carries no media, so its picture and video lists stay empty.
`CSituationDraftWording` words the title as a card's context chip shows it.

## `CTagDraft`

One tag of a card, as the card's label chips show it.

## `CVideoDraft`

One video of a card or a scenario, as its video row shows it.
`CVideoDraftEmpty` is true when no location was ever recorded, so a reading card folds the video away.
The screen is what the screen plays, or null when the location reaches nothing.
A local file counts only while it exists, which the engine checks.
`CVideoDraftName` is the shown name of the video, the console's line text, from the screen's address else the location.
`CVideoDraftFrom` is the moment the screen plays from, the film's start when the span names none.
`CVideoDraftUntil` is the moment the screen stops, or null to play to the end.

## Stamp

Commit: `44d0e95bcfcc`
- `CCardDraft.cs` `cbaadd8168067db2`
- `CExampleDraft.cs` `7ea7fee42a2f919c`
- `CGlossDraft.cs` `8fd636e4e09d6989`
- `CImageDraft.cs` `ea2395fe6d3c85c9`
- `CRegisterDraft.cs` `cc78c6b5ae887a4e`
- `CSentenceDraft.cs` `6cfdb00095d044f5`
- `CSituationDraft.cs` `5fa9d073279c56a7`
- `CTagDraft.cs` `9566cd97f2f83c1f`
- `CVideoDraft.cs` `d91dc3773257ff7a`
