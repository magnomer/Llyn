# LRigFactoryContext.cs
Hash: `e3d3902dfb5cc9d9`

## `public static class LRigFactoryContext`

The sub-root that builds the context group of the rig.
It names only the context archives, so `LRigFactory` stays free of them.

## `public static LRigContext LRigContextBuild(LDatabase database)`

Builds every context archive over `database` and bundles them as one group.
The one database the factory opened stands behind each archive.
Nothing here is opened or read.
