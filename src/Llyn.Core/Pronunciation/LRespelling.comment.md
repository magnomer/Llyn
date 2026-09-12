# LRespelling.cs

## `public sealed record LRespelling(`

One group of ordered rewrite rules a language pack declares, scoped to the regional varieties it names.
A pack recasts a looked-up transcription into its preferred symbol convention through these groups.
The engine knows a group only as a name, a variety scope, and a rule list.
Nothing consumes the groups until the lookup applies them and the user switches respelling on.

**Parameters**

- `LRespellingName` — The name the pack gives the group, for reading the pack rather than for the engine.
- `LRespellingVarieties` — The variety names the group applies to, compared ordinally against a candidate's tag.
  An empty list applies the group to every candidate, tagged or not.
  A non-empty list never matches an untagged candidate.
- `LRespellingRules` — The rules in written order, each held as an [LRespellingRule](LRespellingRule.comment.md).

## `public string LRespellingResolve(string phonetic, string variety)`

Returns the transcription unchanged when the variety falls outside the group's scope.
Otherwise it runs every rule in written order, each on the previous rule's output.
The input is NFC-normalized once before the first rule.
So a combining mark and its precomposed form from different sites match the same pattern.

## `public static string LRespellingScan(IReadOnlyList<LRespelling> groups, string phonetic, string variety)`

Applies a whole pack's groups in list order, feeding the output of one into the next.
An empty list returns the input untouched.

## Inline notes

### `private bool LRespellingVarietyCheck(string variety)`

An empty scope admits every variety, and a named scope admits only an exact ordinal match.
