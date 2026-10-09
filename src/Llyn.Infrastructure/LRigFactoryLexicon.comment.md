# LRigFactoryLexicon.cs
Hash: `3611aceaf4aa85c7`

## `public static class LRigFactoryLexicon`

The sub-root that builds the lexicon group of the rig.
It names only the lexicon archives, so `LRigFactory` stays free of them.

## `public static LRigLexicon LRigLexiconBuild(LDatabase database)`

Builds every lexicon archive over `database` and bundles them as one group.
The one database the factory opened stands behind each archive.
Nothing here is opened or read.
