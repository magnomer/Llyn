# LReflexPort.cs
Hash: `9d9c67ddda315c52`

## `public interface LReflexPort`

The slice of the engine a deportment sees when it shows an entry's reflex readings.
The readings are fetched in the background, so the port has a check beside the rebuild.
`LReflexFacade` implements it.

## `bool LEngineReflexCheck(long entryId);`

Whether the entry's reflex readings are still being fetched.

## `void LEngineReflexRebuild(long entryId);`

Fetches the entry's reflex readings again.

## `IReadOnlyList<LReflexGuise> LEngineGuiseRead(string language, IReadOnlyList<string> reflexes);`

How each reflex row prints, one answer per language in `reflexes`, in order.
The fold set is the one the pack of the entry's `language` declares.
