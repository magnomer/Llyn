# LScriptSource.cs

## `public interface LScriptSource`

The port that asks one script style's site for one character's glyph images.
It is a fetch over the network, not storage, so it is not a Vault.
`LScriptSourceHttp` in Infrastructure is its adapter over the shared `HttpClient`.
The engine holds the port from the rig and never names a client.

## `Task<(IReadOnlyList<LScriptImage> LScriptFound, bool LScriptReached)> LScriptSourceFind(`

The images the style's page links, each with its bytes, and whether the site answered at all.
An unreached site answers an empty list and false, so the caller can retry later.
