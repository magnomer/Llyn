# LAnatomy.cs

## `public sealed record LAnatomy(`

The phonological anatomy of one reflex reading: its onset, vowel, coda and tone, held twice.
One set is cut from the reading as fetched, the IPA or script form, and one from its respelling.
Both sets are always held together, so a consumer never picks one and finds the other missing.
It is derived by the engine from the rules the entry's own pack declares, never by a view.
Each part is text, empty when the reading has no such part or no rule cuts the language.

**Parameters**

- `LAnatomyOnsetIpa` — The onset of the reading as fetched, such as `ㄹ`, `r` or `ʈ͡ʂ`.
- `LAnatomyVowelIpa` — The vowel of the reading as fetched, such as `ㅣ` or `ɤ`.
- `LAnatomyCodaIpa` — The coda of the reading as fetched, such as `ㅁ`, `mu` or `ŋ`.
- `LAnatomyToneIpa` — The tone of the reading as fetched, such as `35`, or empty for a toneless reading.
- `LAnatomyOnsetRespelling` — The onset cut from the respelling, or from the reading when it has none.
- `LAnatomyVowelRespelling` — The vowel cut from the respelling, or from the reading when it has none.
- `LAnatomyCodaRespelling` — The coda cut from the respelling, or from the reading when it has none.
- `LAnatomyToneRespelling` — The tone cut from the respelling, or from the reading when it has none.

## `public static readonly LAnatomy LAnatomyEmpty = new();`

The anatomy of a reading no rule cuts, every part empty.

## `public bool LAnatomyBlank`

True when every one of the eight parts is empty.

## `public string LAnatomyIpaRead(string kind)`

The IPA part a Diwei kind tallies: onset for an initial, vowel and coda joined for a rime, else empty.

## `public string LAnatomyRespellingRead(string kind)`

The same part from the respelling set.

## `private static string LAnatomyPartRead(string kind, string onset, string vowel, string coda)`

Picks the part for the kind from the three handed in.

## `public static LAnatomy LAnatomyCreate(LAnatomyPiece ipa, LAnatomyPiece respelling)`

Joins the two cut pieces into one anatomy, the first the IPA set and the second the respelling set.

## `public static LAnatomy LAnatomyScan(`

The anatomy of one reading under the first rule that names its language.
The rule cuts the reading for the IPA set and the respelling for the respelling set.
A language no rule names yields the empty anatomy.
