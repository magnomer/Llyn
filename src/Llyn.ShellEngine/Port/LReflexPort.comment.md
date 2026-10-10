# LReflexPort.cs
Hash: `e2eb9636b1fea203`

## `public interface LReflexPort`

The slice of the engine a deportment sees when it shows an entry's reflex readings.
The readings are fetched in the background, so the port has a check beside the rebuild.
It also reads and writes whether an entry's "More readings" rows are opened.
It reads and writes the Fanqie and Script box state of an entry too.
That state is fold state of one entry, which this port already serves.
`LReflexFacade` implements it.

## `bool LEngineReflexCheck(long entryId);`

Whether the entry's reflex readings are still being fetched.

## `void LEngineReflexRebuild(long entryId);`

Fetches the entry's reflex readings again.

## `IReadOnlyList<LReflexGuise> LEngineGuiseRead(string language, IReadOnlyList<string> reflexes);`

How each reflex row prints, one answer per language in `reflexes`, in order.
The fold set is the one the pack of the entry's `language` declares.

## `bool LEngineSpreadCheck(long entryId);`

Whether the entry's "More readings" rows are opened.

## `void LEngineReflexSpread(long entryId, bool opened);`

Opens or closes the entry's "More readings" rows and keeps that state for the entry.
Each write raises the fold bulletin on the entry, so other open views re-read it.

## `bool LEngineBoxCheck(long entryId, LFoldBox box);`

Whether that editor box of the entry is opened.

## `void LEngineBoxSpread(long entryId, LFoldBox box, bool opened);`

Opens or closes that editor box and keeps that state for the entry.
Each write raises the fold bulletin on the entry, so other open views re-read it.
