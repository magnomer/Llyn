# CPortraitChoice.cs
Hash: `57fa08f2f47e2c37`

## `public sealed record CPortraitChoice(string CPortraitChoiceKey, string CPortraitChoiceSuffix, bool CPortraitChoiceChosen, CPortraitMedium CPortraitChoiceMedium)`

One export format as a driver offers it, ready to show.
The engine names the formats, their suffixes and the default, and Conduct chooses the wording key.
The envoy's file question lists the rows and answers the chosen row's medium.

**Parameters**

- `CPortraitChoiceKey`: the localization key of the format's name.
- `CPortraitChoiceSuffix`: the file suffix the format is written under, dot included, never holding `|`.
- `CPortraitChoiceChosen`: whether the format is the one offered first, true on exactly one row.
- `CPortraitChoiceMedium`: the format itself, as the export gate takes it.
