# LRigFactoryDraft.cs
Hash: `dce49fea44c85c7d`

## `public static class LRigFactoryDraft`

The sub-root that builds the draft group of the rig.
It names only the draft archives, so `LRigFactory` stays free of them.

## `public static LRigDraft LRigDraftBuild(string root)`

Builds every draft archive over `root` and bundles them as one group.
The workspace root the factory resolved stands behind each archive.
Nothing here is opened or read.
