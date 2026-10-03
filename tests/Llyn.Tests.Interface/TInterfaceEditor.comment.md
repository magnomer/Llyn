# TInterfaceEditor.cs
Hash: `d1bf4d925a91b1e2`

## `internal static class TInterfaceEditor`

The relays that build an editor gate over outlets or fakes and reach its internal members.
It is a class of its own, apart from `TInterfaceConduct` and its sibling conduct relay classes.
Each relay is transparent and carries no test logic of its own.

## `internal static CEntryDraft? TEditorDraftRead(this CEditor editor)`

The held draft of `editor` mapped through the folio, or null without one.
No driver reads the whole draft back, so tests inspect it through this relay.
