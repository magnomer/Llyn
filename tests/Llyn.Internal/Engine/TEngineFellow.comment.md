# TEngineFellow.cs

## `public sealed class TEngineFellow`

The co-author tally the vita shows, read from the engine in one call.
It also covers the two names the union question reads.

## `public void FellowFind_TwoSharedWorks_CountsTwo()`

An Author credited beside the read one on two Sources is listed once, counted two.

## `public void FellowFind_SortsBySharedThenName()`

The higher shared count leads, and equal counts sort by name.
An Author credited on no shared Source is not listed at all.

## `public void UnionRead_NoDraftAndMissingKept_ReadsBothEmpty()`

With no held draft and no stored kept Author, the union question reads two empty names.

## `public void UnionRead_PaddedKeptName_ReadsItTrimmed()`

The kept name reaches the union question without its outer spaces.

## `private static void TFellowReferenceCreate(LEngine engine, string title, params LAuthor[] credited)`

Stores one Source and credits the given Authors on it in order.
