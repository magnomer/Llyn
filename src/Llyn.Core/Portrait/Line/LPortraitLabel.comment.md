# LPortraitLabel.cs
Hash: `0dc4501554f04f66`

## `public sealed record LPortraitLabel`

The localized words an export needs, resolved by the caller.
Localization lives in the shell, and the engine holds none.
Passing the words in keeps export out of the interface layer without moving translation into logic.

**Parameters**

- `LPortraitLabelUnknown` - what stands in for a field that could not be read.
- `LPortraitLabelMeaning` - the singular word, used as a card kind.
- `LPortraitLabelMeanings` - the plural word, used as a section heading.
- `LPortraitLabelCollocation` - the singular word, naming the kind of a referring collocation in the incoming band.
- `LPortraitLabelOther` - the heading over the collocations linked to no meaning.
- `LPortraitLabelStub` - the format of a see-line, filled with the expression, else the title, and the home meaning's number.
- `LPortraitLabelHeading` - the format of the heading over a meaning's collocation rows, filled with the row count.
- `LPortraitLabelIncoming` - the heading over the entries that link here.
- `LPortraitLabelNote` - the heading over the entry note.
- `LPortraitLabelForm` - the heading over the written forms.
- `LPortraitLabelParadigm` - the heading over the inflection slots.
- `LPortraitLabelFrequency` - the heading over the frequency rows.
- `LPortraitLabelGlyph` - the heading over the character chips.
- `LPortraitLabelScript` - the heading over the character form plates.
- `LPortraitLabelFanqie` - the heading over the rime book rows.
- `LPortraitLabelExample` - the heading of one example row under a card.
- `LPortraitLabelGloss` - the label of a gloss line whose language is unnamed.
- `LPortraitLabelSource` - the label of the source line under an example.
- `LPortraitLabelMention` - the heading over the words an example mentions.
- `LPortraitLabelEtymology` - the heading over the entry's etymology.
- `LPortraitLabelSituation` - the heading over situation chips.
- `LPortraitLabelRegister` - the heading over register chips.
- `LPortraitLabelTranslation` - the heading over translation links.
- `LPortraitLabelTag` - the heading over tag chips.
- `LPortraitLabelUnit` - the name of each lexical unit, which leads the speech chips.
- `LPortraitLabelFreedom` - the name of each freedom, which follows the unit in the speech chips.
