# TInterfaceEditor.cs
Hash: `f96e7e9752417a99`

## `internal static class TInterfaceEditor`

The relays that build an editor gate over outlets or fakes and reach its internal members.
It is a class of its own, apart from `TInterfaceConduct` and its sibling conduct relay classes.
Each relay is transparent and carries no test logic of its own.
An editor built here gets a fresh repaint memory, since no atelier stands behind it.

## `internal static CEditor TEditorCreate(LEngine engine, Action<Action> marshal) => new(`

Builds the editor over every real outlet on `engine`, as the plain overload does, but with the given `marshal`.
A test thus chooses how the editor's notices reach it, such as holding them back.

## `internal static CEditor TEditorCreate(LEngine engine, CEnvoy envoy) => new(`

Builds the editor over every real outlet on `engine`, as the plain overload does, but with the given `envoy`.
A test thus reads each notice the editor shows.

## `internal static CEntryDraft? TEditorDraftRead(this CEditor editor)`

The held draft of `editor` mapped through the folio, or null without one.
No driver reads the whole draft back, so tests inspect it through this relay.
