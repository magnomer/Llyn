# QReflexFrame.cs

## `public sealed class QReflexFrame : Decorator`

The frame the Veneer places around the reflex list, on the reading view and in the editor alike.
It draws nothing and only hands the framework the surface peer for itself and its list.
So the dozens of text blocks a Han character's readings make are not reported one by one to accessibility subscribers.
The fields and buttons inside the rows stay reachable through that peer.
The list inside stays a plain items control, since only an element's own type can hand over a peer.

## `protected override AutomationPeer OnCreateAutomationPeer()`

The surface peer, which lists only the focusable controls beneath the framed list.
