# LSourceAttempt.cs

## `public sealed record LSourceAttempt(`

One extraction attempt within a source, declared by a language pack.
A source tries its attempts in order and keeps the first non-empty value.
An attempt fetches each of its URLs in turn, through a resilient reader with transient retry.
It applies a single extraction strategy to the response.

**Parameters**

- `LSourceAttemptUrls` — URL templates to try in order.
  Each `{word}` placeholder is replaced with the URL-escaped headword.
- `LSourceAttemptStrategy` — How to extract the value.
  `"regex"` takes a capture group on the raw body.
  `"json"` follows `LSourceAttemptPath`, then optionally a regex.
  `"span"` is the tolerant nested-span IPA reader.
- `LSourceAttemptPattern` — The regex applied to the body (regex/span) or to the JSON value (json).
  `null` when the strategy needs none.
- `LSourceAttemptGroup` — The regex capture group to read, and `0` for the whole match.
- `LSourceAttemptPath` — Dot-separated JSON path for the `"json"` strategy.
  Each segment is a literal property name.
  `null` for other strategies.
- `LSourceAttemptGuard` — Optional regex (with `{word}`) that must match the body for the attempt to count.
  It guards against pages served for an unrelated word.
- `LSourceAttemptPhonetic` — When `true`, the value is a phonetic form: decode entities and strip enclosing IPA delimiters.
  Left `false` for audio URLs.
- `LSourceAttemptHeaders` — Extra request headers sent with every URL of this attempt, or `null` for none.
  Some sources reject requests without them (for example Naver needs a `Referer`).
  Kept in the pack as data so a new source's headers need no code.
- `LSourceAttemptPrefix` — Base URL prepended to a relative extracted value to make it absolute.
  Cambridge audio `src` is site-relative, for example.
  It is `null` when the value is already absolute.
