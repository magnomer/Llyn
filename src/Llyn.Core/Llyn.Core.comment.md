# Llyn.Core

Core builds the domain model and the application-facing contracts.
It has no project references, so the core stays independent of outer layers.
It grants internal access to Llyn.Tests.Interface, whose relays reach contracts and models for the tests.
A Windows-only call turns the build red through `CA1416` as an error, so the platform stays out of Core.

## `LPhonograph`

The port through which the logic rings play a recording.
The engine decides which file plays and at what level, and never touches a player itself.
`LPhonographMedia` in Core.Windows plays through the platform, and a test stays silent.
`LPhonographPlay` plays the file from its start, replacing whatever was playing.
`LPhonographStop` stays quiet when nothing plays.
`LPhonographVolumeSet` takes a level from zero to one, for the next and the playing file.

## `LUsher`

The port through which the logic rings ask a fact about a path.
`LEnsign` decides which flags to keep and which cached file to drop, and never touches a disk itself.
`LUsherFile` in the infrastructure answers from the file system, and a test answers from a list.
Playing a recording is not a path fact, so it goes through the sibling port `LPhonograph`.
`LUsherPathExist` answers false for a null path.
`LUsherPathDelete` stays quiet when the file is already gone or locked.
`LUsherLockCheck` is true when the system would not open a file, rather than its content being bad.
`LUsherOpen` throws when a launch fails, and the caller shows the failure.

## Context

Context holds the shared marks a card can carry, which are Registers, Situations and Tags.
Each is independent data owned by nothing, and any number of Meanings and Collocations reference it by id.
The order a mark appears in lives on each reference rather than on the mark.
The name or title is visible data and never identity, so renaming it leaves the id and every reference untouched.
Registers and Situations carry `LStateValue`.
So an empty field says whether nothing was recorded or the user marked it not known.
Each of the three kinds has the same four shapes.
The stored record is `LRegister`, `LSituation` or `LTag`.
The draft record is what a card carries.
The offer record is what a field keeps after a gate.
The row record is one offered line.

## Stamp

Commit: `c2f335c331fe`
- `LPhonograph.cs` `493ee502280877e8`
- `LUsher.cs` `3f38471976099398`
- `Llyn.Core.csproj` `af732680c4489b17`
