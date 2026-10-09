# LRigCitation.cs
Hash: `b5f32e8248f44af8`

## `public sealed record LRigCitation(LAuthorVault LRigCitationAuthors, LImageVault LRigCitationImages, LReferenceVault LRigCitationReferences, LVideoVault LRigCitationVideos)`

The citation group of the rig, holding the ports that record where a fact came from.
It covers authors, images, references and videos.
`LRig` holds it as `LRigCitation`, so the rig stays small while each port keeps its own slot.
Every property is named `LRigCitation{Base}`, the base being the port's own, as on the rig.
The composition root builds it through `LRigFactoryCitation`, and a test builds it from fakes.
