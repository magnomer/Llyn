# LCourierFacade.cs
Hash: `7c4bb51fd179b02e`

## `internal sealed class LCourierFacade`

The engine's facade for the Joplin push and its access grant.

## `public LCourierFacade(LEngineHearth hearth, LLiveryFacade livery, LSettingsFacade settings)`

Stores the hearth and the sibling facades it calls, all built by `LEngine` before this one.
The staff and the shared state are read through the hearth.
It takes its siblings rather than the engine, so it names only the facades it uses.

## `private LEngineStaff LCourierFacadeStaff`

Reads `LEngineHearth.LEngineStaffHeld`, so a call after a workspace switch reaches the new workspace's courier clerk.

## `public bool LEngineCourierCheck()`

Whether the current workspace holds a Joplin token, asked of the courier clerk.

## `public async Task<LReceipt> LEngineCourierSend(Func<string, string> lookup, CancellationToken cancellation)`

Forwards to the courier clerk, which guards its own gate.
It hands over `LLiveryFacade.LEngineLiveryRead` as the page reader and `lookup` unchanged.
The language reader is the other `LEngineLiveryRead` overload over `LSettingsFacade.LEngineTextFind`.
That source answers null for a missing key, as the rime-table page builder expects.
The Rime table panel passes the same source, so a category reads the same in Llyn and Joplin.
A call made after a workspace switch uses the new workspace.
A push already running finishes against the workspace it started in.
A refused token, as `LCourierWarrantCheck` judges it, is cleared before the refusal goes on.
So the shell offers Connect again instead of a connection that no longer works.

## `public async Task LEngineCourierAttach(CancellationToken cancellation)`

Awaits the grant, then stores the hidden token in the settings.
A settings bulletin follows only a real change, so an unchanged token repaints nothing.

## `private void LEngineWarrantClear()`

Drops the stored token, the reverse of the store in `LEngineCourierAttach`.
A settings bulletin follows only a real change, as there.
