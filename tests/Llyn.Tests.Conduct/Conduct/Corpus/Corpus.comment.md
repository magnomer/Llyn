# Corpus

These tests cover the corpus area's catalogs, transcript editing and quotation display on real workspaces.
They distinguish parent Example state from child entry state through draft reads and notifications.

## `TAnthology`

Catalog tests exercise rows and aperture state independently of transcript editing.
Failure cases separate row loading from tally loading.

## `TAnthologyPrepare`

The shared setup builds an anthology over an Example desk through the test relay.

## `TAnthologyCitation`

Citation tests distinguish resolving a Source from displaying its ready line.
A failing resolve and an unopened desk check separate refusal paths.

## `TAnthologyExample`

Direct Example mapping checks linked excerpts independently of workspace editing.
Unknown and unwritten text are separate cases because their wording and muted verdicts differ.

## `TAnthologyGloss`

Insertion tests preserve neighboring gloss languages while using the configured language for new rows.
Preparation tests distinguish an empty held transcript from an unopened desk.

## `TCorpus`

Example and quotation selections are checked against distinct diptych sides.
Query and workspace changes check that stale selections clear.
Deletion tests protect the quotation side from deleting its parent Example.

## `TCorpusPrepare`

The shared setup supplies an active tab and a marshal that executes callbacks immediately.

## `TCorpusHold`

A counted marshal checks that transcript editing passes through the supplied callback.
Cancellation checks blank transcript content separately from its retained tally and placeholder.
Atelier closure checks that the quoting entry editor releases its draft and stops recording playback.

## `TCorpusPortrait`

Export uses a real file to distinguish an Example selection from an entry displayed for export.
The empty print case checks the completed task without invoking a printer.

## `TCorpusScribe`

Leave answers distinguish cancellation, discard and storage of an unsaved transcript.
Closing transcript and quotation editors checks their different routes back to the chosen Example.

## `TCorpusVista`

Repeated restoration checks retained queries and duplicate observer notifications.
Entry notices check row notifications for quote tallies.
The asynchronous load checks rows and languages after filling.

## `TQuotation`

Two Examples distinguish entries quoting the selection from unrelated entries.
No selection checks the unfiltered entry list.
Query tests distinguish unmatched wording from vacant wording.

## Stamp

Commit: `c2f335c331fe`
- `TAnthology.cs` `5f4cbaf4238e8702`
- `TAnthologyCitation.cs` `9e6422ccd79e62ec`
- `TAnthologyExample.cs` `7b627fdeb76ed1de`
- `TAnthologyGloss.cs` `9359d7893d79d281`
- `TCorpus.cs` `0d88f7649ee45fc0`
- `TCorpusHold.cs` `d5ed98be21918e0f`
- `TCorpusPortrait.cs` `e4f3a91ae8cfb9bb`
- `TCorpusScribe.cs` `8ecc1fd49b2fdcb7`
- `TCorpusVista.cs` `d5f543a6b5a2bddf`
- `TQuotation.cs` `4ea73c19f863dfd9`
