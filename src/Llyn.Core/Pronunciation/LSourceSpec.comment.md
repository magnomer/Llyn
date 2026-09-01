# LSourceSpec.cs

## `public sealed record LSourceSpec(`

The language-pack definition of one source: its name, the kind of value it yields, and the ordered extraction attempts that make it work. This is pure data loaded from `languages//source.json`; the generic source runner turns it into a live `LSource`, so no source-specific code is needed for an ordinary source.

**Parameters**

- `LSourceSpecName` — The source's display name, for example `"Cambridge"`.
- `LSourceSpecKind` — The value kind: `"pronunciation"` or `"audio"`.
- `LSourceSpecAttempts` — The extraction attempts, tried in order until one yields a value.
