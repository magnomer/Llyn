# LTrail.cs

## `public interface LTrail`

A path as the operating system reads it, and the port that resolves, relates and names one.
The engine keeps every path as text.
It asks here whenever the text must be read the way the disk reads it.
`LTrailSystem` in Infrastructure answers with `System.IO.Path`, and a test may answer the same or with its own rule.
Nothing here touches the disk, so no answer depends on what exists.

## `string? LTrailResolve(string root, string path)`

The full path of `path`, taken under `root` when it is not already fully qualified.
A relative path that climbs out of `root` resolves to nothing, and so does a path the system cannot read.

## `string? LTrailRelativeResolve(string root, string path)`

The path relative to `root`, or nothing when it lies outside `root` or on another root.

## `string LTrailNameRead(string path)`

The last segment of the path, a trailing separator ignored.

## `bool LTrailRootCheck(string path)`

Whether the path starts from a root rather than from wherever the program stands.

## `string LTrailNameNormalize(string name)`

The name with every character a file name cannot carry replaced by an underscore.
