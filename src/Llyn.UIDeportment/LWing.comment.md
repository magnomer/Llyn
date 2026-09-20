# LWing.cs

## `public sealed class LWing`

The deportment of one side of the duplex panel: the entry vista it holds and the reading view under it.
It finds the rows, takes the query, order and language sieve, and loads the chosen entry.
The reading view's deportment is built here, since a wing has no editor to own it.
The chosen entry is saved to the workspace state under the wing's side, so the next run reopens it.

## `public void LWingEntrySave()`

Writes the chosen entry to the left or right slot of the workspace state, whichever side this wing is.
The vista itself says which side it is, so nothing here copies that.
