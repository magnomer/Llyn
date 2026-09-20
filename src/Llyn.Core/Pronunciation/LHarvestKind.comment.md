# LHarvestKind.cs

## `public enum LHarvestKind`

Which moment of an audio-recording discovery one step reports.
A discovery is a sequence of steps, one source step per source, one recording per source, and one end.

## `LHarvestKindSource,`

An audio source has begun searching.

## `LHarvestKindRecording,`

A source has finished and the step carries what it had.

## `LHarvestKindEnd,`

Every audio source has finished and no further step follows.
