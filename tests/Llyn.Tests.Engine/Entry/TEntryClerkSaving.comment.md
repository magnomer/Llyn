# TEntryClerkSaving.cs
Hash: `4da0af21547a1615`

## `public sealed class TEntryClerkSaving`

Covers the entry clerk's create over a rig of fakes.
Where the fakes answer nothing, a workspace engine stands in.
A draft without a language loads no pack and judges no paradigm.

## `public void EntryClerkSave_HeadwordOnlyDraft_StoresEntryAndRecordsCreate()`

A draft with only a headword becomes a stored entry and one revision holding one create change for it.
The workspace row points at that revision.

## `public void EntryClerkSave_PronunciationDraft_WritesRowAndRecordsCreate()`

Two saves, the first carrying a reading, store distinct entries and record one revision per save.
The reading lands as a row of the first entry only.
The first revision holds that entry's create change and then the reading's create change.

## `public void EntryClerkSave_FullDraft_RecordsEveryChildCreate()`

A new entry saved with a meaning, a transcription, a reflex and an etymology records one revision.
It holds the entry's create first and then each child's create, in the order the save writes them.
The rig fakes answer no meaning, so this one runs through a workspace engine.
