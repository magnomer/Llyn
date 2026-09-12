# TRespelling.cs

## `public sealed class TRespelling`

Covers the built-in reading cleanup and the pack-declared respelling groups.
The cleanup drops syllable dots and every whitespace while stress marks and tie bars survive.
Rules run in written order, so a later rule sees the earlier rule's output.
A scoped group skips an untagged reading and one of another variety, and an unscoped group takes both.
Precomposed and decomposed input reach the same output through NFC.
Scanning a pack chains its groups, and an empty pack returns the input.
The shipped English pack is idempotent over Cambridge, Longman, and Wiktionary style forms, run per variety.
Both the British and the American group recast the glide and open-mid rows, and an untagged reading is left alone.
