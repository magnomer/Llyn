# Compartment

This folder holds the factory sub-roots, each building one group of the rig.

## `LRigFactoryLexicon`, `LRigFactorySentence`, `LRigFactoryContext`, `LRigFactorySound`, `LRigFactoryCitation`, `LRigFactoryKeeping`, `LRigFactoryRehearsal`

Each sub-root builds one group of the rig over the one database the factory opened.
It names only that group's adapters, so `LRigFactory` stays free of them.
Nothing here is opened or read.
`LRigFactoryRehearsal` also takes the workspace root, since the deck manifest is a workspace file.

## `LRigFactoryDraft`

This sub-root builds the draft group over the workspace root the factory resolved.
It names only the draft archives.
Nothing here is opened or read.

## `LRigFactoryAsset`

This sub-root builds the asset group.
The manifest lands under the root.
The portrait and livery wear the one theme handed in.
Nothing here is opened or read.

## `LRigFactorySource`

This sub-root builds every fetcher over the client and the recording archive over the root, client and clock.
Every page adapter gets one two-second regex patience from here.
The caller owns the client, so nothing here holds it across builds.
Nothing here is opened or read.

## Stamp

Commit: `9377e657b395`
- `LRigFactoryAsset.cs` `1b0e0e9d6cf95305`
- `LRigFactoryCitation.cs` `df638484473ad0ef`
- `LRigFactoryContext.cs` `e3d3902dfb5cc9d9`
- `LRigFactoryDraft.cs` `dce49fea44c85c7d`
- `LRigFactoryKeeping.cs` `d2b3c98cc215a0a2`
- `LRigFactoryLexicon.cs` `222aad69e5594ba2`
- `LRigFactoryRehearsal.cs` `bf1eb3024ebfd474`
- `LRigFactorySentence.cs` `476cf0db15cd6c04`
- `LRigFactorySound.cs` `d903e45a869d5e10`
- `LRigFactorySource.cs` `767b380f920c1702`
