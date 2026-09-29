# QEditor.cs

## `internal sealed class QEditor`

The editor's driver half: it owns the Conduct editor a panel built and wires it to the `PEditor` surface.
Conduct's types stop here, so the surface keeps its controls, its Observes and its Refines.
It follows `QLectern`, the display's driver over a Conduct area.

## `internal CEditor QEditorArea { get; }`

The Conduct editor the panel's area built, through which the surface's Observes reach one gate each.

## `internal void QEditorIntroduce(PWindow host, PEditor surface)`

Subscribes each of the editor's notices to one Refine of the surface.
Every subscriber makes at most one Conduct read, so a notice fans out here and never inside a member.
The draft's writes follow the order the draft once showed in, so the sentence frame precedes the cards.
The citation list, the language menu and the volume answer each opened workspace.
The editor's own subjects arrive as area events through the one marshal handed to `CEditorObserverAttach`.
