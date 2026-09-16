# TInterfaceMeaning.cs

## `internal static partial class TInterface`

The relays for the stores of the meaning side of an entry.
That is the meanings and collocations themselves, and the registers, situations, tags and translations hung on them.
They open a store and run one store operation.
Each relay is transparent and carries no test logic of its own.
The stores of the entry itself are relayed in `TInterfaceDatabase.cs`.
