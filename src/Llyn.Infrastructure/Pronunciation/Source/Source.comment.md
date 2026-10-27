# Source

The web adapters that fetch pronunciation, phonetic series, reflex, script and recording data for the pronunciation system.
They stand apart from the Core ports they implement because they alone touch HTTP and the file system.
Nothing here knows a site, a dictionary or a language.
Every address, form, pattern and format comes from a language pack.
Each adapter binds to the one shared `HttpClient` the rig builds it over.
Each lookup answers a result together with a reached flag.
A page that answered with nothing is told apart from a host never reached.
Only the second is retried later.

## `LSourceFactoryHttp`

The adapter behind the source factory port, building live `LSource` sets over one shared client.
It is called once per list, so a transcription set and a recording set never mix.
Every ordinary source becomes a data-driven `LSourceGeneric`.
This is the seam where a source needing logic a pack cannot express would go to a hand-written handler.
The orchestrators receive the finished set and stay language-agnostic.
`LSourceFactoryCreate` returns one live source per declared spec, in pack order.

## `LSourceGeneric`

The language-agnostic source runner.
It executes a source purely from its `LSourceSpec`.
So a new ordinary source needs only a `source.json` entry and no code.
It tries each attempt in order and fetches the attempt's URLs through `LSourceReader`.
It runs every reading the attempt declares over that one fetched body.
Each reading extracts by its own strategy and tags its value with a variety.
The strategies are a regex capture, a JSON path, the tolerant nested-span IPA reader, or the fetched address.
A reading takes the first match of its pattern.
The later matches on a dictionary page belong to other headwords.
A reading marked `every` takes every match, so a page listing one reading per etymology yields them all.
A reading's skip count passes over that many earlier matches.
An attempt whose page loaded is an answer even when nothing was extracted.
Only a source whose every attempt failed to fetch is reported as never reached.

### `LSourceFind`

Merges readings across attempts, keeping every value the first attempt to answer for a variety gave.
A later attempt adds nothing to a variety an earlier one filled, so a fallback never doubles a reading.
An untagged reading counts as a variety of its own, so a flat attempt still answers once.
The loop stops once every variety the spec declares is filled.
Readings come back in first-seen order, which keeps the pack's declared order on the menu.
The headword an attempt followed to carries over, so later attempts ask for the page it landed on.

### `LSourceSpellingResolve`

Recasts the headword into the spelling the pack's sources key on.
Each `spelling` rule runs in written order on the previous rule's output.
A Latin pack drops macrons here, so a headword typed with them still reaches the bare-spelled page.
The recast word fills `{word}` in every URL and match pattern.
The headword as typed fills `{headword}` in a URL, for a service that needs the dropped marks.
A headword a page followed to is taken as the page wrote it, so the rules run once at entry.
A rule set that empties the word is ignored, so a bad rule never sends a blank request.

### `LSourceAttemptResolve`

Runs one attempt and follows the pointer it captures.
Each hop fetches the attempt again for the headword the pointer named.
The readings of every page on the way are gathered.
So a character that is both a word and the simplified form of another shows both sets.
A reading already gathered under the same variety is not added twice.
At most two hops are taken.
A headword already visited ends the chase, so two pages pointing at each other cannot loop.
The answer counts as reached when any page on the way loaded.
It comes back with the headword the chase ended on.

### `LSourceAttemptRun`

Fetches and extracts once.
The address that answered is kept, since a `link` reading takes it as its value.
The second value is the headword the `follow` reading captured, or `null` when there is none.
A page the guard rejects is not followed.
An attempt marked decoded has its body HTML-decoded first, so entity-written characters match as characters.
A pattern that runs past its patience reads as a page with nothing on it.
Patterns come from a pack and run over a page a stranger wrote, so one can spin for minutes.
One lookup must never hold the program that long.

### `LSourceBodyRead`

Runs the guard, every reading and the follow reading over one page, under the runner's patience.
A `link` reading skips the body and answers the address the page was fetched from.
It serves a speech service whose address is itself the recording, once the fetch shows it answers.

### `LSourcePatternResolve`

Puts the headword into a pattern naming `{word}`, escaped so it matches literally.
It lets a search result be narrowed to the file carrying exactly this headword.
A pattern without the token is returned as it is.

### `LSourceValueScan`

Dispatches one reading to its declared strategy and normalizes the texts it captured.
A reading marked `every` keeps every captured text, and any other keeps the first alone.
A dictionary page prints the headword first and its inflections and neighbours after it.
So the first match is the headword's reading.
The follow reading shares this path, so a pointer is captured with the same three strategies.
`LSourceValueRead` returns the first value, which is what the follow reading wants.

### `LSourcePieceScan`

Splits one captured phonetic text at each comma or slash, since a source writes alternatives that way.
An address or a bare spelling is not split, since a comma or slash is part of it.

### `LSourcePathRead`

Walks the reading's dot path down a JSON body and returns the string it lands on.
A body that is not JSON, a missing property or a non-string leaf all read as nothing.

### `LSourceGroupScan`

Returns the capture group of every match from the skipped one onward.
`LSourceValueScan` decides afterwards whether the first alone or all are kept.
A json value may still need a regex to pick the phonetic out of surrounding wikitext.

### `LSourceNormalize`

Strips a phonetic value of its delimiters or a plain value of its surrounding space.
An empty result reads as nothing.

### `LSourceVarietyScan`

Collects the distinct variety tags across all readings, computed once at construction.

## `LSourceReader`

A resilient page reader for HTML pronunciation sources.
It tries each candidate URL in turn.
Each URL is fetched once, with no retry.
A transient failure (timeout, network error, 408, 429, 5xx) counts as lost for that URL.
Other failing responses (403, 404) count as blank, and both fall through to the next URL.
A 404 is an answer, since the site is up and has no such word.
A transient failure is not an answer, and a menu reports that as a failure to retrieve.

### `LSourceReaderRead`

The first URL with a body wins and comes back beside the answer, so the caller knows which one.
When none has a body, the answer is blank if any URL answered at all, and lost otherwise.
So one unreachable mirror does not condemn a source whose other mirror simply had no entry.

## `LPronunciationText`

Extracts and normalizes pronunciation text scraped from HTML.
It is tolerant on purpose.
Given a pattern matching an IPA element's opening tag, it reads the full inner content.
It walks nested spans to the matching close and strips inner markup.
This survives nested markup, where stopping at the first `</span>` would truncate the value.

### `LPronunciationTextScan`

Returns the inner text of every element the pattern opens, from the skipped match onward, in page order.
Each text still carries its delimiters and entities, so the caller splits and normalizes it.
An element whose close is never found, or whose text is blank, is passed over.

### `LPronunciationTextNormalize`

Decodes HTML entities and strips enclosing IPA delimiters and a stray comma.
The caller stores just the phonetic characters, and the UI supplies its own brackets.

### `LPronunciationInnerRead`

Reads a span's inner HTML from just after its opening tag up to the matching `</span>`, counting nested spans.
It returns `null` when the close is never found.

## `LRecordingArchive`

The adapter behind the recording port.
It downloads remote recordings into the workspace and returns their local paths.
A chosen recording is saved under a per-language folder.
A previewed recording is cached under the workspace's temp folder.
The root and client are fixed at construction, so the rig builds a fresh one per workspace.
Saved audio is workspace data, so it lives under the chosen root and never elsewhere.
Temporary files also stay inside the workspace.

### `LRecordingSave`

The file sits under `audio/<language>/` and is named by the headword, the variety and the address.
A recording with no variety leaves that part out, so a pack without varieties reads the same.
A refused or timed-out fetch, or an unwritable folder or file, raises `LVaultFault` with the cause inside.
A cancellation the caller asked for rethrows as it is, and any other fault propagates unwrapped.

### `LRecordingSweep`

Deletes every file under `audio` whose full path is not in `kept`.
A missing folder sweeps nothing.
A file that will not delete is left for the next sweep.

### `LRecordingArchiveRead`

Fetches the bytes of one recording, the path both saving and previewing go through.
A host answering 429 is asked again after a pause, up to three tries in all.
The pause is the host's Retry-After when longer than five seconds, and five seconds otherwise.
It waits through the `LClock` handed to the archive, so a test clock can make it instant.
A host naming a Retry-After over 30 seconds is not waited for, and the fetch fails at once.
Wikimedia's audio host answers 429 to a handful of quick fetches and names one second, which retries proved too short.
Any other failing status raises, so the caller sees a refused fetch rather than an empty file.
The two public callers wrap that raise in `LVaultFault`.

### `LRecordingFaultCheck`

Says whether an exception is an expected outside failure to wrap in `LVaultFault`.
These are a network failure, a file or access failure, and a cancellation the caller did not ask for.
That last is how a timeout arrives.
A cancellation the caller asked for is not wrapped, so it still reads as a cancellation.

### `LRecordingStemRead`

The file stem a saved recording takes, before its extension.
A tagged recording appends the variety after a dot, so `tomato.British` and `tomato.American` sit side by side.
A short digest of the address ends the stem, so `Weg` and `weg` never land on one file.
Windows folds case in file names, and two words differing only by case are still two words.
The digest also keeps two homographs apart when their recordings differ.
One recording fetched twice lands on one file.
Both parts are normalized, so a variety name never carries a path character into the file name.

### `LRecordingDigestRead`

The hex digest of an address, the identity every cached or saved file is named by.

### `LRecordingPrepare`

Downloads a recording to a temporary cache file for immediate playback and returns its path.
Playing a local file is reliable, whereas streaming a remote token-bearing URL through the media stack is not.
The file is named by a digest of the full address, so two sources' same-named files never collide.
A file already in the cache is returned as it stands, without another fetch.
Rewriting it would fail anyway, since the player keeps the file it last played open.
Fetch and file failures raise `LVaultFault`, as `LRecordingSave` raises them.

### `LRecordingExtensionRead`

Reads the extension from the address and keeps it only when it names an audio format.
Anything else falls back to `.mp3`, so a source cannot name a file `.exe` or `.html` on disk.

### `LRecordingArchiveNormalize`

Makes a value safe as one file name segment.
Path characters become underscores.
A name Windows reserves for a device, such as `con` or `nul`, is prefixed.
Otherwise the file could not be created.
The reservation covers the part before the first dot, so the check reads only that far.

## `LReflexSourceHttp`

The adapter behind the `LReflexSource` port.
It asks one rule's site for one character and reads the readings it answers with.
The page is matched by the rule's pattern, and each match becomes draft rows.
The address, the form, the pattern and the format come from the pack.
The rule pattern runs case-blind with dots crossing lines, within the patience handed to the adapter.
The rig passes two seconds to every adapter, through `LRigFactorySource`.

### `LReflexSourceFind`

Fetches the page and reads its rows.
The second value says whether the site was reached and read.
A page never fetched and a busy answer are not reached, so the character is asked again later.
A page reached but matching nothing is reached, and the miss is remembered for the session.
A pack regex that times out is not reached either, since the page was never fully read.

### `LReflexSourceScan`

Reads the rows out of one fetched page.
Each match yields a row whose text is the format filled from the match, with every slash and bracket dropped.
Its kind, romanization, meaning, note and main mark come from their named groups.
Its region is the rule's.
With `split` set the text and romanization are cut into pieces.
Each reading is paired with the romanization at its position when the piece counts agree.
The gloss of each reading supplies its separate note and meaning.
A row repeating the kind, text and romanization of an earlier row is dropped.
A page lists a reading once per section, and one is enough.
A match whose text comes out empty is skipped.
With `every` unset only the first match is kept.
Rows that all carry one text are all marked in common use, since the character is said one way only.
Otherwise, with `first` set and no match capturing `main`, the first row is marked in common use.

### `LReflexRowMatch`

True when two rows carry one kind, one text and one romanization, so the second adds nothing.

### `LReflexDelimiterRemove`

Drops every slash and bracket, so a stored reading is bare.
`LLanguageCache.LLanguageAnatomyResolve` then splits it into onset, vowel and coda.

### `LReflexPieceScan`

Cuts one text on the rule's split pattern into trimmed non-empty pieces.
Without a pattern the text is handed back whole.

### `LReflexGlossRead` and `LReflexGlossFind`

A match capturing a note or meaning hands those texts back directly.
Otherwise the gloss is looked up in the stretch after the match, up to the rule's `until` pattern.
The rule's gloss pattern is filled with the escaped romanization, and its named groups are the answer.
Both are empty without a gloss pattern, without a romanization, or when the stretch holds no such line.

### `LReflexTextResolve`

Runs the rule's rewrites over the formatted text in pack order and trims it.
A rule with no rewrite hands the text back as it came.

### `LReflexRomanizationFormat`

Runs the rule's recasts over one romanization piece in pack order and trims it.
With `superscript` set every ASCII digit left is raised, so `oq7` is stored as `oq⁷`.
The gloss is looked for under the piece as it came, since the page never carries the recast form.

### `LReflexTextFormat` and `LReflexSlotFormat`

Fill the rule's format from the match, leaving a slot that names no group as written.
A `[[...]]` segment is dropped whole when a slot inside it captured nothing, so `롱[[({initial})]]` prints `롱` alone.
A single bracket is literal, so `[{text}]` prints the reading in brackets.
Runs of whitespace collapse to one space, so a multi-line capture prints on one line.

### `LReflexGroupRead`

The plain text of one named group, or empty when the group did not take part.

### `LReflexTextNormalize`

Strips tags, decodes entities, collapses whitespace and composes the text, so a stored reading compares by value.

### `LReflexBodyRead`

Fetches one page with a GET, or a POST when the rule lists form fields, under the rule's headers.
Not found answers an empty page, since the site has no entry and the miss is final.
Any other failure answers `null`, since the site was not reached.

## `LScriptSourceHttp`

The adapter behind the `LScriptSource` port.
It asks one web database for the glyph pictures of one character in one style.
The site offers no query API, so the database is reached by posting its own search form.
The answer is read by the pack's pattern, and every picture it names is fetched as bytes.
It knows only the shape an [LScriptStyle](../../../Llyn.Core/Pronunciation/Script/Script.comment.md) declares.

### `LScriptSourceFind`

Posts the form, reads the pictures and captions the pattern names, and fetches each picture in turn.
A picture that cannot be fetched is skipped, and the positions count only those kept.
The gloss read from the page is repeated on every picture of the style.
A caption's chronology is stored as its own code, so nothing is fetched again when the interface language changes.
The second value says whether the database answered at all.
An answer naming no picture is a miss, while a database that never answered is not.
A page the pattern could not read in time counts as never answered.

### `LScriptBodyRead`

Posts the pack's fields as a URL-encoded form, which sends the character as UTF-8 percent escapes.
The site reads a raw multibyte body as another encoding and finds nothing, so the encoding matters.
A failed request or an error status reads as null.

### `LScriptHitScan`

Yields every match as a resolved picture address, a cleaned caption and the code of its age.
The longest chronology label the pack lists is cut out of the caption here.
The age is matched once and then stored.
A caption whose age the pack does not list keeps it, and the code is empty.
A match whose image group is empty is dropped.

### Helpers

- `LScriptGlossRead` returns the first group of the gloss pattern, cleaned, or empty when none is declared or matched.
- `LScriptGroupRead` returns a numbered group's text, or empty when the number is zero or beyond the match.
- `LScriptAddressResolve` decodes entities, runs the rewrite rules, and prepends the prefix to a relative address.
- `LScriptTextNormalize` turns line breaks into spaces, drops other tags, decodes entities and folds whitespace.
- `LScriptDataRead` fetches one picture's bytes, or null when the request fails, errs or returns nothing.

## `LShengfuSourceHttp`

The adapter that reads a character's phonetic series off the web under a pack rule.
It carries no language knowledge, and the address and regex both come from the rule.
It takes the one shared client, so every fetch keeps the same agent and timeout.

### `LShengfuSourceFind`

Fetches the page of the character and reads the series out of it.
A missing page counts as answered with nothing, so the character is not fetched again.
A refused, busy, unreachable or regex-timed-out page counts as unanswered, so a later display retries it.

### `LShengfuSourceScan`

Runs the rule's pattern over the body and keeps each distinct `shengfu` group in order.
Several series of one character are joined by the rule's separator into one text.
Markup and entities are stripped first, so the text reads as the site printed it.

## Inline notes

### LSourceReader.cs: request.Headers.TryAddWithoutValidation(header.Key, header.Value)

Some headers are restricted, such as Referer.
Validation is skipped so they are sent verbatim.

### LSourceReader.cs: catch (OperationCanceledException)

This is a per-request timeout from `HttpClient.Timeout`, not a user cancellation.
It is retried.

## Stamp

Commit: `9377e657b395`
- `LPronunciationText.cs` `1c97f580d1fbea73`
- `LRecordingArchive.cs` `12f1ccbbedafaaba`
- `LReflexSourceHttp.cs` `7eff40edf76169ca`
- `LScriptSourceHttp.cs` `5de8c0ab0e4ce91b`
- `LShengfuSourceHttp.cs` `b1d791a971da1010`
- `LSourceFactoryHttp.cs` `0e012faa3406448c`
- `LSourceGeneric.cs` `44c2f25ca2150c9d`
- `LSourceReader.cs` `1c84ff8ab48e7f13`
