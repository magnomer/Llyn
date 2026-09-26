# TEngineFellow.cs

## `public sealed class TEngineFellow`

The co-author tally the vita shows, read from the engine in one call.

## `public void FellowFind_TwoSharedWorks_CountsTwo()`

An Author credited beside the read one on two Sources is listed once, counted two.

## `public void FellowFind_SortsBySharedThenName()`

The higher shared count leads, and equal counts sort by name.
An Author credited on no shared Source is not listed at all.

## `private static void TFellowReferenceCreate(LEngine engine, string title, params LAuthor[] credited)`

Stores one Source and credits the given Authors on it in order.
