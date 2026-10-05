# TInterfaceDeportment.cs
Hash: `ab868b17d93d0f9e`

## `[assembly: CollectionBehavior(DisableTestParallelization = true)]`

The Windows tests build workspaces from the portable suite's fixture, whose disposal clears every SQLite pool.
The portable suite's own switch covers only its assembly, so this one repeats it here.
Two workspaces alive in parallel let one disposal close a connection the other is opening.

## `internal static class TInterfaceDeportment`

The relays for the deportment classes the Windows tests drive.
It is a class of its own rather than a part of `TInterface`.
The relay layer grows by owner and not by partial.
Each relay is transparent and carries no test logic of its own.
The posture relay builds its deportment over a real workspace folder.
The index item and caret key relays reach internal statics and make no WPF object.
The lectern fold relays hand a WPF toggle through, so their caller runs them on an STA thread.
The etymology relays reach the field's internal show and read its two faces by their place in the body.
The card position attach hangs one card in a bare list with only the number box in its row.
It lays the list out so its row exists, then hands the row painter to the real item watcher.
It returns the box the painter finds by its part name, so the caller runs it on an STA thread.
The card row relay hands transcription rows to the shared row matcher with the sheet's key and painters.
The scheme relay reaches a transcription row's internal choice refresh, which makes no WPF object.
The localization relays build the settings language choice over a given surface and refine it.
The choice is internal, so its create relay hands it back as an object, as the card relay does.
Its surface holds a WPF combo box, so the caller runs the localization relays on an STA thread.
