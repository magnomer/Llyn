# LInflectionKind.cs
Hash: `03e0f84167e2d61a`

## `public sealed record LInflectionKind(string LInflectionKindName, string LInflectionKindPattern) : IEquatable<LInflectionKind>`

One headword class of a rule book, named by the pack and recognised by a pattern.
A headword may belong to several kinds at once.
Named groups in the pattern feed the stem templates, so `${root}` reads the group `root`.
Matching ignores case, ignores culture and waits at most one second.
Equality compares the two strings, because the compiled pattern differs per instance.

**Parameters**

- `LInflectionKindName`: the pack's class identifier for stem templates and kind-limited rules.
- `LInflectionKindPattern`: the regular expression recognizing this class.

## `public bool LInflectionKindCheck(string headword)`

Whether `headword` belongs to this kind.
A timeout excludes this kind without deciding whether another kind can predict the cell.

## `public string LInflectionKindResolve(string headword, string template)`

Expands `template` against the first match in `headword`.
Unmatched text remains around the first replacement, so templates requiring whole-headword expansion need anchored patterns.
A match that times out returns the headword unchanged.

## `public bool Equals(LInflectionKind? other)`

Ordinal name and pattern equality excludes the cached regex instance from record identity.

## `public override int GetHashCode()`

Hashing uses the same name and pattern strings as equality.
