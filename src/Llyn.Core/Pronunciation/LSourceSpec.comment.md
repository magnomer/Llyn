# LSourceSpec.cs

## `public sealed record LSourceSpec(`

The language-pack definition of one source.
It gives its name, the kind of value it yields, and the ordered extraction attempts.
This is pure data loaded from `languages//source.json`.
The generic source runner turns it into a live `LSource`.
So no source-specific code is needed for an ordinary source.

**Parameters**

- `LSourceSpecName` — The source's display name, for example `"Cambridge"`.
- `LSourceSpecKind` — The value kind: `"pronunciation"` or `"audio"`.
- `LSourceSpecAttempts` — The extraction attempts, tried in order until one yields a value.
