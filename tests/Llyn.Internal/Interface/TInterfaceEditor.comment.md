# TInterfaceEditor.cs

## `internal static class TInterfaceEditor`

The relays that build an editor gate over outlets or fakes and reach its internal members.
It is a class of its own, so `TInterfaceConduct` keeps only the other conduct relays.
Each relay is transparent and carries no test logic of its own.

## `internal static CEntryDraft? TEditorDraftRead(this CEditor editor)`

The held draft of `editor` as the draft bulletin maps it, or null without one.
No driver reads the whole draft back, so tests inspect it through this relay.
