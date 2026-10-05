# LCourierFacade.cs
Hash: `61ffcb5932adf2b6`

## `internal sealed class LCourierFacade`

The engine's facade for the Joplin push and its access grant.

## `public LCourierFacade(LEngine engine)`

Stores the engine, whose staff it reads on each call.

## `public Task<LReceipt> LEngineCourierSend(LPortraitLabel label, CancellationToken cancellation)`

Forwards to the courier clerk, which guards its own gate.
A call made after a workspace switch uses the new workspace.
A push already running finishes against the workspace it started in.

## `public async Task LEngineCourierAttach(CancellationToken cancellation)`

Awaits the grant, then stores the hidden token in the settings.
A settings bulletin follows only a real change, so an unchanged token repaints nothing.
