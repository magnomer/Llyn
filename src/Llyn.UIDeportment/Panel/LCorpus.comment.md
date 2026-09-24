# LCorpus.cs

## `public sealed class LCorpus`

The corpus panel's deportment: the example vista, the quotation vista and the desk over the example draft.
The example side finds rows and their usage, and takes the query, order and kind filter.
It also loads or deletes the chosen Example.
The quotation side finds the entries of the chosen Example and loads the chosen entry.
The desk holds the example tenure the transcript editor edits, under the `Example` scope.
The panel's mode, its bin and its scribe toggle stay in the veneer until the tabs share one panel deportment.

## `public LEditor LCorpusEditor { get; }`

The entry editor's deportment on the quotation side, which takes the quotation vista when the panel's vistas are restored.

## `public LDesk LCorpusDesk { get; }`

The desk over the example draft, started by subject rather than by a vista.
The veneer attaches its observers to the tenure the desk announces, as the entry editor does.

## `public void LCorpusStart(long? id)`

Starts the desk on a stored Example or on nothing, from the `Corpus` origin the recovered draft names.

## `public void LCorpusTranscriptSet(bool editing)`

Marks the example vista as edited or read, which the posture keeps across runs.

## `public void LCorpusEditorSet(bool editing)`

Marks the quotation vista as edited or read, for the entry editor on the right.

## `public int LCorpusUsageRead(long? id)`

How many entries cite the given Example, read fresh from the engine.
Zero when no Example is given.

## `public void LCorpusCitationSet(string title)`

Points the Example's citation at the Reference the engine resolves the typed title to.
The engine reads the Reference cited now off the held draft, so an unchanged title keeps it.
