# LReflexSource.cs

## `public interface LReflexSource`

The port that asks one reflex rule's site for one character's modern readings.
It is a fetch over the network, not storage, so it is not a Vault.
`LReflexSourceHttp` in Infrastructure is its adapter over the shared `HttpClient`.
The engine holds the port from the rig and never names a client.

## `Task<(IReadOnlyList<LReflexDraft> LReflexFound, bool LReflexReached)> LReflexSourceFind(`

The reflex drafts the rule reads from the answer, and whether the site answered at all.
An unreached site answers an empty list and false, so the caller can retry later.
