# LEpoch.cs

## `public sealed record LEpoch(`

One chronology label a language pack lists under a `script` style, paired with the code Llyn stores for it.
A source names the age of an inscription in its own language, such as 西周早期 under a bronze.
The label is the source's word and the code is Llyn's, so the interface language decides what the reader sees.
Every label is pack data, so a source that dates its pictures differently needs no code.

**Parameters**

- `LEpochLabel` — The chronology exactly as the source prints it at the head of a caption.
- `LEpochCode` — The stored code the label stands for, which names a localization key under `Epoch`.

## `public static (string LEpochFound, string LEpochCaption) LEpochResolve(`

Cuts a known chronology off the head of a caption, returning its code and the caption that remains.
The longest label wins, so a dynasty with a period is never mistaken for the dynasty alone.
A label counts only at the head and only when a space or the caption's end follows it.
A vessel named 商鞅方升 therefore keeps its name, because 商 there is the start of a word.
Nothing matches when the pack lists no label for the age the source printed, and the caption is returned whole.
An unmatched chronology stays inside the caption, so a later pack can still cut it out.
