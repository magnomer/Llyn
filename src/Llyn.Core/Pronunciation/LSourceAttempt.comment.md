# LSourceAttempt.cs

## `public sealed record LSourceAttempt(`

One fetch attempt within a source, declared by a language pack.
A source tries its attempts in order and keeps the first that yields a non-empty answer.
An attempt fetches each of its URLs in turn, through a resilient reader with transient retry.
It then runs every reading it declares over the fetched body.
The readings are held as [LSourceReading](LSourceReading.comment.md) records, one per variety the page lists.

**Parameters**

- `LSourceAttemptUrls` — URL templates to try in order.
  Each `{word}` placeholder is replaced with the URL-escaped headword.
- `LSourceAttemptReadings` — The extractions applied to the body, each tagged with the variety it yields.
  A source that lists one form declares one reading with an empty tag.
- `LSourceAttemptGuard` — Optional regex (with `{word}`) that must match the body for the attempt to count.
  It guards against pages served for an unrelated word.
- `LSourceAttemptHeaders` — Extra request headers sent with every URL of this attempt, or `null` for none.
  Some sources reject requests without them (for example Naver needs a `Referer`).
  Kept in the pack as data so a new source's headers need no code.
- `LSourceAttemptPrefix` — Base URL prepended to a relative extracted value to make it absolute.
  Cambridge audio `src` is site-relative, for example.
  It is `null` when the value is already absolute.
- `LSourceAttemptFollow` — Optional reading-shaped extraction run on the body, naming another headword whose page is read as well.
  The value it captures is another headword the same attempt is fetched again for.
  A simplified Chinese page only points at its traditional form, and the pointer is read from the page itself.
  It is `null` when the source never redirects by page text.
- `LSourceAttemptDecoded` — Whether the body is HTML-decoded before the guard and the readings run.
  A site that writes every CJK character as a numeric entity is then matched on its decoded text.
  So a pack pattern can name the character itself.
