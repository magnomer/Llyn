# LEngineDraftLeftover.cs

## `public sealed partial class LEngine`

What a launch does with the held drafts an earlier launch left behind.
One call collects what is already saved and one offers back what is not.
Both walk the whole drafts folder, which no other call here does.
The lifecycle calls they lean on live in `LEngineDraftHold.cs`.

## `public void LEngineLeftoverSweep()`

Clears what the drafts folder keeps forever, before anything counts what is left.
A draft committed just before a kill kept its file, because the commit names the stored entry first.
Its content then matches that entry, so nothing offers it back and nothing deletes it either.
Such a draft is dropped here with its claim and its court rows.
A sentence draft is measured the same way, against the Example it names rather than the entry.
A situation draft is measured against the Situation it names.
A source draft is measured against the Reference it names.
A draft naming no entry, or an entry since deleted, is left for recovery to offer back.
So is one whose content differs from the entry it names, which is work the user would lose.
A draft this engine holds, or another running copy claims, is passed over untouched.
The half-written pending files both archives leave behind go too.
Only files older than an hour, so a save in flight in the other copy is never taken.
A draft file of another version goes with its claim and its court rows, as a cancel would take them.
Such a file was written by another build, and reading it under this shape could commit a duplicate entry.
Nothing here throws on a folder that is missing or unknown.

## `public IReadOnlyList<LDraft> LEngineLeftoverRead()`

The held drafts nothing is still working on and that differ from the entry they opened from.
A draft this engine started is claimed by an open window.
Offering it back would fight the window still typing into it.
A draft another running copy of the program claims is passed over too.
The claim file that copy wrote tells it apart.
Two copies may run on one workspace, and an in-memory set cannot see across them.
A claim naming a process that is gone is what a crash leaves behind.
The draft under it is exactly what recovery is for.
A draft matching its origin carries nothing worth recovering, which is what an untouched panel leaves behind.
What remains is what a forced shutdown cost, counted at launch.
