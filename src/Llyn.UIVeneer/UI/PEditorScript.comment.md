# PEditorScript.cs

## `public partial class PEditor`

The script box of the editor: the same control as the reading view's, folded under its head.

## `internal void PEditorScriptShow()`

Reads the pack's styles for the entry's language and the entry's stored pictures.
It asks the engine to fetch what is missing first.
A fetch started here reaches the editor by bulletin like any other.
An unsaved entry has no id and shows nothing.
A failed read hands the box nothing.
