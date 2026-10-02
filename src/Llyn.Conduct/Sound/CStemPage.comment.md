# CStemPage.cs
Hash: `6d750d35babf8c4d`

## `public sealed record CStemPage(`

The page of one phonetic series, as the xiesheng reader prints it.
The blank page carries empty text and no character.

**Parameters**

- `CStemPageLanguage`: the language of the series, which picks its fonts and flag.
- `CStemPageKey`: the series key, printed as the headword.
- `CStemPageCharacters`: the member characters, already ordered by the engine.
- `CStemPageEmpty`: whether the series holds no character, as the engine judges it.
- `CStemPageFont`: the headword font of the series language, ready to paint on the key.
- `CStemPageGlyph`: the glyph font of the series language, ready to paint on the character list.
