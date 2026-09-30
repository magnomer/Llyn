# PInput.cs

## `public class PInput : UserControl`

The input panel as a control: the shared editor standing on no entry at all.
The lexical editing structure itself is `PEditor`, which every panel that edits an entry mounts.
This panel is what that editor means here.
It is the form an entry is created through, and only created.
It never opens on a stored entry.
So a store here always writes a new one and leaves the form empty for the next.
Correcting an entry that exists is the browse-style panels' work.

## `public PInput()`

Loads the markup the Veneer holds as its content and takes over its name scope.

## `internal PEditor PEditor`

The editor the markup places, found by name.
It is internal because the navigation tab opens the prospect list on it.

## `internal void PInputIntroduce(PWindow host)`

Puts the panel to work on the input editor the atelier builds, and opens the form empty.
The atelier builds it with the input vista already restored.
A workspace move empties the form in Conduct, since the editor opens a fresh draft on `CWorkspaceOpened`.
A different folder is a different database, so whatever was typed against the old one is begun again.

## Inline notes

### `PEditor.PEditorIntroduce(host, new QEditor(host.PWindowAtelier.CAtelierInputCreate(host.PWindowEnvoy)));`

The editor this panel attaches stands on no entry, because this panel creates them.
A form that stood on one would turn the next store into an update of it.
That is how a session's second entry used to overwrite its first.
