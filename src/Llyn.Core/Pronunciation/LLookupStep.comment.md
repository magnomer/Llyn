# LLookupStep.cs

## `public sealed record LLookupStep(`

One step of a pronunciation or transcription lookup, carried to the caller as data.
The lookup streams a source step per source, a candidate per source, and one end, in that order.
A step names its kind, the source it concerns, that source's position, and the candidate when it has one.
The position comes with the source step, so a menu row can be placed before any answer exists.
A source step and the end step carry no candidate.
The end step names no source and carries order zero.
Steps may be produced on background threads, so a caller that touches UI marshals on its own side.
The shell receives steps through a delegate, so no surface implements a contract to hear them.

## `public bool LLookupStepEnded => LLookupStepKind == LLookupKind.LLookupKindEnd;`

Whether this is the end step, answered here so the shell asks and never compares the kind itself.
A shell tells the three kinds apart by this verdict and by whether the candidate is present.
