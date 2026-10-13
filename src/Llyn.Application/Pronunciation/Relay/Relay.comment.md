# Relay

The lookup and harvest orchestrators, their subscriber contracts and the relays that turn calls into steps.
They stand apart from the clerks because they hold no store and no pending state.
They only fan a request out to ready-built sources and stream the answers.
Sources are supplied language-agnostic (see `LSource`), so no orchestrator knows any particular source or language.

## `LLookup`

It fans a pronunciation request out to every source at once and streams each result to the receiver.
The caller hands it the transcription sources alone, and it runs the whole set.
Each candidate carries its source's position in that set, so a slow source keeps its declared place.
One slow source never blocks the others, and one failing source never stops them running.
A source that answered produces one candidate per reading, all sharing the source's position.
Each candidate carries its reading's variety.
The readings pass through `LReading.LReadingScan` first, so an untagged reading fans out over the pack's declared varieties.
Each fanned-out reading is then normalized and run through the pack's cleanup groups.
The fan-out comes first so a cleanup group scoped to one variety sees the variety it targets.
Cleanup has no switch except a literal lookup, so every other candidate carries cleaned text.
The returned set therefore holds cleaned text too, and a cache built on it never needs a second fetch.
The user's optional respelling is not applied here, because the cache must stay free of it.
A literal lookup skips normalization and cleanup and keeps each reading as written, apart from trimming.
That is the transcription path, where a scheme such as Pinyin keeps its spaces and knows no IPA cleanup.
A source that had nothing or was never reached still produces one candidate with no phonetic.
So the menu can name a broken source rather than leaving its row silently absent.
The returned set is in source order by stable sort, so readings keep their order inside a source.
Collecting the set here costs nothing, since every candidate already passes through this one place.
The lookup reports complete once all sources have finished.

### `LSeekerStart`

It starts every source at once and waits for all of them.
A cancelled lookup withholds `LReceiverLookupFinish`, since the menu it would close is already gone.
Every other ending reports it, a source failure included.
The lookup started the receiver, so it alone ends it, and the caller never reports the end again.

### `LLookupSourceRun`

It announces the source first, so the menu can show the row while the answer is pending.
It adds the candidates to `found` under a lock before streaming them.

### `LLookupAnswerRead`

Each source runs under its own minute-long deadline, linked to the caller's cancellation.
A host that neither answers nor refuses is written off at that deadline instead of holding the lookup open.
The deadline is per source, so one dead host never eats another's budget.
The caller's own cancellation escapes, because that ends the lookup rather than one source.
Only the deadline becomes an answer saying the source was never reached.
Any other failure propagates, so the caller records it instead of hiding it as a lost answer.
Reading the deadline off the outer token instead would have failed the whole search over one dead host.

## `LHarvest`

It is the download counterpart to `LLookup`, fanning an audio request out to every source it was given.
The caller hands it the recording sources alone.
Each recording carries its source's position, so a slow source keeps its declared place.
One slow source never blocks the others, and one failing source never stops them running.
A source that answered produces one recording per reading, all sharing the source's position and carrying the reading's variety.
The readings pass through `LReading.LReadingScan` first, so an untagged recording fans out over the pack's varieties.
A source with nothing, never reached, or with nothing in the asked variety still streams one addressless recording.
So the menu can name a broken source rather than leaving its row silently absent.
The discovery reports complete once all sources have finished.
The returned set is ordered by source position with a stable sort.
It is never narrowed, so a caller can keep it and narrow it again for another row.

### `LHarvestStart`

The `variety` is that of the row that opened the menu, or empty when that row has none.
Empty streams every variety, so the menu shows each recording under its own flag.
A named variety streams that variety only, so the menu offers what fits the row.
The returned set holds every variety either way.
The harvest alone ends the listener, once, whether the sources succeed or one throws.
A cancelled harvest withholds `LListenerFinish`, since the menu it would close is already gone.

### `LHarvestSourceRun`

It announces the source first, so the menu can show the row while the answer is pending.
It adds every recording to `found` under a lock, then streams the narrowed ones.
A source cancelled after its answer streams nothing.

### `LHarvestRecordingScan`

It narrows already fanned-out recordings to the one variety asked for.
An empty variety returns the recordings untouched.
Otherwise a recording survives only when its tag equals the variety ordinally.
The fan-out ran first, so an untagged reading already became one recording per declared variety.
A recording tagged with another variety is dropped, since the row it would fill belongs elsewhere.
A source left with nothing gets one addressless recording under its position, reached as its answer was.
The recordings are walked in source position runs, which is how the harvest returns them.

### `LHarvestAnswerRead`

It is the recording counterpart of `LLookupAnswerRead`.
Only the deadline becomes a lost answer.
Any other failure propagates to the caller instead of hiding as one.

## `LListener`

The subscriber contract for an audio-recording discovery, mirroring `LReceiver`.
Each source reports when it starts, then reports its recordings when it is done.
The discovery reports once when all sources have finished.
Callbacks may arrive on background threads.
An implementation that touches UI is responsible for marshalling.

### `LListenerRecordingAdd`

It arrives whether the source offered a recording, had none, or could not be reached.

### `LListenerFinish`

Every audio source has finished.
No further callbacks follow.

## `LListenerRelay`

It turns the three listener calls into a stream of harvest steps handed to one delegate.
The harvest keeps its contract, and the caller keeps a delegate that crosses any depth as data.
So the shell never implements the contract, and the engine names no shell type to reach it.
Each call becomes exactly one step, on the thread that made the call.
The sink is required, since a relay with nowhere to send is a caller mistake.

### `LListenerRelayEnded`

It tells whether this relay has sent its end step.
The tenure that made the relay reads it after a failed search.
It ends the sink itself only when the harvest never did.

### `LListenerFinish` step

The end step names no source and carries order zero.
It marks the relay ended before the step leaves.
So a sink that throws on the end is never ended twice.

## `LReceiverRelay`

It does for `LReceiver` what `LListenerRelay` does for `LListener`, producing lookup steps.
Each call becomes exactly one step, on the thread that made the call.
A respelling wrapper sits in front of it like any receiver, so cached and live lookups respell alike.
The sink is required, since a relay with nowhere to send is a caller mistake.

### `LReceiverRelayEnded`

It tells whether this relay has sent its end step, through a respelling wrapper or directly.
The tenure that made the relay reads it after a failed lookup.
It ends the sink itself only when the lookup never did.

### `LReceiverLookupFinish`

The end step names no source and carries order zero.
It marks the relay ended before the step leaves.
So a sink that throws on the end is never ended twice.

## `LReceiverRespelling`

It wraps a receiver so every candidate reaching it carries its phonetic recast through the pack's respelling groups.
The source start and the finish are forwarded untouched, because respelling adds only a text.
A candidate with a phonetic is copied with the recast text in its respelling and every other field kept.
The original phonetic stays as it came, so the menu shows either form and the pick stores both.
A candidate without a phonetic is forwarded as it is, so a broken source still names its row.
The group list may be empty, and the wrapper then copies the phonetic into the respelling unchanged.
The engine wraps whenever the pack declares groups, whatever the user's switch says.
The engine wraps the receiver and never the cache, so a cached lookup replays through this wrapper too.
Both constructor arguments are required, since a wrapper with nothing to forward to is a caller mistake.

## Inline notes

### LLookup.cs: cancellation.ThrowIfCancellationRequested()

A source that answered after the user closed the menu reports nothing.
The row it would fill is already gone.

## Stamp

Commit: `c2f335c331fe`
- `LHarvest.cs` `626d00a270675e5e`
- `LListener.cs` `1879e19b98fa48ed`
- `LListenerRelay.cs` `fc5a424248587ed6`
- `LLookup.cs` `75ae370208c8aaae`
- `LReceiverRelay.cs` `8154e1068d60cb50`
- `LReceiverRespelling.cs` `41bef12d66b6083c`
