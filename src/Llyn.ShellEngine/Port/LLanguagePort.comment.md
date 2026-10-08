# LLanguagePort.cs
Hash: `a38e3623c2731155`

## `public interface LLanguagePort`

The slice of the engine a deportment sees when it asks the language facts of a pronunciation.
It starts the sound fetches and reads the tone contour, the silent flag and the accent block.
`LLanguageFacade` implements it.

## `IReadOnlyList<int> LEngineContourScale { get; }`

The pitch levels a tone contour draws, highest first, passed up from the rule that parses the levels.

## `void LEngineSoundStart(long entryId);`

Starts every background fetch a reading view of the entry shows, in the engine's order.

## `IReadOnlyList<LContour> LEngineContourRead(string language, string ipa);`

The tone contour syllables of `ipa` in `language`, empty when nothing is drawn.

## `bool LEngineSilentCheck(string language);`

Whether the pack declares the language silent, so the input panel hides its pronunciation rows.
A blank language answers false, so an empty desk keeps its pronunciation rows.

## `LAccentSheet LEngineAccentRead(LEntryDraft draft);`

The pronunciation block of `draft`, ready for the reading view.

## `Task<LAccentSheet> LEngineAccentLoad(LEntryDraft draft, Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store);`

Loads the variety flags the block of a draft draws, then answers the block.
