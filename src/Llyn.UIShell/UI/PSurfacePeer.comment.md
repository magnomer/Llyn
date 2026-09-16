# PSurfacePeer.cs

## `internal sealed class PSurfacePeer : FrameworkElementAutomationPeer`

The accessibility peer a text-heavy list surface hands the framework instead of the default one.
The framework otherwise makes a peer for every text block in the surface and reports each to any subscriber.
A subscriber is any process that asked the system for property changes, and several common utilities do.
Each report is one cross-process call, so a surface of four hundred text blocks costs half a second per entry.
This peer reports only the controls a person can act on, the fields and buttons, and nothing they only read.
A screen reader therefore still reaches every field and button, and the text rows are read from the window's text.
The text input service talks to a field directly and never through this peer, so typing and the IME stand.

## `internal PSurfacePeer(FrameworkElement owner)`

Wraps the surface the peer speaks for.

## `protected override AutomationControlType GetAutomationControlTypeCore()`

The surface reads as a plain group.

## `protected override string GetClassNameCore()`

The owner's type name, so an inspector still tells one surface from another.

## `protected override List<AutomationPeer> GetChildrenCore()`

The peers of the focusable controls beneath the surface, found by walking the visual tree.
Nothing below a focusable control is walked, since that control's own peer answers for its parts.

## `private static void PSurfacePeerScan(DependencyObject node, List<AutomationPeer> children)`

Walks one node's visual children, collecting a peer for each focusable control and descending past the rest.
