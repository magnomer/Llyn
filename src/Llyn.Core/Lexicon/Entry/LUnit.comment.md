# LUnit.cs
Hash: `947607240093dcdb`

## `public enum LUnit`

The lexical unit an entry stands for, kept apart from its parts of speech.
A part of speech names a role, and the unit names what kind of piece carries it.
A language whose writing spaces its words offers Content, Function and Morpheme.
A language that does not offers Word and Morpheme.
The numbers are stored in `entry.unit`, so a member never changes its number.

- `LUnitEmpty` — Not chosen yet.
- `LUnitContent` — A content word in a spaced language.
- `LUnitFunction` — A function word in a spaced language.
- `LUnitMorpheme` — A morpheme, offered in every language.
- `LUnitWord` — A word in a language that does not space its words.
