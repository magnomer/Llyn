# TEngineClock.cs

## `public sealed class TEngineClock`

Covers the clock port the engine stamps a moment with.
A frozen rig clock stamps a held entry draft and a held author draft with the frozen moment.
So every timestamp the engine writes is the rig's to set, and no engine test depends on the wall clock.
