# Rig

This folder holds the bundle of ports the engine is built over.
Its nine group records live in `Compartment`.

## `LRig`

The bundle of ports the engine is built over.
It is one record and one constructor argument.
The composition root assembles it through `LRigFactory` in Infrastructure, and a test assembles it from fakes.
The engine copies each port into a field of its own and never learns which adapter stands behind it.
A workspace change hands the engine a whole new rig, so every port swaps at once.
Every property is named `LRig{Base}`, the base being the port's own, so a port and its slot read alike.
There is no identity port.
The engine builds `LIdentity` over `LRigKeeping.LRigKeepingWorkspaces` itself, since it is a use case.
`LEnsign` is likewise built engine-side over `LRigUsher`, since it is a cache and not an adapter.
The root hands in the printing surface `LRigPress` beside the usher, since both are media.
It hands in the printer list `LRigQueue` beside `LRigPress`, since both serve printing.
`LRigPressVault` keeps the print setting, built from the workspace root beside the posture file.
`LRigWarrant` is the token protector, handed in by the root since protection depends on the operating system.
`LRigPhonograph` is the player, handed in by the root for the same reason.
`LRigOutpost` is the Joplin push port, set beside the fetch ports since it too speaks HTTP.
It owns the manifest of what was last pushed, so no manifest port sits in the rig.
`LRigDraft` is the dossier port, which keeps drafts with their claims and court links in the workspace draft folder.
`LRigTrail` and `LRigClock` are the two ambient facts the engine may not read itself, the path rules and the time.
`LRigLease` is the workspace lease port, built over the same root as the other root-bound adapters.
The engine opens it only when built as leased.
A workspace change swaps it with the rest of the rig.
`LRigEntryQuery` is the read-only entry query port, set beside `LRigEntries` since both cover entries.
`LRigWorkspace` is the root every root-bound adapter was built over, kept so the engine can say where it stands.

## Stamp

Commit: `1f3467bbe5c6`
- `LRig.cs` `6db9b61e91677f20`
