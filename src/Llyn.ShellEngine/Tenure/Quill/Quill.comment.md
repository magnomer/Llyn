# Quill

This folder owns Author, Example, Source and Situation field edits, plus shared Mention removal and selection checks.
Its quills route changes through the tenure rather than writing records themselves.

## `LQuillAuthor`

Author name typing uses the tenure's deferred request path.

## `LQuillExample`

Example typing writes text as known, while language and reference choices apply immediately.
The reference find, the title resolve and the Mention edits address the held Example through the draft's own example site.
The Mention edits go through `LQuillChip`, so a card sentence and the held Example share one span path.

## `LQuillMention`

Mention selection checks share the herald's kept draft instead of forcing pending typing on every menu check.
`LQuillSenseRead` persists first because taking an Entry identity requires the latest typed text.
The find takes an example site, so one rule serves a card sentence and the held Example.
Mention and sense checks have a form for a card sentence, named by kind, card and sentence.
They have another form for the held Example.
Removal by Mention id has the same two forms.
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

Commit: `8c403d1df768`
- `LQuillAuthor.cs` `69ecc10a26693e88`
- `LQuillExample.cs` `2693b7d181569437`
- `LQuillMention.cs` `aaf477dd07735483`
- `LQuillReference.cs` `926d3f4719bbe6a2`
- `LQuillSituation.cs` `dacb4ab21213311a`
