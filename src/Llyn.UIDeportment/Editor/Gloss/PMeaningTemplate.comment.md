# PMeaningTemplate.cs

## `public class PMeaningTemplate : ResourceDictionary`

The meaning card as a template.
A template lives in a dictionary rather than in the panel it fills.
So the panel keeps its own layout.
The events a card raises belong to the panel all the same.
This class exists only to hand them back to it.

The host's fill subscribes each forwarder on the realized part, where an event attribute stood before.

## `internal PMeaningTemplate(PEditor host)`

Merges the markup the Veneer holds, since the dictionary carries no class of its own there.
