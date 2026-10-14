# Rig

This folder holds the bundle of ports the engine is built over and its ten group records.

## `LRig`

The bundle of ports the engine is built over.
It is one record and one constructor argument.
The composition root assembles it through `LRigFactory` in Infrastructure, and a test assembles it from fakes.
The engine copies each port into a field of its own and never learns which adapter stands behind it.
A workspace change hands the engine a whole new rig, so every port swaps at once.
Every property is named `LRig{Base}`, the base being the port's own, so a port and its slot read alike.
There is no identity port.
The engine builds `LIdentity` over `LRigKeeping.LRigKeepingWorkspaces` itself, since it is a use case.
`LEnsign` is likewise built engine-side over `LRigUsher`, since it is a cache and not an adapter.
The root hands in the printing surface `LRigPress` beside the usher, since both are media.
It hands in the printer list `LRigQueue` beside `LRigPress`, since both serve printing.
`LRigPressVault` keeps the print setting, built from the workspace root beside the posture file.
`LRigWarrant` is the token protector, handed in by the root since protection depends on the operating system.
`LRigPhonograph` is the player, handed in by the root for the same reason.
`LRigOutpost` is the Joplin push port, set beside the fetch ports since it too speaks HTTP.
`LRigAsset.LRigAssetManifest` remembers what was last pushed to Joplin and is kept with the other workspace files.
`LRigAsset.LRigAssetLivery` answers the style note Joplin imports, set beside the portrait port since both wear the theme.
`LRigTrail` and `LRigClock` are the two ambient facts the engine may not read itself, the path rules and the time.
`LRigEntryQuery` is the read-only entry query port, set beside `LRigEntries` since both cover entries.
`LRigProcess` is the id of the process the rig was built in.
A claim on a draft is compared against it.
`LRigWorkspace` is the root every root-bound adapter was built over, kept so the engine can say where it stands.

## Rig groups

Ten records group the ports, so the rig stays small while each port keeps its own slot.
`LRig` holds each as `LRig{Group}`, and each property is named `LRig{Group}{Base}`.
The composition root builds each through `LRigFactory{Group}`, and a test builds it from fakes.
Each group lets a consumer of one kind of port read them through one slot.

## `LRigAsset`

The ports that read and write workspace and theme files.
It covers the manifest, localization, markup, portrait and livery.

## `LRigCitation`

The ports that record where a fact came from.
It covers authors, images, references and videos.

## `LRigContext`

The ports that say where and how a word is used.
It covers registers, situations and tags.

## `LRigDraft`

The ports that keep drafts, the claims on them and the courts.

## `LRigKeeping`

The ports that keep the workspace and what users saved in it.
It covers the doctor, revisions, workspaces, tombstones, favorites, fold state and notes.

## `LRigLexicon`

The ports that describe a word itself.
It covers etymology maps, collocations, glosses, inflections, lacunae, meanings, morphologies and parts of speech.

## `LRigRehearsal`

The ports that keep flashcard study.
It covers decks with their bearings, deck folders, cues with their cells, and stencils.
It also holds the phone folder port and the deck manifest store.
Rehearsal points at lexical rows, and no lexical port points back.

## `LRigSentence`

The ports that describe a sentence and what hangs on it.
It covers examples, mentions, sentences and translations.

## `LRigSound`

The ports that describe how a word is pronounced.
It covers diwei, fanqie, shengfu, stems, frequencies, pronunciations, reflexes, scripts and transcriptions.

## `LRigSource`

The ports that fetch pronunciation data from outside.
It covers the source factory, the four fetchers and the recordings.

## Stamp

Commit: `aae5349dbb94`
- `LRig.cs` `84137e92a3f10901`
- `LRigAsset.cs` `353ad140ea730368`
- `LRigCitation.cs` `b5f32e8248f44af8`
- `LRigContext.cs` `4019730f09188fc7`
- `LRigDraft.cs` `3d1e48eaa8039edd`
- `LRigKeeping.cs` `e2b8f853ca537642`
- `LRigLexicon.cs` `2e1e7ac85cf3fa00`
- `LRigRehearsal.cs` `1fbbc0696ed26fd3`
- `LRigSentence.cs` `0efa207365b2e66c`
- `LRigSound.cs` `8226c2ce0a371822`
- `LRigSource.cs` `443817afa7375534`
