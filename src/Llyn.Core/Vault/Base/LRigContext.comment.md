# LRigContext.cs
Hash: `4019730f09188fc7`

## `public sealed record LRigContext(LRegisterVault LRigContextRegisters, LSituationVault LRigContextSituations, LTagVault LRigContextTags)`

The context group of the rig, holding the ports that say where and how a word is used.
It covers registers, situations and tags.
`LRig` holds it as `LRigContext`, so the rig stays small while each port keeps its own slot.
Every property is named `LRigContext{Base}`, the base being the port's own, as on the rig.
The composition root builds it through `LRigFactoryContext`, and a test builds it from fakes.
