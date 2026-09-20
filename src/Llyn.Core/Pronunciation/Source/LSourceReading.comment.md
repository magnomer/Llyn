# LSourceReading.cs

## `public sealed record LSourceReading(`

One extraction a source attempt applies to a fetched page, declared by a language pack.
An attempt fetches its page once and runs every reading it declares over the same body.
Each reading that matches yields one [LReading](LReading.comment.md) tagged with its variety.
A page listing two varieties is read twice, once per reading, without a second fetch.

**Parameters**

- `LSourceReadingVariety` — The variety tag the extracted form carries, matching one the pack declares.
  It is empty when the reading yields an untagged form.
- `LSourceReadingStrategy` — How to extract the value.
  `"regex"` takes a capture group on the raw body.
  `"json"` follows `LSourceReadingPath`, then optionally a regex.
  `"span"` is the tolerant nested-span IPA reader.
- `LSourceReadingPattern` — The regex applied to the body (regex/span) or to the JSON value (json).
  `null` when the strategy needs none.
- `LSourceReadingGroup` — The regex capture group to read, and `0` for the whole match.
- `LSourceReadingPath` — Dot-separated JSON path for the `"json"` strategy.
  Each segment is a literal property name.
  `null` for other strategies.
- `LSourceReadingPhonetic` — When `true`, the value is a phonetic form: decode entities and strip enclosing IPA delimiters.
  Left `false` for audio URLs.
- `LSourceReadingSkip` — How many earlier matches of the pattern to pass over before reading.
  `0` reads the first match.
  It lets one page yield a second variety from its second span.
- `LSourceReadingEvery` — When `true`, every match of the pattern from the skipped one onward is read.
  A page that carries one reading per etymology lists them all this way.
  When `false`, only the first match is read.
  The later matches on a dictionary page belong to inflections and neighbouring headwords.
  A comma or slash inside that one match still yields each alternative.
