# LRig.cs

## `public sealed record LRig(`

The bundle of ports the engine is built over: one record, one constructor argument.
The composition root assembles it through `LRigFactory` in Infrastructure, and a test assembles it from fakes.
The engine copies each port into a field of its own and never learns which adapter stands behind it.
A workspace change hands the engine a whole new rig, so every port swaps at once.
There is no identity port: the engine builds `LIdentity` over `LRigWorkspaces` itself, since it is a use case.
`LEnsign` is likewise built engine-side over `LRigUsher`, since it is a cache and not an adapter.
Every property is named `LRig{Base}`, the base being the port's own, so a port and its slot read alike.
The four fetch ports follow the port's whole name, since `LRigFanqie` already names the fanqie vault.
`LRigWorkspace` is the root every root-bound adapter above was built over, kept so the engine can say where it stands.
