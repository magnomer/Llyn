# TDisplayGrasp.cs
Hash: `933aa7157e1b6c5d`

## `public sealed class TDisplayGrasp`

Covers the reading view's grasp area, its limit, reads and gate, driven with no window.
The hostile cases stand on a fake grasp port, and the gate cases on a wing over the real engine.

## `public void DisplayGraspStep_HostileLimit_ReadsNoneBelowZero(int limit, int read)`

Any engine grasp limit reads back unchanged, except a negative one, which reads zero.
So the star control never takes a negative limit, as `CDisplayGrasp` promises.

## `public void DisplayGraspRead_HostileStep_ClampsBetweenZeroAndLimit(int limit, int stored, int read)`

Any stored step the engine answers reads between zero and the limit, as `CGrasp` promises.
A negative limit counts as zero, so every step then reads zero.
The label is the engine's wording of the clamped step, not of the stored one.

## `public void DisplayGraspSet_StandingStepPressedAgain_ClearsTheGrasp()`

A new step is stored and answered with its wording, and the standing step pressed again clears it.

## `public void DisplayGraspSet_NoEntryChosen_StoresNothing()`

With no entry chosen the gate stores nothing and words nothing.

## `internal static LGraspPort TGraspPortCreate(int limit, int stored)`

A fake grasp port answering only the grasp limit, the stored step and the step's wording.
The wording is the step itself, so a fact sees which step was worded.
`TEsteem` fakes its grasp reads here too.
