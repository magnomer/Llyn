# TErrandClip.cs
Hash: `131e14f4053b3523`

## `public sealed class TErrandClip`

Covers the clip popup's gates end to end on a real editor: start, flag load, preview and taking.
A recording search over the held draft fills the clip with the source, its recording and the end.
The recording lands with its variety keys, and a blank headword answers the empty notice.
The flag load stores the variety flags of a flagged pack, and nothing while no search runs.
A step that lands mid-fetch keeps the preview's fetching mark, which turns playing once the file lands.
The finish returns the recording to plain once, and a refused fetch marks the recording refused.
A popup cancelled mid-fetch plays nothing.
A taking reads saving while a step lands, starts nothing on a second press, then reads saved.
It tags the primary row with the recording's variety, and a refused download offers the retry.
A broken host shows the recording notice once, for the preview, the taking and the flag load alike.
The preview still marks the recording refused, and the taking still offers the retry.
A recording store that raises a vault fault shows the same notice and records the fault once.
The preview then marks the recording refused, and the taking offers the retry with nothing attached.

## `private const string TErrandClipPack`

A one-source pack whose recording source answers one British recording.

## `private static async Task<CErrand> TErrandClipStart(TEditorFixture editor, string language)`

Opens a new entry with a headword in the language, and waits until its recording search ended.
A pack without sources ends the search at once, so later steps are the test's own.

## `private static LEngine TErrandFaultStart(TWorkspace workspace, List<Exception> recorded)`

Starts an engine on the workspace whose recording store faults on every save and fetch.
Its audit recorder adds each recorded fault to `recorded`.

## `private static TEditorFixture TErrandClipPrepare(LEngine engine)`

Builds an editor over the engine whose notices nobody reads.

## `private static TEditorFixture TErrandClipPrepare(LEngine engine, List<string> asked)`

Builds an editor over the engine and restores the input tab's vista, ordered by headword.
Each notice the editor shows is added to `asked`.
It answers the fixture, so a test reads only the entry and desk facets.
