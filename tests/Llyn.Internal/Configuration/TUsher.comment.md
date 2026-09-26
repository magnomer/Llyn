# TUsher.cs

## `public sealed class TUsher`

The file usher behind the `LUsher` port: the presence check, the quiet delete and the lock check.
The lock check is pinned on a wrapped sharing fault and on a missing file.
Opening a folder or a link lives in the media ring, which the tests do not reference.
