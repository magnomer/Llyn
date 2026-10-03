# TEditor.cs
Hash: `ef5643e711597398`

## `public sealed class TEditor`

Covers the entry editor's gates end to end on a real workspace.
An open holds the entry.
The fresh drafts live in `TEditorFresh`.
Every open announces the shaped content once.
A save reopens what it stored, but a fresh input draft reopens blank.
An unchanged save stores nothing, and a finish without storing lets the tenure go.
An undo drops the typing by reopening the stored entry.
Field gates write the headword, the trimmed note and the reading, and an empty language changes nothing.
The tenure's variety set tags the draft's primary reading, and a blank one or missing row changes nothing.
The reads answer empty on an empty desk, and an added etymon reads back by headword.
A field edit made while the desk fills its controls is dropped.

## `internal static LEntry TEditorEntryPrepare(LEngine engine)`

Stores the English entry water with one card, so an editor has something to open.
`TEditorFresh` shares it.

## `internal static CEditor TEditorPrepare(LEngine engine, string tab)`

Builds an editor over the engine and restores the vista of the named tab, ordered by headword.
`TEditorFresh` shares it.
