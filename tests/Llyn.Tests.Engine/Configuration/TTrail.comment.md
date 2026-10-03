# TTrail.cs
Hash: `0f416a889d3d95cd`

## `public sealed class TTrail`

Covers the trail adapter over `System.IO.Path`, which touches no disk.
A path outside the root, or one that climbs out, has no relative form, and one inside has.
A name that merely begins with two dots, such as `..notes.txt`, is inside.
A qualified path resolves to itself and a relative one anchors under the root, unless it climbs out.
The name reads past a trailing separator, and a barred character becomes an underscore.
The root check accepts a rooted path and refuses a bare name.

## `private static readonly string TTrailRoot`

A root under the temp folder that never has to exist, since no call here reads the disk.
