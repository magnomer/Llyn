# LFanqieSource.cs

## `public interface LFanqieSource`

The port that asks one rime book's site for one character's placements.
It is a fetch over the network, not storage, so it is not a Vault.
`LFanqieSourceHttp` in Infrastructure is its adapter over the shared `HttpClient`.
The engine holds the port from the rig and never names a client.

## `Task<(IReadOnlyList<LFanqieRow> LFanqieFound, bool LFanqieReached)> LFanqieSourceFind(`

The placements the book answers with, and whether the site answered at all.
An unreached site answers an empty list and false, so the caller can retry later.
