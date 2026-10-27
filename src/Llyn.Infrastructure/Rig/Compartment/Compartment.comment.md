# Compartment

This folder holds the factory sub-roots, each building one group of the rig.

## `LRigFactoryLexicon`, `LRigFactorySentence`, `LRigFactoryContext`, `LRigFactorySound`, `LRigFactoryCitation`, `LRigFactoryKeeping`, `LRigFactoryRehearsal`

Each sub-root builds one group of the rig over the one database the factory opened.
It names only that group's adapters, so `LRigFactory` stays free of them.
Nothing here is opened or read.
`LRigFactoryRehearsal` also takes the workspace root and the clock.
They build the locker entrance, whose deck manifest is a workspace file stamped with that clock.

## `LRigFactoryAsset`

This sub-root builds the asset group over the theme alone.
The portrait adapter wears the one theme handed in.
Nothing here is opened or read.

## `LRigFactorySource`

This sub-root builds every fetcher over the client and the recording archive over the root, client and clock.
Every page adapter gets one two-second regex patience from here.
The caller owns the client, so nothing here holds it across builds.
The recording archive also takes the clock, so its waits follow it.
Nothing here is opened or read.

## Stamp

Commit: `1f3467bbe5c6`
- `LRigFactoryAsset.cs` `47bb7430f0dd3962`
- `LRigFactoryCitation.cs` `7634dabcf93a95a6`
- `LRigFactoryContext.cs` `3ad86ac77c55c2a3`
- `LRigFactoryKeeping.cs` `1fcb4dbc484efd5f`
- `LRigFactoryLexicon.cs` `042c11d9ecf3a395`
- `LRigFactoryRehearsal.cs` `65c4d261bcdf4545`
- `LRigFactorySentence.cs` `255e9ba6957041d9`
- `LRigFactorySound.cs` `740daebef13359b6`
- `LRigFactorySource.cs` `df49b6e0d010d2ed`
