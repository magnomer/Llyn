# LRigFactorySentence.cs
Hash: `476cf0db15cd6c04`

## `public static class LRigFactorySentence`

The sub-root that builds the sentence group of the rig.
It names only the sentence archives, so `LRigFactory` stays free of them.

## `public static LRigSentence LRigSentenceBuild(LDatabase database)`

Builds every sentence archive over `database` and bundles them as one group.
The one database the factory opened stands behind each archive.
Nothing here is opened or read.
