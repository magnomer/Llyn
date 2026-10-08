# QInput.cs
Hash: `3268e212c7a17a65`

## `internal sealed class QInput`

The input panel's driver: the shared editor standing on no entry at all.
The lexical editing structure itself is the Veneer `PEditor`, which every panel that edits an entry mounts.
This panel is what that editor means here.
It is the form an entry is created through, and only created.
It never opens on a stored entry.
So a store here always writes a new one and leaves the form empty for the next.
Correcting an entry that exists is the browse-style panels' work.

## `internal QInput(FrameworkElement surface)`

Takes the veneer's page as its surface, which the window pulls by contract ID.
It builds the editor's driver over the editor the page places, pulled by its contract ID `PEditor`.
Nothing reaches Conduct before the window introduces it.

## `internal void QInputIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)`

Puts the panel to work on the input editor the atelier builds, and opens the form empty.
The atelier builds it with the input vista already restored.
A workspace move empties the form in Conduct, since the editor opens a fresh draft on `CWorkspaceOpened`.
A different folder is a different database, so whatever was typed against the old one is begun again.

## `internal void QInputExitRefine()`

Closes the editor as the window closes, before the engine goes.

## Inline notes

### `_qInputEditor.QEditorIntroduce(atelier, envoy, volume, mentionMenu, atelier.CAtelierInputCreate(envoy));`

The editor this panel attaches stands on no entry, because this panel creates them.
A form that stood on one would turn the next store into an update of it.
That is how a session's second entry used to overwrite its first.
