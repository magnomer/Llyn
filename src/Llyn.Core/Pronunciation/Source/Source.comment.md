# Source

This folder holds the contracts and data for pronunciation and audio sources.
A source is a language-agnostic runner.
What it searches and how it extracts a value come from the language pack, so nothing is hardcoded here.
The folder also holds the streamed step records a lookup reports through.

## `LSource`

A single pronunciation or audio source.
Which of the two it is comes from the list the language pack declared it in.
It does not come from the source itself.
It returns an `LAnswer`, which carries the extracted value and whether the source was reached.

## `LSourceFind`

Finds a value for a word.
Implementations must not throw for an ordinary "not found" or network failure.
They report it in the answer, so one failing source never fails the whole search.
`LAnswerBlank` means the source answered and had nothing, and `LAnswerLost` that it was never reached.

## `LSourceFactory`

The port that turns declared source specs into live sources the engine can query.
It is a factory over the network, not storage, so it is not a Vault.
`LSourceFactoryHttp` in Infrastructure is its adapter over the shared `HttpClient`.
The engine caches the sets it builds per language and scheme and never names a client.
It creates one live source per spec, in the order given.

## `LSourceSpec`

The language-pack definition of one source, loaded from `languages/<Lang>/source.json`.
It gives the name and the ordered extraction attempts.
The list it is declared in decides what it is for, so it carries no kind of its own.
The generic source runner turns it into a live `LSource`, so an ordinary source needs no code.
The attempts are tried in order until one yields a value.
The spelling holds the pack's ordered headword rewrite rules, empty when the pack declares none.
A frequency source also carries its bands and the figures that turn its raw value into a word interval.
The bands label a raw figure the shared ladder cannot grade, and are empty outside a frequency source.
The total is the corpus size a raw count or per-million figure is measured against.
The interval is then total over raw.
The factor scales a raw rank or class to estimate the interval.
The base is exponentiated by a raw class before the factor applies.
Without a base the raw figure multiplies directly.
The unit is the word printed before a bare numeric answer in the tooltip, such as `Level`.

## `LSourceAttempt`

One fetch attempt within a source, declared by a language pack.
A source tries its attempts in order and keeps the first that yields a non-empty answer.
An attempt fetches each of its URLs in turn, through a resilient reader with transient retry.
It then runs every reading it declares over the fetched body.
Each `{word}` in a URL template is replaced with the URL-escaped headword.
A source that lists one form declares one reading with an empty tag.
The guard is an optional regex that must match the body for the attempt to count.
It guards against pages served for an unrelated word.
Headers go with every URL of the attempt, since some sources reject requests without them.
Naver, for example, needs a `Referer`.
They stay in the pack as data, so a new source's headers need no code.
The prefix is prepended to a relative extracted value, as with Cambridge audio.
The follow reading names another headword whose page is read as well.
A simplified Chinese page only points at its traditional form, and the pointer is read from the page itself.
The decoded flag HTML-decodes the body before the guard and the readings run.
A site that writes every CJK character as a numeric entity is then matched on its decoded text.
So a pack pattern can name the character itself.

## `LSourceReading`

One extraction a source attempt applies to a fetched page, declared by a language pack.
An attempt fetches its page once and runs every reading it declares over the same body.
Each reading that matches yields one `LReading` tagged with its variety.
A page listing two varieties is read twice, once per reading, without a second fetch.
The variety is empty when the reading yields an untagged form.
The strategy `regex` takes a capture group on the raw body.
The strategy `json` follows the path, then optionally a regex.
The strategy `span` is the tolerant nested-span IPA reader.
The pattern is applied to the body, or to the JSON value for `json`.
It is null when the strategy needs none.
The group is the capture group to read, and zero is the whole match.
The path is dot-separated, and each segment is a literal property name.
The phonetic flag decodes entities and strips enclosing IPA delimiters, and stays false for audio URLs.
The skip passes over that many earlier matches, so one page can yield a second variety from its second span.
With every set, all matches from the skipped one onward are read.
A page that carries one reading per etymology lists them all this way.
Without it only the first match is read.
The later matches on a dictionary page belong to inflections and neighbouring headwords.
A comma or slash inside that one match still yields each alternative.

## `LReceiver`

Subscriber contract for a pronunciation lookup.
The requester implements this to receive streamed results.
Each source reports when it starts, and each reports exactly one candidate when it is done.
The lookup reports once when all sources have finished.
So a menu can stand a row per source from the first moment and resolve each in place.
Callbacks may arrive on background threads.
An implementation that touches UI is responsible for marshalling.

## `LReceiverSourceStart`

A source has begun searching.
The order is its position in the language pack's list.
The position comes with the start, so a row can be placed before any answer exists to place it by.

## `LReceiverCandidateAdd`

A source has finished and this is what it had.
It arrives whether the source found a transcription, had none, or could not be reached.

## `LReceiverLookupFinish`

Every source has finished.
No further callbacks follow.

## `LLookupKind`

Which moment of a pronunciation lookup one step reports.
A lookup is a sequence of steps, one source step per source, one candidate per source, and one end.
The source step means a source has begun searching.
The candidate step means a source has finished and carries what it had.
The end step means every source has finished and no further step follows.

## `LLookupStep`

One step of a pronunciation or transcription lookup, carried to the caller as data.
The lookup streams a source step per source, a candidate per source, and one end, in that order.
The position comes with the source step, so a menu row can be placed before any answer exists.
A source step and the end step carry no candidate.
The end step names no source and carries order zero.
Steps may be produced on background threads, so a caller that touches UI marshals on its own side.
The shell receives steps through a delegate, so no surface implements a contract to hear them.

## `LLookupStepEnded`

Whether this is the end step, answered here so the shell asks and never compares the kind itself.
A shell tells the three kinds apart by this verdict and by whether the candidate is present.

## `LHarvestKind`

Which moment of an audio-recording discovery one step reports.
A discovery is a sequence of steps, one source step per source, one recording per source, and one end.
The three kinds mean a source began, a source finished with what it had, and every source finished.

## `LHarvestStep`

One step of an audio-recording discovery, carried to the caller as data.
The discovery streams a source step per source, a recording per source, and one end, in that order.
A source step and the end step carry no recording.
The end step names no source and carries order zero.
Steps may be produced on background threads, so a caller that touches UI marshals on its own side.
The shell receives steps through a delegate, so no surface implements a contract to hear them.

## `LHarvestStepEnded`

Whether this is the end step, answered here so the shell asks and never compares the kind itself.
A shell tells the three kinds apart by this verdict and by whether the recording is present.

## Stamp

Commit: `c2f335c331fe`
- `LHarvestKind.cs` `89bb6b4933ceb2c5`
- `LHarvestStep.cs` `3dac0b73825c7f3b`
- `LLookupKind.cs` `ac2d9c5d226028ec`
- `LLookupStep.cs` `e346977745adccf4`
- `LReceiver.cs` `7e3f3275d0559f71`
- `LSource.cs` `50be833c18cceb0e`
- `LSourceAttempt.cs` `9055bc342c57fcb2`
- `LSourceFactory.cs` `addbe5c215fc30e4`
- `LSourceReading.cs` `471d3bde8bc22fd4`
- `LSourceSpec.cs` `df31fffb637e5565`
