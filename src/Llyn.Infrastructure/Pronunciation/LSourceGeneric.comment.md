# LSourceGeneric.cs

## `public sealed class LSourceGeneric : LSource`

The language-agnostic source runner. Given a language pack's `LSourceSpec`, it executes the source purely from that data: it tries each attempt in order, fetches the attempt's URLs resiliently (see `LSourceReader`), and extracts the value with the attempt's declared strategy — a regex capture, a JSON path, or the tolerant nested-span IPA reader. It holds no knowledge of any particular source, dictionary, or language, so a new ordinary source needs only a `source.json` entry and no code.

## Inline notes

### `string? captured = string.IsNullOrEmpty(attempt.LSourceAttemptPattern)`

A json value may still need a regex to pick the phonetic out of surrounding wikitext.
