# LRepertoire.cs

## `public sealed class LRepertoire`

The repertoire panel's deportment: the situation vista, the occurrence vista and the desk over the situation draft.
The situation side finds rows and their usage, and takes the query, order and kind filter.
It also loads or deletes the chosen Situation.
The occurrence side finds the entries of the chosen Situation and loads the chosen entry.
The desk holds the situation tenure the scenario editor edits, under the `Situation` scope.
The panel's mode, its bin and its scribe toggle stay in the veneer until the tabs share one panel deportment.

## `public LEditor LRepertoireEditor { get; }`

The entry editor's deportment on the occurrence side, which takes the occurrence vista when the panel's vistas are restored.

## `public LDesk LRepertoireDesk { get; }`

The desk over the situation draft, started by subject rather than by a vista.
The veneer attaches its observers to the tenure the desk announces, as the entry editor does.

## `public void LRepertoireStart(long? id)`

Starts the desk on a stored Situation or on nothing, from the `Repertoire` origin the recovered draft names.

## `public void LRepertoireScenarioSet(bool editing)`

Marks the situation vista as edited or read, which the posture keeps across runs.

## `public void LRepertoireEditorSet(bool editing)`

Marks the occurrence vista as edited or read, for the entry editor on the right.
