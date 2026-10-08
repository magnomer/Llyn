# LScriptPort.cs
Hash: `cf36885a53e2af51`

## `public interface LScriptPort`

The slice of the engine a deportment sees when it shows an entry's script images.
The images are fetched in the background, so the port has a check beside each read.
`LLanguageFacade` implements it, since a script style is a fact of the pack.

## `bool LEngineScriptCheck(long entryId);`

Whether the entry's script images are still being fetched.

## `void LEngineScriptRebuild(long entryId);`

Fetches the script images of every character of the entry again.

## `IReadOnlyList<LScriptGroup> LEngineScriptDivide(long entryId);`

The entry's stored script images grouped by style, starting no fetch.

## `IReadOnlyList<LScriptGroup> LEngineScriptRead(long entryId);`

The images grouped by style, after starting the fetch of every character still missing.

## `bool LEngineStyleCheck(string language);`

Whether the language's pack declares at least one script style.
