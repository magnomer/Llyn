# TCardSpeech.cs

## `public sealed class TCardSpeech`

Covers the part of speech gates over an input editor on a real workspace, with no delay.
Typed text stays pending in the draft, and the read answers it beside the chips.
A committed name becomes one chip and clears the pending text, and a blank name sends nothing.
An erased chip leaves while the pending text stays.
A draft changed from elsewhere clears the pending text.
An undone draft shows its own list, and the next add builds on it with no stale copy.
Typing answers the draft language's parts that hold the text, each marked when held.
A text matching none carries its own key.
The read carries the same menu for the settled text, and a desk with no draft carries the empty-catalog key.

## `private static CEditor TCardSpeechPrepare(LEngine engine)`

Builds an input editor, opens an English draft and settles the speech read once.

## `private static string[] TSpeechNamesRead(CEditor editor)`

Flushes the deferred requests and answers the names the draft carries.
