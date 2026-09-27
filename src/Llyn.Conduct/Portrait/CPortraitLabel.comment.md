# CPortraitLabel.cs

## `public sealed record CPortraitLabel(`

The localized words an export or a print needs, read by the driver.
Localization lives in the driver, and the panel controller turns the words into the engine's label.

**Parameters**

- `CPortraitLabelUnknown`: what stands in for a field that could not be read.
- `CPortraitLabelMeaning`: the singular word, used as a card kind.
- `CPortraitLabelMeanings`: the plural word, used as a section heading.
- `CPortraitLabelCollocation`: the singular word, used as a card kind.
- `CPortraitLabelCollocations`: the plural word, used as a section heading.
- `CPortraitLabelIncoming`: the heading over the entries that link here.
- `CPortraitLabelNote`: the heading over the entry note.
- `CPortraitLabelForm`: the heading over the written forms.
- `CPortraitLabelParadigm`: the heading over the inflection slots.
- `CPortraitLabelFrequency`: the heading over the frequency rows.
- `CPortraitLabelGlyph`: the heading over the character chips.
- `CPortraitLabelScript`: the heading over the character form plates.
- `CPortraitLabelFanqie`: the heading over the rime book rows.
- `CPortraitLabelExample`: the heading of one example row under a card.
- `CPortraitLabelGloss`: the label of a gloss line whose language is unnamed.
- `CPortraitLabelSource`: the label of the source line under an example.
- `CPortraitLabelMention`: the heading over the words an example mentions.
- `CPortraitLabelEtymology`: the heading over the entry's etymology.
- `CPortraitLabelSituation`: the heading over situation chips.
- `CPortraitLabelRegister`: the heading over register chips.
- `CPortraitLabelTranslation`: the heading over translation links.
- `CPortraitLabelTag`: the heading over tag chips.
