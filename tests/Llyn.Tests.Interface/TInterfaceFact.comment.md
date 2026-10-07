# TInterfaceFact.cs
Hash: `3c75feaf9de90795`

## `internal static partial class TInterface`

The relays for the facts the engine records answer with, plus the localization, theme, usher and flag cache relays.
Each relay hands one production operation through unchanged.

## `static TInterface()`

Lists the embedded catalog languages once, as the bootstrap does, so the localization relays normalise against the real list.

## `internal static IReadOnlyList<LFanqieRow> TFanqieRowSort(IReadOnlyList<LFanqieRow> rows, IReadOnlyList<LFanqieBook> books)`

Relays the order the fanqie clerk reads and groups rows in, so a test pins it over built rows.

## `internal static LEpoch TEpochCreate(string label, string code)`

Builds one declared epoch of a script style.

## `internal static LScriptImage TScriptImageCreate(string character, string style, int position, string caption, string epoch, long id = 0)`

Builds one stored script image with no gloss and no data.
The epoch and the stored id are the keys a sort test varies.

## `internal static IReadOnlyList<LScriptImage> TScriptImageSort(IReadOnlyList<LScriptImage> images, IReadOnlyList<LScriptStyle> styles, IReadOnlyList<string> spelled)`

Relays the script image order, so a test pins it over built images.
