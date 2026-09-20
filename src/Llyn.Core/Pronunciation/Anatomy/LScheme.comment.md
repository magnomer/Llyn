# LScheme.cs

## `public sealed record LScheme(string LSchemeName, IReadOnlyList<LSourceSpec> LSchemeSources)`

One transcription scheme a language pack declares, such as Jyutping or Pinyin.
A transcription row names its scheme by this name, and the pack lists where that scheme can be looked up.
The engine holds no list of schemes of its own.

**Parameters**

- `LSchemeName` — The scheme's name as the pack spells it, the tag a transcription row carries.
- `LSchemeSources` — The sources the pack declares for looking a word up in this scheme.
  A scheme declared as a bare name carries an empty list, and its lookup finds nothing.
