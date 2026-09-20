# LRigFactory.cs

## `public static class LRigFactory`

The one place `new L*Archive`, `new L*Loader`, `new L*File` and `new L*Http` are written.
The composition root calls it once at start and once per workspace change, and hands the rig to the engine.
The engine itself contains the use cases and builds none of the adapters they run on.

## `public static LRig LRigFactoryBuild(string workspace, HttpClient client)`

Builds every adapter over the workspace at `workspace` and bundles them as one rig.
The path must be fully qualified, so a bare name never lands beside whatever folder the process runs from.
The folder is created when missing, so a fresh workspace opens as an empty one.
One database stands behind every archive, and the client handed in behind every fetcher and download.
The caller owns the client and disposes it, so the factory holds nothing across builds.
The test suite hands in a client over a stub handler, so an engine-level search runs offline.
Nothing here is opened or read: the engine runs the doctor, realm and settings reads on the rig it receives.
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
