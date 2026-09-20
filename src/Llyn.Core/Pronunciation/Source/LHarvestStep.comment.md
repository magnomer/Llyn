# LHarvestStep.cs

## `public sealed record LHarvestStep(`

One step of an audio-recording discovery, carried to the caller as data.
The discovery streams a source step per source, a recording per source, and one end, in that order.
A step names its kind, the source it concerns, that source's position, and the recording when it has one.
A source step and the end step carry no recording.
The end step names no source and carries order zero.
Steps may be produced on background threads, so a caller that touches UI marshals on its own side.
The shell receives steps through a delegate, so no surface implements a contract to hear them.

## `public bool LHarvestStepEnded => LHarvestStepKind == LHarvestKind.LHarvestKindEnd;`

Whether this is the end step, answered here so the shell asks and never compares the kind itself.
A shell tells the three kinds apart by this verdict and by whether the recording is present.
