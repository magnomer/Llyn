# LSourceGeneric.cs

## `public sealed class LSourceGeneric : LSource`

The language-agnostic source runner.
Given a language pack's `LSourceSpec`, it executes the source purely from that data.
It tries each attempt in order.
It fetches the attempt's URLs resiliently (see `LSourceReader`).
It runs every reading the attempt declares over that one fetched body.
Each reading extracts with its own declared strategy and tags its value with a variety.
That is a regex capture, a JSON path, or the tolerant nested-span IPA reader.
A reading's skip count passes over that many earlier matches.
So one page yields a second variety from a later span.
It holds no knowledge of any particular source, dictionary, or language.
So a new ordinary source needs only a `source.json` entry and no code.
It answers with an `LAnswer`.
A source that was fetched and had nothing is told apart from one never reached.
An attempt whose page loaded is an answer even when nothing was extracted from it.
Only a source whose every attempt failed to fetch is reported as never reached.

## `public async Task<LAnswer> LSourceFind(string word, CancellationToken cancellation)`

Merges readings across attempts, keeping the first value seen for each variety.
An untagged reading counts as a variety of its own, so a flat attempt still answers once.
The loop stops once every variety the spec declares is filled, so a later attempt is not fetched for nothing.
Readings come back in the order they were first seen, which keeps the pack's declared order on the menu.

## `private static HashSet<string> LSourceVarietyScan(LSourceSpec spec)`

Collects the distinct variety tags across all of the spec's readings, computed once at construction.

## Inline notes

### `string? captured = string.IsNullOrEmpty(reading.LSourceReadingPattern)`

A json value may still need a regex to pick the phonetic out of surrounding wikitext.
