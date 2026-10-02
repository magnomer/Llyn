# TVaultFakeEtymology.cs
Hash: `db8f128df75fca2c`

## `internal sealed class TVaultFakeEtymology : LEtymologyVault`

An in-memory etymology store for the clerk tests.
It keeps the narrative and the links of each entry apart, as the database does.
It checks nothing, so a test that wants the span rules exercises the archive instead.

## `public LEtymology? LEtymologyRead(long entryId)`

The narrative held for the entry, or null when none was saved.

## `public IReadOnlyList<LEtymon> LEtymologyEtymonRead(long entryId)`

The links held for the entry, empty when none were set.

## `public LEtymology? LEtymologySave(long entryId, LEtymology? etymology)`

Keeps the narrative, numbering its mentions from one, and forgets it when the prose is blank.

## `public IReadOnlyList<LEtymon> LEtymologyEtymonSet(long entryId, IReadOnlyList<long> targetIds)`

Keeps the links in the order given, each holding only its target.
