# PImageTemplate.cs

## `public class PImageTemplate : ResourceDictionary`

The picture row dictionary, loaded from its Veneer markup and merged by the entry and situation editors.
The editors subscribe the row's clicks themselves, so this holds no forwarder.

## `internal PImageTemplate()`

Merges the markup the Veneer holds, since the dictionary carries no class of its own there.
