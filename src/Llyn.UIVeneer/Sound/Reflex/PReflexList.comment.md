# PReflexList.cs

## `public sealed class PReflexList : ItemsControl`

The list the reflex rows are drawn in, on the reading view and in the editor alike.
It is a plain items control that hands the framework the surface peer instead of the default one.
So the dozens of text blocks a Han character's readings make are not reported one by one to accessibility subscribers.
The fields and buttons inside the rows stay reachable through that peer.

## `protected override AutomationPeer OnCreateAutomationPeer()`

The surface peer, which lists only the focusable controls beneath the list.
