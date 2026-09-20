# LLookupKind.cs

## `public enum LLookupKind`

Which moment of a pronunciation lookup one step reports.
A lookup is a sequence of steps, one source step per source, one candidate per source, and one end.

## `LLookupKindSource,`

A source has begun searching.

## `LLookupKindCandidate,`

A source has finished and the step carries what it had.

## `LLookupKindEnd,`

Every source has finished and no further step follows.
