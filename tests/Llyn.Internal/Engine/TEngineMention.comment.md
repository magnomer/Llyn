# TEngineMention.cs

## `public sealed class TEngineMention`

The labels a chip line shows for the Mentions of one sentence, resolved by the engine in one call.

## `public void MentionResolve_SenseNamed()`

A Mention narrowed to a Meaning carries the span text, the Entry's headword and the Meaning's title.
A Mention standing for nothing carries its span text with an empty name and sense.
