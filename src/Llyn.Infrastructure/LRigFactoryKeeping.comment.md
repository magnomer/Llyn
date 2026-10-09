# LRigFactoryKeeping.cs
Hash: `4c63ca8cbe665362`

## `public static class LRigFactoryKeeping`

The sub-root that builds the keeping group of the rig.
It names only the keeping archives, so `LRigFactory` stays free of them.

## `public static LRigKeeping LRigKeepingBuild(LDatabase database)`

Builds every keeping archive over `database` and bundles them as one group.
The one database the factory opened stands behind each archive.
Nothing here is opened or read.
