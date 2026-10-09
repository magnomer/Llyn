# LRigFactoryAsset.cs
Hash: `1b0e0e9d6cf95305`

## `public static class LRigFactoryAsset`

The sub-root that builds the asset group of the rig.
It names only the asset adapters, so `LRigFactory` stays free of them.

## `public static LRigAsset LRigAssetBuild(string root, LTheme theme)`

Builds every asset adapter and bundles them as one group.
The manifest lands under `root`, and the portrait and livery wear the one `theme` handed in.
Nothing here is opened or read.
