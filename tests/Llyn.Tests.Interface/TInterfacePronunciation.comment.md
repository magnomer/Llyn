# TInterfacePronunciation.cs
Hash: `f6b7200ad7bbbed3`

## `internal static partial class TInterface`

The relays for the pronunciation lookup seams below the engine.
That is the reading, answer, candidate and variety records, the respelling rules, and the lookup fan-out.
The recording records, the harvest fan-out, the workspace download, and the engine discovery are relayed here too.
The language pack loader, the glyph split and the engine's language reads are relayed here too.
The anatomy, descent and tone contour reads are relayed here too.
The reflex guise, the stem key scan and the shengfu record are relayed here too.
The frequency records, the gauge, and the once and band resolves are relayed here too.
The two short fanqie row builders sit here beside the full one in `TInterfaceFact`.
Each relay is transparent and carries no test logic of its own.

`TInterfaceFanqie` holds the anchor, diwei and hypothesis relays.
`TInterfaceSource` holds the source specs and the page sources over a client.
`TInterfaceTrove` holds the per-session trove.

## `internal static LSeeker TLookupCreate(IReadOnlyList<LSource> sources, IReadOnlyList<LVariety>? varieties = null, IReadOnlyList<LRespelling>? cleanups = null, bool literal = false)`

Returns the lookup as its interface, so a test never names the application type.
The varieties default to none, so a test without a pack behaves like a language without varieties.
The cleanups default to none too, so a test without a pack sees only the built-in normalization.
The literal flag defaults to false.

## `internal static LReceiver TReceiverRespellingCreate(LReceiver inner, IReadOnlyList<LRespelling> groups)`

Returns the respelling wrapper as the receiver interface, so a test drives it as the engine would.
The three receiver relays below it let a test push one callback at a time through that interface.

## `internal static LReceiver TReceiverRelayCreate(Action<LLookupStep> sink)`

Returns the step relay as the receiver interface, so a test can check each callback becomes one step.
`TListenerRelayCreate` is the listener counterpart over harvest steps.

## `internal static IReadOnlyList<LReading> TReadingScan(IReadOnlyList<LReading> readings, IReadOnlyList<LVariety> varieties)`

Relays the static fan-out so it can be checked without a source, receiver, or listener.

## `internal static LHarvest THarvestCreate(IReadOnlyList<LSource> sources, IReadOnlyList<LVariety>? varieties = null)`

The recording counterpart of `TLookupCreate`.
The varieties default to none, so a test without a pack behaves like a language without varieties.

## `internal static IReadOnlyList<LRecording> THarvestRecordingScan(IReadOnlyList<LRecording> recordings, string variety)`

Relays the static narrowing, so a test can check a kept set narrowed to one row's variety.

## `internal static Task<string> TWorkspaceRecordingPrepare(LRecording recording, string root, HttpClient client, CancellationToken cancellation)`

Relays the preview download into the workspace cache.

## `internal static Task TEngineRecordingFind(this LEngine engine, long session, string word, string language, long target, Action<LHarvestStep> sink, CancellationToken cancellation)`

Relays the engine-level discovery, so a test can see the draft's row variety narrow the search.

## `internal static LAnatomy TAnatomyScan(IReadOnlyList<LAnatomyRule> rules, string language, string text, string respelling)`

Relays `LAnatomy.LAnatomyScan`.

## `internal static bool TAnatomyRuleMatch(this LAnatomyRule rule, string language)`

Relays `LAnatomyRule.LAnatomyRuleMatch`.

## `internal static IReadOnlyList<string> TDescentScan(IReadOnlyList<LDescent> rules, string language, string tone)`

Relays `LDescent.LDescentScan`.

## `internal static bool TDescentMatch(this LDescent rule, string language)`

Relays `LDescent.LDescentMatch`.

## `internal static LFanqieRow TFanqieRowCreate(string initial, string rime, string heading, string division, string tone, bool rounded)`

A row carrying only the parts the hypothesis reads.
The character, book, position and text are fixed, because the resolve never looks at them.
