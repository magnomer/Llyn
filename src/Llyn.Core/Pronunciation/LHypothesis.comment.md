# LHypothesis.cs

## `public sealed record LHypothesis(`

The reconstruction a language pack names under `hypothesis`: how a rime-book placement sounds.
It is the user's own system of initials, finals and tone changes, held as tables in a pack file.
The engine knows nothing of Middle Chinese, only that an initial and a final are looked up and joined.
A pack without the file carries `null`, and the fanqie box prints the placement alone.

**Parameters**

- `LHypothesisInitials` — The onset each initial stands for, keyed by the initial's character, such as 疑 to `ng`.
- `LHypothesisFinals` — The final each rime stands for, keyed by rime and division, such as `模 一` to `o`.
  A rounded placement first tries the key with `合` appended, such as `寒 一 合` to `wan`.
  A rime ending in a capital letter, the 重紐 mark, is tried as written and then without the letter.
- `LHypothesisTones` — The class rows of each rime-book tone, keyed by the tone's character.
  Each row is one [LHypothesisTone](LHypothesisTone.comment.md): an onset pattern, its rewrites and its class label.

## `private const string LHypothesisRounded = "合";`

The suffix a rounded final's key carries.

## `public LHypothesisSound? LHypothesisResolve(LFanqieRow row)`

The sound of one placement: onset and final joined, then the tone's first matching row applied.
The row's rewrites run over the joined syllable, and its class becomes the sound's class.
A tone outside the table, or with no row taking the onset, keeps the joined syllable with a blank class.
`null` when the initial or the final is not in the tables.
The box then falls back to the placement text.

## `private string? LHypothesisFinalFind(LFanqieRow row)`

Looks the final up, rounded key first when the placement is rounded, then the plain key.
The rime is tried as written and then without its 重紐 letter.

## `private static IEnumerable<string> LHypothesisRimeScan(string rime)`

The rime as written, then the rime without a trailing capital letter when it has one.

## `private static string LHypothesisKeyFormat(string rime, string division, bool rounded)`

The finals key: rime, a space and the division, then a space and `合` when rounded.
A placement without a division keys on the rime alone.
