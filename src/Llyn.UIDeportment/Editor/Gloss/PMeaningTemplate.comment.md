# PMeaningTemplate.cs

## `public class PMeaningTemplate : ResourceDictionary`

The meaning card as a template.
A template lives in a dictionary rather than in the panel it fills.
So the panel keeps its own layout.
The panel's fill subscribes its own handlers on the realized part, so the dictionary holds none.

## `internal PMeaningTemplate()`

Merges the markup the Veneer holds, since the dictionary carries no class of its own there.
