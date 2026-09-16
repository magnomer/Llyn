# LSourceSpec.cs

## `public sealed record LSourceSpec(`

The language-pack definition of one source.
It gives its name and the ordered extraction attempts.
The list it is declared in decides what it is for, so it carries no kind of its own.
This is pure data loaded from `languages//source.json`.
The generic source runner turns it into a live `LSource`.
So no source-specific code is needed for an ordinary source.
A frequency source also carries its bands and the figures that turn its raw value into a word interval.

**Parameters**

- `LSourceSpecName` — The source's display name, for example `"Cambridge"`.
- `LSourceSpecAttempts` — The extraction attempts, tried in order until one yields a value.
- `LSourceSpecSpelling` — The pack's ordered headword rewrite rules, empty when the pack declares none.
- `LSourceSpecBands` — The pattern bands labelling a raw figure the shared ladder cannot grade, empty outside a frequency source.
- `LSourceSpecTotal` — The corpus size a raw count or per-million figure is measured against, or `null`.
  The interval is then total over raw.
- `LSourceSpecFactor` — The multiplier a raw rank or class is scaled by to estimate the interval, or `null`.
- `LSourceSpecBase` — The base a raw class exponentiates before the factor applies, or `null` when the raw figure multiplies directly.
- `LSourceSpecUnit` — The word printed before a bare numeric answer in the tooltip, such as `Level`, or `null`.
