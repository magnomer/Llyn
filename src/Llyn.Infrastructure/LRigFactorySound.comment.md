# LRigFactorySound.cs
Hash: `d903e45a869d5e10`

## `public static class LRigFactorySound`

The sub-root that builds the sound group of the rig.
It names only the sound archives, so `LRigFactory` stays free of them.

## `public static LRigSound LRigSoundBuild(LDatabase database)`

Builds every sound archive over `database` and bundles them as one group.
The one database the factory opened stands behind each archive.
Nothing here is opened or read.
