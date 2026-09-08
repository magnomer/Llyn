# LSourceGeneric.cs

## `public sealed class LSourceGeneric : LSource`

The language-agnostic source runner.
Given a language pack's `LSourceSpec`, it executes the source purely from that data.
It tries each attempt in order.
It fetches the attempt's URLs resiliently (see `LSourceReader`).
It extracts the value with the attempt's declared strategy.
That is a regex capture, a JSON path, or the tolerant nested-span IPA reader.
It holds no knowledge of any particular source, dictionary, or language.
So a new ordinary source needs only a `source.json` entry and no code.
It answers with an `LAnswer`.
A source that was fetched and had nothing is told apart from one never reached.
An attempt whose page loaded is an answer even when nothing was extracted from it.
Only a source whose every attempt failed to fetch is reported as never reached.

## Inline notes

### `string? captured = string.IsNullOrEmpty(attempt.LSourceAttemptPattern)`

A json value may still need a regex to pick the phonetic out of surrounding wikitext.
