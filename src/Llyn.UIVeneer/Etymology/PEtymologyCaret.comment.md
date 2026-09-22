# PEtymologyCaret.cs

## `internal sealed class PEtymologyCaret`

The entry at the end of the source chips, where a word is typed before it becomes a link.
It is one item of the field's collection, so the chips and the entry wrap as one line.
The word lives here rather than on the box, which is rebuilt whenever a chip is added.

## `public string PEtymologyCaretText`

The word standing in the entry, raised as it changes so the box and the field agree.

## `public event PropertyChangedEventHandler? PropertyChanged`

How the drawn box learns the word was cleared.
