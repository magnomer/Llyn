# Lookup

This folder holds the pronunciation lookup and source relays.
It also holds the stubs and handlers that answer and record a search.

## TInterfacePronunciation

The relays for the pronunciation lookup seams below the engine.
That is the reading, answer, candidate and variety records, the respelling rules, and the lookup fan-out.
The recording records, the harvest fan-out, the workspace download, and the engine discovery are relayed here too.
The language pack loader, the glyph split and the engine's language reads are relayed here too.
The anatomy, descent and tone contour reads are relayed here too.
The reflex guise with an optional shown text and the shown-form rule are relayed here too.
The stem key scan and the shengfu record are relayed here too.
The frequency records, the gauge, and the once and band resolves are relayed here too.
The two short fanqie row builders sit here, and the full one sits in `TInterfaceFact` in the Language folder.
Each relay is transparent.

Related relays live in other files.
`TInterfaceFanqie`, in the Entry folder, holds the anchor, diwei and hypothesis relays.
`TInterfaceSource`, in this folder, holds the source specs and the page sources over a client.
`TInterfaceTrove`, in the Tenure folder, holds the per-session trove.

## `TLookupCreate`

Returns the lookup as its interface, so a test never names the application type.
The varieties default to none, so a test without a pack behaves like a language without varieties.
The cleanups default to none too, so a test without a pack sees only the built-in normalization.
The literal flag defaults to false.
The harvest builder is the recording counterpart and defaults its varieties the same way.

## `TReceiverRespellingCreate`

Returns the respelling wrapper as the receiver interface, so a test drives it as the engine would.
The relay creators let a test push one callback at a time through that interface.
The step relay lets a test check each callback becomes one step.

## `TEngineRecordingFind`

Relays the engine-level discovery, so a test can see the draft's row variety narrow the search.
It wraps the sink in a fresh relay, as the tenure does.
The transcription find wraps its sink the same way.

## `TFrequencyGaugeCreate`

Builds a frequency gauge with any band, so a fact can hand Conduct a band the engine never counts.
The rank, the spare stars and the ranked flag stay the gauge's own reads of that band.

## `TContourCreate`

Builds one contour syllable with any levels, so a fact can hand Conduct levels off the scale.
Its tone is the syllable's own read, so it is toned whenever the levels are not empty.

## `TFanqieRowCreate`

The first overload builds a row carrying only the parts the hypothesis reads.
The character, book, position and text are fixed, because the resolve never looks at them.
The second overload builds a row with a chosen character and position.
It serves a test that saves rows into a fanqie store.
The book and text are fixed, the heading repeats the rime, and the row is never rounded.

## `TReflexOrderCreate`

Builds a declared reflex order directly, so a test needs no language pack to check it.

## `TTallyLineScan`

Relays the tally line scan, so a test pins its language order against the reflex list.

## `TFrequencySort`

Relays the frequency order, the pack's declared sources first and the rest by code point.

## `TGlyphOrderSort`

Relays the character order by code point.
Beyond the basic plane it parts from UTF-16 ordinal order, which is why a test pins it.

## `TInterfaceSource`

The relays for the pronunciation sources.
That is the source spec records, the generic source over a client, and its find.
The reflex, shengfu, script and fanqie page sources are relayed here too.
Each relay is transparent.
The source builder returns the source as its interface, so a test never names the infrastructure type.
The reflex find reads one character through the page source with a bare rule holding only the given pattern.
The shengfu, script and fanqie finds do the same for their sources.

## `TListenerStub`

A listener that records the sources started, the recordings added, and how often the discovery finished.
The lists are locked because the harvest reports from several sources at once.

## `TReceiverStub`

A receiver that records the sources started, the candidates added, and how often the lookup finished.
The lists are locked because the lookup reports from several sources at once.

## `TPronunciationHelper`

The fixtures a pronunciation lookup test builds from.
A client over a stub handler stands in for the network.
A failing client throws the given exception on every request.
A gated client holds its one answer until the gate task completes.
A reading and an attempt are built with the fields a test names and defaults for the rest.
A follow attempt takes its pointer reading first and its ordinary readings after it.
A source stub answers with fixed readings and a receiver stub records what it was told.
A lost source stub answers as a host that was never reached.
A listener stub is the recording counterpart of the receiver stub.

## `TPronunciationHelperUrl`

The one address every stub attempt fetches.
A single-body handler ignores it, so such a test never depends on the word being escaped.
A page-map handler answers by the escaped address, so its keys spell the word escaped.

## `TSourceHandler`

A message handler that answers every request with one body and one status.
It stands in for the network so a source test runs offline.
Built from a page map instead, it answers each absolute URL with its own body.
Any other URL gets a 404.
That shape lets a follow test serve the pointing page and the page pointed at.
A failing status exercises the reader's retry and lost paths without a server.
Given a gate task, it holds every answer until that task completes.
That lets an engine test act between a fill starting and its answer landing.
Built from an exception instead, it throws that exception on every request, as a client timeout does.
It counts every request it sees, so a test can tell one fetch from a repeat.

## `TSourceStub`

A source that answers every word with the same fixed answer.
It lets the lookup fan-out be tested without a client or a language pack.

## Stamp

Commit: `c2f335c331fe`
- `TInterfacePronunciation.cs` `1906fb6ccbfc5af2`
- `TInterfaceSource.cs` `cdac272edbe526b4`
- `TListenerStub.cs` `cd67f626a506e3b1`
- `TPronunciationHelper.cs` `5e32f29b42e25acc`
- `TReceiverStub.cs` `5dcf9eb20ede4012`
- `TSourceHandler.cs` `4d231fb35dd51a79`
- `TSourceStub.cs` `773267ea76963972`
