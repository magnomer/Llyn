# TInterfaceDeportment.cs

## `[assembly: CollectionBehavior(DisableTestParallelization = true)]`

The Windows tests build workspaces from the portable suite's fixture, whose disposal clears every SQLite pool.
The portable suite's own switch covers only its assembly, so this one repeats it here.
Two workspaces alive in parallel let one disposal close a connection the other is opening.

## `internal static class TInterfaceDeportment`

The relays for the deportment classes a browse panel holds.
It is a class of its own rather than a part of `TInterface`.
The relay layer grows by owner and not by partial.
Each relay is transparent and carries no test logic of its own.
The window relays build their deportments over fake ports, with no engine behind them.
The entry row relays read a flag store no test fills, so no WPF object is made.
The lectern fold relays hand a WPF toggle through, so their caller runs them on an STA thread.
