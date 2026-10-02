# TCardSentence.cs
Hash: `063f9bd8b937e922`

## `public sealed class TCardSentence`

Covers the sentence gates over an entry desk on a real workspace, with no delay.
A row added below the first takes the second place, and a held row can be dropped.
The typed text, particle and dependence land on the row as written.
A Source picked from the citation dropdown is cited on the row by its id.
A selected word links its span to the Entry.
A language with no frame answers empty particle and dependence lists.
With no held draft a gate does nothing.

## `private static (CDesk TSentenceDesk, CSentence TSentenceGate) TSentencePrepare(LEngine engine)`

Starts an entry desk by origin and subject with no delay, and builds its sentence gates.

## `private static (long TSentenceSheet, long TSentenceRow) TSentenceCardAdd(CDesk desk)`

Adds a meaning card with one sentence to the held draft, and answers both ids.
