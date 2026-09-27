# CStemPage.cs

## `public sealed record CStemPage(`

The page of one phonetic series, as the xiesheng reader prints it.
The blank page carries empty text and no character.

**Parameters**

- `CStemPageLanguage`: the language of the series, which picks its font and flag.
- `CStemPageKey`: the series key, printed as the headword.
- `CStemPageCharacters`: the member characters, already ordered by the engine.
- `CStemPageEmpty`: whether the series holds no character, as the engine judges it.
