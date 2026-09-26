# TEntryClerkSaving.cs

## `public sealed class TEntryClerkSaving`

Covers the entry clerk's create over a rig of fakes and no engine.
The draft carries no language, so no pack is loaded and no paradigm is judged.

## `public void EntryClerkSave_HeadwordOnlyDraft_StoresEntryAndRecordsCreate()`

A draft with only a headword becomes a stored entry and one revision holding one create change for it.
The workspace row points at that revision.

## `public void EntryClerkSave_PronunciationDraft_WritesRowAndRecordsCreate()`

A draft carrying a reading writes its pronunciation row through the fake vault and still records one revision per save.
