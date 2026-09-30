# TErrandClip.cs

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

## `private const string TErrandClipPack`

A one-source pack whose recording source answers one British recording.

## `private static async Task<CErrand> TErrandClipStart(CEditor editor, string language)`

Opens a new entry with a headword in the language, and waits until its recording search ended.
A pack without sources ends the search at once, so later steps are the test's own.

## `private static CEditor TErrandClipPrepare(LEngine engine)`

Builds an editor over the engine and restores the input tab's vista, ordered by headword.
