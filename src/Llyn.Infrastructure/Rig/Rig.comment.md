# Rig

This folder holds the composition of every adapter into the rig.
It carries the factory and its group sub-roots.

## `LRigFactory`

This is the one place `new L*Archive`, `new L*Loader`, `new L*File` and `new L*Http` are written, beside its group sub-roots.
The composition root calls it once at start and once per workspace change.
It hands the rig to the engine.
The engine holds the use cases and builds none of the adapters they run on.

## `LRigFactory.LRigFactoryBuild`

It builds every adapter over the workspace and bundles them as one rig.
The usher, press, printer queue, warrant and phonograph are handed in.
The host's adapters for them live in Core.Windows, which this project never references.
The path must be fully qualified, so a bare name never lands beside the process's current folder.
The folder is created when missing, so a fresh workspace opens as an empty one.

One database stands behind every archive.
The lexicon, sentence, context, sound, citation and keeping groups are built over that database by their sub-roots.
The draft group is built over the root.
The asset group is built over the root and the theme.
The source group is built over the client and the root.
The posture file and the print setting file both keep their JSON under the root.
The posture adapter is built over a keep file of its own.
So the posture lands under the root beside the settings.

The theme is loaded once and handed to both the portrait and livery adapters.
The export and the Joplin notes therefore wear the same theme by construction.

The client handed in stands behind every fetcher and every download.
The caller owns the client and disposes it, so the factory holds nothing across builds.
The Joplin outpost owns its own client, so the token never meets a redirect or a proxy.
The test suite hands in a client over a stub handler, so an engine-level search runs offline.

No workspace file is opened or read here.
The engine runs the doctor and settings reads on the rig it receives.
A folder that cannot be opened therefore fails in the engine, before the old rig is let go.

The trail, the clock and the process id are read here.
The root is the one place outside the rings that may touch them.

## `LRigFactory.LRigClientCreate`

It builds the client the composition root hands to every rig.
The timeout and browser-like agent suit the sources.
The root builds it once and passes the same one to each rig.
A workspace change therefore leaks no socket pool.

## Inline notes

### LRigFactory.cs: private const long LRigFactoryCeiling

This is the most bytes one response may hold before the client refuses it.
No source hands back a page or a recording bigger than that.
An unbounded one would fill memory.

### LRigFactory.cs: client.DefaultRequestHeaders.UserAgent.ParseAdd

Cambridge and some Wiktionary edge caches reject requests without a browser-like agent.

## Stamp

Commit: `aae5349dbb94`
- `LRigFactory.cs` `020585819087890d`
