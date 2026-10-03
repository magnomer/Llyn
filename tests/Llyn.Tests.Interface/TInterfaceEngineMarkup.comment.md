# TInterfaceEngineMarkup.cs
Hash: `5d82c941cc9d2a0e`

## `internal static class TInterfaceEngineMarkup`

The relays for the engine's markup exchange.
That is the markup read, the target find, the import and the export.
Every relay but the import is transparent and carries no test logic of its own.

## `internal static TMarkupOutcome TEngineMarkupImport(this LEngine engine, LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)`

The engine reports only what an import left behind, so the stored entries are found again here.
A merge or a replace names its target.
A new entry is the match its headword did not have before the import.
