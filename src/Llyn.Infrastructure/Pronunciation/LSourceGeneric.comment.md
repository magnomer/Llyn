# LSourceGeneric.cs

## `public sealed class LSourceGeneric : LSource`

The language-agnostic source runner.
Given a language pack's `LSourceSpec`, it executes the source purely from that data.
It tries each attempt in order.
It fetches the attempt's URLs resiliently (see `LSourceReader`).
It runs every reading the attempt declares over that one fetched body.
Each reading extracts with its own declared strategy and tags its value with a variety.
That is a regex capture, a JSON path, or the tolerant nested-span IPA reader.
A reading takes the first match of its pattern.
The later matches on a dictionary page belong to other headwords.
A reading marked `every` takes every match, so a page listing one reading per etymology yields them all.
A reading's skip count passes over that many earlier matches.
It holds no knowledge of any particular source, dictionary, or language.
So a new ordinary source needs only a `source.json` entry and no code.
It answers with an `LAnswer`.
A source that was fetched and had nothing is told apart from one never reached.
An attempt whose page loaded is an answer even when nothing was extracted from it.
Only a source whose every attempt failed to fetch is reported as never reached.

## `public async Task<LAnswer> LSourceFind(string word, CancellationToken cancellation)`

Merges readings across attempts, keeping every value the first attempt to answer for a variety gave.
A later attempt adds nothing to a variety an earlier one filled, so a fallback never doubles a reading.
An untagged reading counts as a variety of its own, so a flat attempt still answers once.
The loop stops once every variety the spec declares is filled, so a later attempt is not fetched for nothing.
Readings come back in the order they were first seen, which keeps the pack's declared order on the menu.
The headword an attempt followed to carries over, so later attempts ask for the page it landed on.

## `private string LSourceSpellingResolve(string word)`

Recasts the headword into the spelling the pack's sources key on.
Each `spelling` rule of the pack runs in written order on the previous rule's output.
A Latin pack drops the macrons here, so a headword typed with them still reaches the bare-spelled page.
The recast word fills `{word}` in every URL and match pattern of every attempt.
A headword a page followed to is taken as the page wrote it, so the rules run once at entry.
A rule set that empties the word is ignored, so a bad rule never sends a blank request.

## `private async Task<(LAnswer, string)> LSourceAttemptResolve(LSourceAttempt attempt, string word, CancellationToken cancellation)`

Runs one attempt and follows the pointer it captures.
Each hop fetches the attempt again for the headword the pointer named.
The readings of every page on the way are gathered.
So a character that is both a word and the simplified form of another shows both sets.
A reading already gathered under the same variety is not added twice.
At most two hops are taken.
A headword already visited ends the chase, so two pages pointing at each other cannot loop.
The answer counts as reached when any page on the way loaded.
It comes back with the headword the chase ended on.

## `private async Task<(LAnswer, string?)> LSourceAttemptRun(LSourceAttempt attempt, string word, CancellationToken cancellation)`

Fetches and extracts once.
The second value is the headword the attempt's `follow` reading captured, or `null` when there is none to follow.
A page the guard rejects is not followed.

A pattern that runs past its patience is read as a page with nothing on it.
Every pattern comes from a language pack and runs over a page a stranger wrote.
Either can make a backtracking pattern spin for minutes, and one lookup must never hold the program that long.

## `private static (LAnswer, string?) LSourceBodyRead(LSourceAttempt attempt, string body, string word)`

Runs the guard, every reading and the follow reading over one fetched page, under the patience the runner sets.

## `private static LSourceReading LSourcePatternResolve(LSourceReading reading, string word)`

Puts the headword into a pattern that names `{word}`, escaped so it matches literally.
It lets a search result be narrowed to the file that carries exactly this headword.
A pattern without the token is returned as it is.

## `private static string? LSourceValueRead(LSourceReading reading, string body)`

The first value a reading yields, which is what the follow reading wants: one headword to go to.

## `private static IReadOnlyList<string> LSourceValueScan(LSourceReading reading, string body)`

Dispatches one reading to its declared strategy and normalizes the texts it captured.
A reading marked `every` keeps every captured text, and any other keeps the first alone.
A dictionary page prints the headword first and its inflections and neighbours after it.
So the first match is the headword's reading, and the later ones are not.
A page carrying one reading per etymology is the case `every` exists for.
The follow reading and the ordinary readings share it, so a pointer is captured with the same three strategies.

## `private static IReadOnlyList<string> LSourcePieceScan(LSourceReading reading, string text)`

Splits one captured phonetic text at each comma or slash.
A source writes its alternatives that way inside one element.
An address or a bare spelling is not split, since a comma or slash is part of it.

## `private static string? LSourcePathRead(LSourceReading reading, string body)`

Walks the reading's dot path down a JSON body and returns the string it lands on.
A body that is not JSON, a missing property or a non-string leaf all read as nothing.

## `private static IReadOnlyList<string> LSourceGroupScan(LSourceReading reading, string text)`

The capture group of every match of the pattern from the skipped one onward.
Whether the first alone or all of them are kept is decided afterwards by `LSourceValueScan`.
A json value may still need a regex to pick the phonetic out of surrounding wikitext.

## `private static string? LSourceNormalize(LSourceReading reading, string captured)`

Strips a phonetic value of its delimiters or a plain value of its surrounding space.
An empty result reads as nothing.

## `private static HashSet<string> LSourceVarietyScan(LSourceSpec spec)`

Collects the distinct variety tags across all of the spec's readings, computed once at construction.
