# LUnit.cs
Hash: `c99a5412bc633ebf`

## `public enum LUnit`

The lexical unit an entry stands for, kept apart from its Freedom and its parts of speech.
A part of speech names a role, and the unit names what kind of piece carries it.
The user chooses it, and every language offers the same three.
The numbers are stored in `entry.unit`, so a member never changes its number.
Numbers 1 and 2 held the retired content and function units, and a rebuild clears them.

- `LUnitEmpty` — Not chosen yet.
- `LUnitMorpheme` — A morpheme.
- `LUnitWord` — A word.
- `LUnitExpression` — An expression of more than one word.
