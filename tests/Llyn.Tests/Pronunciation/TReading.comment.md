# TReading.cs

## `public sealed class TReading`

Covers the static fan-out a source's readings pass through before they become candidates or recordings.
A tag the pack does not declare passes through first, then the untagged reading fans out per declared variety.
Without varieties the readings come back as they were.
A variety the answer already tags is not fanned out again.
