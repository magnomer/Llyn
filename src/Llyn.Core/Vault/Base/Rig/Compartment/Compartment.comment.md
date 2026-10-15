# Compartment

This folder holds the ten group records of the rig.

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
