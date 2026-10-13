# Quill

This folder owns Author, Example, Source and Situation field edits, plus shared Mention removal and selection checks.
Its quills route changes through the tenure rather than writing records themselves.

## `LQuillAuthor`

Author name typing uses the tenure's deferred request path.

## `LQuillExample`

Example typing writes text as known, while language and reference choices apply immediately.

## `LQuillMention`

Mention selection checks share the herald's kept draft instead of forcing pending typing on every menu check.
`LQuillSenseRead` persists first because taking an Entry identity requires the latest typed text.
Span-based removal lives in `LQuillChip`, which reuses this quill's find.

## `LQuillReference`

Source typing defers, while kind and credit changes apply immediately.
The reference clerk decides whether a kind choice warrants a request.
Author additions and picks share the tenure path, keeping credits with the Source being edited.
The draft clerk validates names and rejects picking the former Author again.

## `LQuillSituation`

Each Situation field has its own request, so typing one field does not rewrite another.
The held Situation supplies both the target identity and the previous field state.
An empty field retains uncertainty only when its held value was uncertain.
Reading that state does not flush pending typing, preserving its delay.

## Stamp

Commit: `c2f335c331fe`
- `LQuillAuthor.cs` `69ecc10a26693e88`
- `LQuillExample.cs` `9bb47d84a3103237`
- `LQuillMention.cs` `6deb5fd9b4c21639`
- `LQuillReference.cs` `926d3f4719bbe6a2`
- `LQuillSituation.cs` `dacb4ab21213311a`
