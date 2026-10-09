# LRigDraft.cs
Hash: `3d1e48eaa8039edd`

## `public sealed record LRigDraft(LDraftVault LRigDraftDrafts, LClaimVault LRigDraftClaims, LCourtVault LRigDraftCourts)`

The draft group of the rig, holding the ports that keep drafts, the claims on them and the courts.
It covers the drafts, claims and courts.
`LRig` holds it as `LRigDraft`, so the rig stays small while each port keeps its own slot.
Every property is named `LRigDraft{Base}`, the base being the port's own, as on the rig.
The composition root builds it through `LRigFactoryDraft`, and a test builds it from fakes.
