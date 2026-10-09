# TErrandFault.cs
Hash: `fc7504c00cfc67f5`

## `public sealed class TErrandFault`

Covers the clip popup's gates when the host or the recording store fails, on a real editor.
A broken host shows the recording notice once, for the preview, the taking and the flag load alike.
The preview still marks the recording refused, and the taking still offers the retry.
A recording store that raises a vault fault shows the same notice and records the fault once.
The preview then marks the recording refused, and the taking offers the retry with nothing attached.
Editors and recording searches come from `TInterfaceConductDesk`, shared with `TErrandClip`.

## `private static LEngine TErrandFaultStart(TWorkspace workspace, List<Exception> recorded)`

Starts an engine on the workspace whose recording store faults on every save and fetch.
Its audit recorder adds each recorded fault to `recorded`.
