# TVaultIdentity.cs

## `public sealed class TVaultIdentity`

Covers `LIdentity` minting over the promise of `LWorkspaceVault` on a real workspace.
Two ids from one issuer descend, since temporary ids count down from the floor.
A fresh issuer over the same workspace continues below what the earlier one minted, and reads that as its floor.
The floor lives in the store, not in the issuer.
