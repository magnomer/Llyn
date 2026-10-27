# Rig

This folder holds the composition of every adapter into the rig.
It carries the factory and its group sub-roots.

## `LRigFactory`

This is the one place `new L*Archive`, `new L*Loader` and `new L*File` are written, beside its group sub-roots.
It is the layer's one public composition point.
It wires the module entrances `LDossierEntrance`, `LEmbassyEntrance` and `LEngraverEntrance`, and holds no fact of its own.
The composition root makes one and calls it once at start and once per workspace change.
It disposes the factory at the end.
It hands the rig to the engine.
The engine holds the use cases and builds none of the adapters they run on.

The usher, press, printer queue, warrant and phonograph are handed to the constructor and reused by every build.
The plain constructor creates the client and owns it, so `Dispose` releases it.
The constructor taking a client leaves it with its caller, so `Dispose` leaves it open.
A test shares one that way.

## `LRigFactory.LRigFactoryBuild`

It builds every adapter over the workspace and bundles them as one rig.
The usher, press, printer queue, warrant and phonograph come from the constructor.
The host's adapters for them live in Core.Windows, which this project never references.
The path must be fully qualified, so a bare name never lands beside the process's current folder.
The folder is created when missing, so a fresh workspace opens as an empty one.

One database stands behind every archive.
The lexicon, sentence, context, sound, citation and keeping groups are built over that database by their sub-roots.
The drafts in flight are one `LDossierEntrance` over the root.
The asset group is built over the theme alone.
The source group is built over the client, the root and the clock.
The rehearsal group is built over the database, the root and the clock.
The posture file and the print setting file both keep their JSON under the root.
Each is built over a keep file of its own.
So both land under the root beside the settings.

The theme is loaded once and handed to both the portrait adapter and the engraver entrance.
The export and the Joplin notes therefore wear the same theme by construction.

The factory's client stands behind every fetcher and every download.
The same client serves every build, so a workspace change leaks no socket pool.
The embassy entrance is built without that client.
It uses the wire's own client, so the Joplin token never meets a redirect or a proxy.
A test that hands in a client over a stub handler keeps a search offline.

No workspace file is opened or read here.
The engine runs the doctor and settings reads on the rig it receives.
A folder that cannot be opened therefore fails in the engine, before the old rig is let go.

The trail and the clock are created here.
The one clock is also handed to the recording archive, so its waits follow the same clock.
The locker entrance takes the same clock to stamp the deck manifest.
The lease is built over the root but not opened here, so building a rig claims nothing.
The engine opens it when it is built as leased.

## `LRigFactory.LRigClientCreate`

It builds the client the plain constructor owns.
The timeout and browser-like agent suit the sources.
It is private, so no caller holds the factory's client.

## Inline notes

### LRigFactory.cs: private const long LRigFactoryCeiling

This is the most bytes one response may hold before the client refuses it.
No source hands back a page or a recording bigger than that.
An unbounded one would fill memory.

### LRigFactory.cs: client.DefaultRequestHeaders.UserAgent.ParseAdd

Cambridge and some Wiktionary edge caches reject requests without a browser-like agent.

## Stamp

Commit: `1f3467bbe5c6`
- `LRigFactory.cs` `cc70b8c61f627d75`
