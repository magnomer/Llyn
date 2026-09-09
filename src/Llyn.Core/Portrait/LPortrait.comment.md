# LPortrait.cs

## `public sealed record LPortrait`

One entry as the display panel shows it, in no particular file format.
Every field is already the text a reader sees, so no writer decides what a field says.
The engine resolves state, ordering and links once, and each writer only lays the result out.
It carries no ids, because nothing downstream navigates anywhere.

The field order is the display's own reading order.
A writer that walks the record top to bottom reproduces the panel.

**Parameters**

- `LPortraitHeadword` - the entry word, shown large at the top.
- `LPortraitLanguage` - the language chip beside the headword.
- `LPortraitPronunciation` - the IPA text shown inside brackets, empty when none is stored.
- `LPortraitSpeech` - the part-of-speech chips, in stored order.
- `LPortraitMeaning` - the meaning cards, in stored order.
- `LPortraitCollocation` - the collocation cards, in stored order.
- `LPortraitIncoming` - the entries whose cards link to this one.
- `LPortraitNote` - the entry note, empty when none is written.
- `LPortraitFavorite` - whether the star is filled.
- `LPortraitLabel` - the localized headings and markers the writers need.
