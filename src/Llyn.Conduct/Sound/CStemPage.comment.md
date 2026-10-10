# CStemPage.cs
Hash: `638208580b45e336`

## `public sealed record CStemPage(string CStemPageLanguage, string CStemPageKey, IReadOnlyList<CStemMember> CStemPageMembers, bool CStemPageEmpty, CFont CStemPageFont, CFont CStemPageGlyph)`

The page of one phonetic series, as the xiesheng reader prints it.
The blank page carries empty text and no character.

**Parameters**

- `CStemPageLanguage`: the language of the series, which picks its fonts and flag.
- `CStemPageKey`: the series key, printed as the headword.
- `CStemPageMembers`: the member characters, already ordered by the engine, each with its reading line and reflex rows.
- `CStemPageEmpty`: whether the series holds no character, as the engine judges it.
- `CStemPageFont`: the headword font of the series language, ready to paint on the key.
- `CStemPageGlyph`: the glyph font of the series language, ready to paint on the member list.
