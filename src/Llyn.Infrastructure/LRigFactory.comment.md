# LRigFactory.cs
Hash: `b4666bd8d6c40b5e`

## `public static class LRigFactory`

The one place `new L*Archive`, `new L*Loader`, `new L*File` and `new L*Http` are written, beside its group sub-roots.
The composition root calls it once at start and once per workspace change, and hands the rig to the engine.
The engine itself contains the use cases and builds none of the adapters they run on.

## `public static LRig LRigFactoryBuild(string workspace, HttpClient client, LUsher usher, LPress press, LWarrant warrant, LPhonograph phonograph)`

Builds every adapter over the workspace at `workspace` and bundles them as one rig.
The usher, the press, the warrant and the phonograph are handed in.
All live in Core.Windows, which this project never references.
The path must be fully qualified, so a bare name never lands beside whatever folder the process runs from.
The folder is created when missing, so a fresh workspace opens as an empty one.
One database stands behind every archive.
The lexicon group is built by `LRigFactoryLexicon` over that database.
The sentence group is built by `LRigFactorySentence` and the context group by `LRigFactoryContext` the same way.
The sound group is built by `LRigFactorySound` over that database.
The citation group is built by `LRigFactoryCitation` and the keeping group by `LRigFactoryKeeping` over that database.
The draft group is built by `LRigFactoryDraft` over the root.
The asset group is built by `LRigFactoryAsset` over the root and the theme.
The source group is built by `LRigFactorySource` over the client and the root.
The client handed in stands behind every fetcher and every download.
The Joplin outpost owns its own client, so the token never meets a redirect or a proxy.
The caller owns the client and disposes it, so the factory holds nothing across builds.
The test suite hands in a client over a stub handler, so an engine-level search runs offline.
Nothing here is opened or read.
The engine runs the doctor and settings reads on the rig it receives.
The theme is loaded once and handed to both the portrait and livery adapters.
The export and the Joplin notes therefore wear the same theme by construction.
The posture adapter is built over a keep file of its own.
So the posture lands beside the settings as before.
The trail, the clock and the process id are read here.
The root is the one place outside the rings that may touch them.
A folder that cannot be opened therefore fails in the engine, before the old rig is let go.

## `public static HttpClient LRigClientCreate()`

Builds the client the composition root hands to every rig, with the timeout and browser-like agent the sources need.
The root builds it once and passes the same one to each rig.
A workspace change therefore leaks no socket pool.

## Inline notes

### `private const long LRigFactoryCeiling = 8L * 1024 * 1024;`

The most bytes one response may hold before the client refuses it.
No source hands back a page or a recording bigger than that.
An unbounded one would fill memory.

### `client.DefaultRequestHeaders.UserAgent.ParseAdd(`

Cambridge (and some Wiktionary edge caches) reject requests without a browser-like agent.
