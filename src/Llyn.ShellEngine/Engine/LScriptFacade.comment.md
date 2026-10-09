# LScriptFacade.cs
Hash: `0868062d56c4daf9`

## `public sealed class LScriptFacade : LScriptPort`

The engine facade for script images, the pictures of an entry's characters in each style its pack names.
A script style is a fact of the pack.
The fetch, the store and the grouping belong to the script clerk alone.
It implements the script port itself, so Host hands it to Conduct with no outlet between.

## `internal LScriptFacade(LEngineHearth hearth)`

Stores the hearth, built by `LEngine` before this one.
The gate and the staff are read through the hearth at each call.
So a workspace switch is seen at once.
It names no sibling facade, since the script clerk answers every call.

## `public IReadOnlyList<LScriptStyle> LEngineStyleRead(string language)`

The script styles of a language.

## `public bool LEngineStyleCheck(string language)`

Whether the language pack names any script style at all.

## `public void LEngineScriptStart(long entryId)`

Starts the fetch of every character that has no images.

## `public void LEngineScriptRebuild(long entryId)`

Drops the stored images of the entry's characters and fetches them again.

## `public IReadOnlyList<LScriptGroup> LEngineScriptDivide(long entryId)`

The images grouped by style.

## `public IReadOnlyList<LScriptGroup> LEngineScriptRead(long entryId)`

The images grouped by style, after starting the fetch of every character still missing.

## `public bool LEngineScriptCheck(long entryId)`

Whether a fetch is pending for any character of the entry.
