# LRigFactoryCitation.cs
Hash: `df638484473ad0ef`

## `public static class LRigFactoryCitation`

The sub-root that builds the citation group of the rig.
It names only the citation archives, so `LRigFactory` stays free of them.

## `public static LRigCitation LRigCitationBuild(LDatabase database)`

Builds every citation archive over `database` and bundles them as one group.
The one database the factory opened stands behind each archive.
Nothing here is opened or read.
