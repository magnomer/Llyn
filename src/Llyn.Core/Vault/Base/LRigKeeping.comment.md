# LRigKeeping.cs
Hash: `7d7a1a8baf949f4b`

## `public sealed record LRigKeeping(LDoctorVault LRigKeepingDoctor, LRevisionVault LRigKeepingRevisions, LWorkspaceVault LRigKeepingWorkspaces, LTombstoneVault LRigKeepingTombstones, LFavoriteVault LRigKeepingFavorites, LNoteVault LRigKeepingNotes)`

The keeping group of the rig, holding the ports that keep the workspace and what users saved in it.
It covers the doctor, revisions, workspaces, tombstones, favorites and notes.
`LRig` holds it as `LRigKeeping`, so the rig stays small while each port keeps its own slot.
Every property is named `LRigKeeping{Base}`, the base being the port's own, as on the rig.
The composition root builds it through `LRigFactoryKeeping`, and a test builds it from fakes.
