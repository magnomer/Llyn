# TEditor.cs

## `public sealed class TEditor`

Covers the entry editor's gates end to end on a real workspace.
An open holds the entry, and a null open holds a fresh draft.
A missing entry falls back to a fresh draft.
Every open announces the shaped content once, and a fresh draft holds one card of each kind.
A save reopens what it stored, but a fresh input draft reopens blank.
An unchanged save stores nothing, and a finish without storing lets the tenure go.
An undo drops the typing by reopening the stored entry.
Field gates write the headword, the trimmed note and the reading, and an empty language changes nothing.
A variety lands on the primary reading the draft holds, and a blank one or a missing row changes nothing.
The reads answer empty on an empty desk, and an added etymon reads back by headword.
A field edit made while the desk fills its controls is dropped.
The seed gates link a tag or register to the first card, and cite a source in its first sentence.
A fresh occurrence start opens a draft already linked to its Situation, and a blank one without.
A fresh quotation start replaces the held draft with one already citing its Example.
A recording search over the held draft streams the source, the recording and the end to the sink.

## `private const string TEditorClipPack`

A one-source pack whose recording source answers one British recording.

## `private static LEntry TEditorEntryPrepare(LEngine engine)`

Stores the English entry water with one card, so an editor has something to open.

## `private static CEditor TEditorPrepare(LEngine engine, string tab)`

Builds an editor over the engine and restores the vista of the named tab, ordered by headword.
