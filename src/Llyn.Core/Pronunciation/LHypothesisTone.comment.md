# LHypothesisTone.cs

## `public sealed record LHypothesisTone(`

One class of a rime-book tone in the user's reconstruction, such as 4S under 上.
The rows of a tone are tried in order, and the first whose onset pattern matches wins.

**Parameters**

- `LHypothesisToneOnset` — A .NET regex matched against the onset the initial table gave, empty to match every onset.
- `LHypothesisToneRules` — Rewrites run in order over the whole syllable, onset and final joined.
- `LHypothesisToneClass` — The class key the reading view prints after the reading, such as `1` or `4S`.

The pattern is compiled once when the record is built, so a broken pattern fails while the pack loads.
The compiled form is a private field, so it never enters the record's equality.

## `public bool LHypothesisToneMatch(string onset)`

Whether the row takes this onset: true for an empty pattern, else the regex's own answer.

## `public string LHypothesisToneResolve(string syllable)`

Runs every rewrite over the syllable in order and returns the result.

## `public bool Equals(LHypothesisTone? other)`

Two rows are equal when their onset, class and rule sequence match.
The synthesized record equality would compare the compiled regex by reference and call twins unequal.

## `public override int GetHashCode()`

Hashes the onset, the class and the rule count.
