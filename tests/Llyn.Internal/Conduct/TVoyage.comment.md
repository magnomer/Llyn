# TVoyage.cs

## `public sealed class TVoyage`

Covers the voyage's trails with no window, since they hold only tab keys and ids.
Nothing recorded lights neither way.
An empty station or the standing one again is recorded once at most.
A step back parks the standing station, so a step forward returns to it.
A refused jump leaves both trails as they were.
A new station after a step back forgets the future.
The past trail keeps fifty stations and drops the oldest first.

## `private static Func<string, long, bool> TVoyageShowCreate(List<(string, long)> shown, bool accepted)`

A jump seam that records every station it is asked to show and answers whether the jump went.
