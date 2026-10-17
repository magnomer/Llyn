# Tenure

This folder owns draft editing lifetimes, request delivery, state readings, observers and searches.
A tenure carries per-draft state rather than belonging to a stateless facade.
The vista, posture and trove keep per-tab, per-session and per-draft engine state.

## `LTenure`

Requests share a queue for deferred typing and immediate edits.
The chronicle clerk owns deferred requests and their wait, while the tenure decides when to flush or end editing.
The queue is built before the gauge because the gauge's initial reading asks the queue for its state.
The queue observer tolerates construction before the gauge is assigned.

The turn serializes application, restoration, preparation and commit.
The gate protects lifetime checks and queue state within those turns.
Immediate edits flush pending typing first, so an earlier typed change is not applied after a deliberate pick.
Readiness and change checks also persist pending typing before reading the gauge.

Cancel closes the queue before discarding the draft, so a failed discard cannot leave the tenure accepting edits.
A discard failure, a refusal included, reaches the caller.
The gauge is raised in `finally`, including when discard throws.

Finish without storage cancels without applying pending requests.
A storage attempt dispatches them first and rethrows any retained queue fault with its original stack.
An unchanged draft cancels instead of committing.
An accepted unreadable-value seam sweeps and retries with a seam that cannot accept another retry.
A declined seam rethrows the commit's own refusal with its stack.
Searches stop and observers detach only after a successful commit or during cancellation.

Entry preparation repeats completion requests for at most three rounds.
Other subjects bypass that completion.
The herald suppresses the held draft's notices during preparation because the return value supplies the current draft.
Preparation cleanup runs even if its action fails.

Stored identity reads do not persist pending edits.
The read answers null for an ended tenure or a stale draft id, and other read failures propagate.

## `LTenureQueue`

The queue retains the chronicle clerk obtained at construction rather than resolving it again after a workspace change.
Its live state requires both an unended lifetime and no retained fault.

Deferred dispatch takes requests and checks live state in one gate hold.
Application happens after releasing that gate, while the caller holds the turn.
Refused requests do not stop later requests in the batch.
Each refused request raises a draft bulletin carrying its refusal, so observers reread the draft.
Other failures retain the first fault and stop application.
Suspension also drops pending requests at the chronicle clerk.

Undo and redo share restoration, which dispatches waiting requests before stepping the chronicle.
A halted or ended queue performs no restoration.

## `LTenureGauge`

The gauge compares a whole state with the last announced state rather than announcing every read.
Fixed ended and failed-check states avoid asking draft checks for answers they cannot supply.
Even an ended read first takes the hearth gate to read the engine revision.

A live, unhalted state is reused only while the engine revision matches.
The revision is captured before computing, so a concurrent revision change prevents reuse on the next reading.
A halted state is neither stored nor reused.
It still checks draft changes, but does not ask for undo or redo availability.
A failed check suspends the queue and answers the fixed halted state.

The storable property reads state without dispatching waiting requests.
This keeps a state inquiry separate from persistence.

## `LTenureHerald`

The observer roster and kept draft share one bulletin handler.
A draft bulletin clears the kept draft before observer delivery.
Roster snapshots are taken under the tenure gate and dispatched outside it.
An ended queue receives no delivery and accepts no new observers.

Preparation suppresses only draft bulletins naming the held draft.
Finishing preparation clears the kept draft and advances its round.
A draft read racing a round change is returned but not cached.
This prevents later Mention checks from reusing that raced read.
The kept-draft read answers null for a stale draft id.
Cancel and successful finish detach the hearth subscription and clear the roster.

## `LErrand`

Recording and transcription searches have separate current forays.
Starting one cancels the preceding foray of its kind without replacing the other kind.
Pending typing is persisted before reading the search word and language.
A blank word or stale draft read starts nothing.

Each sink has a relay shared by the search and its failure completion.
Failure completion ends the relay only if it has not already ended.
Transcription steps carry their foray even when they arrive before the start method returns.
A named scheme selects transcription lookup, while an empty scheme selects pronunciation lookup.

Reading picks apply immediately to the target carried by the foray.
A schemed pick writes transcription text without a variety.
Pronunciation picks write IPA before asking the shared pronunciation quill to set variety.
Non-primary targets are checked against the draft's current rows before a request is made.

## `LForay`

A foray keeps the word, language, target and scheme of one search.
Cancellation is guarded and disposes its source only once.
The search is started without requiring the caller to await it.
A search fault goes to the injected `fault` owner, which records it.
So does a fault of the unstarted callback.

Recording saves check the draft's current word and language both before and after download.
Those checks persist pending typing, so delayed keystrokes participate in the comparison.
An ended tenure or changed word or language prevents attachment.
The download and preview deliberately use `CancellationToken.None`, independent of search cancellation.

The primary target uses an audio request, while another target uses a pronunciation audio request.
Variety assignment follows attachment through the shared pronunciation quill.
An absent saved path answers null rather than making an attachment request.

Schemed lookups load no flags and return a bare reading mark.
Other marks come from engine settings and feed the same Application reading rule.

Cancelled searches end quietly.
Other search failures are recorded because the caller does not await the task.
Failure completion runs only while the search token remains uncancelled.
A failure in that completion is recorded too and is not retried.

## `LPosture`

Posture keeps shared session preferences beside the engine and persists through its settings interface.
Vista bulletins store order and filter, while workspace bulletins reload posture.
Only a vista started through the posture is remembered, so a bulletin of any other vista stores nothing.
That keeps a vista started beside the posture, as a detached atelier does, out of the stored layout.
Dispose removes the engine subscription and editing handlers of remembered vistas.

Volume changes stay in memory until save is requested, avoiding a write for every gesture step.
Nonfinite levels are ignored and finite levels are clamped.
A failed save leaves the remembered stored level unchanged, allowing another settled gesture to retry.

Layout updates merge fields with existing tab records rather than replacing the whole layout.
Unspecified fields and tabs retain their previous values.
Filter comparisons use catalog formatting instead of relying on list reference equality.

The first failed save retains a fault and suppresses later failure notices until load clears the flag.
Fault delivery waits until the posture gate is no longer held.
This lets a subscriber handle the failure outside that lock.

Load uses the legacy settings key only when the current key supplies no posture.
A failed read leaves the current state alone.
A nondefault legacy state is adopted before saving under the current key.

## `LVista`

A vista owns one tab's order, filter, query, chosen row and editing state.
Its hearth reference supplies bulletin delivery without retaining the whole engine.

Order, filter, query and editing changes announce the vista identity.
Selection alone raises no bulletin.
Editing asks notify the save event even when the local mode is unchanged.
This allows shared posture to follow the ask independently of the vista's previous mode.

Plain observers receive their subject's bulletins except notices naming another vista.
Chosen observers additionally require the chosen row identity or a nonpositive bulletin identity.
Both rosters are copied together under the gate and dispatched outside it, plain observers first.
The chosen predicate rereads selection under the gate, allowing earlier callbacks to affect later delivery.

## `LTrove`

Lookup results are held against a nonzero session identity plus the searched word and language.
Transcription results additionally use scheme identity, allowing separate answers for separate schemes.
Ordinal matching prevents a changed word or language from reusing an old answer.

Candidate results are saved only when at least one row has phonetic text.
Recording results require at least one row with an address.
The whole result is retained, preserving unsuccessful rows beside usable findings.
An unusable result does not replace a previously held result.
Session clearing removes every transcription scheme along with candidate and recording results.

## Stamp

Commit: `52c21d68b4c4`
- `LErrand.cs` `c61ceb9f8ddc5692`
- `LForay.cs` `eb0b0946abaaf6ee`
- `LPosture.cs` `1c14bb4b431dac60`
- `LTenure.cs` `e482dc12cd6bda4f`
- `LTenureGauge.cs` `05df6162da764173`
- `LTenureHerald.cs` `f36aae6150e1b18d`
- `LTenureQueue.cs` `31568e59a98da620`
- `LTrove.cs` `3ee760245e6b7943`
- `LVista.cs` `6e86b081ea0753e2`
