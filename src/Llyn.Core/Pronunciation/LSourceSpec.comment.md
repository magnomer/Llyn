# LSourceSpec.cs

## `public sealed record LSourceSpec(`

The language-pack definition of one source.
It gives its name and the ordered extraction attempts.
The list it is declared in decides what it is for, so it carries no kind of its own.
This is pure data loaded from `languages//source.json`.
The generic source runner turns it into a live `LSource`.
So no source-specific code is needed for an ordinary source.

**Parameters**

- `LSourceSpecName` — The source's display name, for example `"Cambridge"`.
- `LSourceSpecAttempts` — The extraction attempts, tried in order until one yields a value.
