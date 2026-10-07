# TEngineScript.cs
Hash: `6a91e6d779c08fe6`

## `public sealed class TEngineScript`

Covers the engine's script fetch over a fixture pack with two styles behind stubbed pages.
A start posts each style's form, reads every picture and caption the pattern names, and fetches the bytes.
The rewrite and prefix shape the picture address before it is fetched, and the gloss is read once per style.
A picture that cannot be fetched is skipped, and the positions count the kept ones.
A read of an entry with nothing stored returns empty, and a start fetches every character.
It stores the sets and raises the script bulletin.
A character every database answered for yet none drew still raises the bulletin.
It is asked once per session and stores nothing.
A rebuild drops the stored sets and fetches again, asking even sources once silent.
A language whose pack lists no styles reads empty and asks nothing.

A fetch lists its pictures in the pack's declared epoch order, an undated picture last.
The image sort orders by character, then pack style, then declared epoch.
Characters follow the headword's spelling, and one the headword lacks follows by code point.
The stored id breaks the last tie, so input order never decides.
An undeclared style follows the declared ones, and an undeclared epoch precedes an undated one.

## `public async Task ScriptSourceFind_PatternTimesOut_AnswersNotReached()`

A pack pattern that times out answers no pictures and not reached, so the miss is not remembered.

## Inline notes

`TScriptFetchRead` saves an entry, starts its fetch, waits for it to settle and reads what it stored.
`TScriptSettle` waits until no fetch runs for the entry, since a bulletin is raised per character.
`TScriptCountCheck` waits for the stub to see the given number of requests.
