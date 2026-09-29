# PCollocationTemplate.cs

## `public class PCollocationTemplate : ResourceDictionary`

The collocation card as a template — the same card shape a meaning uses, over the expression a collocation adds.
Like every template dictionary here, it only hands its events back to the panel.

The host's fill subscribes the two media forwarders on the realized part.
The card's drag, badge and eraser handlers are subscribed from the host directly.

## `internal PCollocationTemplate(PEditor host)`

Merges the markup the Veneer holds, since the dictionary carries no class of its own there.
