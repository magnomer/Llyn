# LRigFactorySource.cs
Hash: `53b6af5940676cc4`

## `public static class LRigFactorySource`

The sub-root that builds the source group of the rig.
It names only the fetchers and the recording archive, so `LRigFactory` stays free of them.

## `public static LRigSource LRigSourceBuild(HttpClient client, string root)`

Builds every fetcher over `client` and the recording archive over `root` and `client`.
The caller owns the client, so nothing here holds it across builds.
Nothing here is opened or read.
