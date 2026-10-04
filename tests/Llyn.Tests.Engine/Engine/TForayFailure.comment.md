# TForayFailure.cs
Hash: `039821f324ec1d94`

## `public sealed class TForayFailure`

How a foray's search fails or finishes, and what it writes to the audit file on the way.
A search that answers tells its listener it finished exactly once and writes no fault.
A search that throws is written to the workspace's audit file, and its listener is told it finished exactly once.
That holds for a source failing inside the harvest or lookup, whose relay sends the end.
It holds as well for a source factory failing before either starts, where the tenure sends it.
Each case waits, within a bounded limit, until the audit file holds the asserted fault and the end has arrived.
Then it counts the end steps.
The expected factory fault is read from `TRigFake`, so the text lives in one place.
A sink that throws on the tenure's end is recorded beside the first fault.
The end is still sent once.
The recording pack comes from [TForay](TForay.comment.md).
The lookup pack is this file's own.

## `private static async Task TForaySettle(Func<bool> condition)`

Polls `condition` until it holds or five seconds pass, then asserts it.
A slow machine fails with a plain message instead of a fixed sleep racing the work.

## `private static string TForayAuditRead(TWorkspace workspace)`

The workspace's audit file text, or empty while the file is missing or held by its writer.
