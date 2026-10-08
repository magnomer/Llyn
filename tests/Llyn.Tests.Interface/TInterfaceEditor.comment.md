# TInterfaceEditor.cs
Hash: `832f84a17265baa9`

## `internal static class TInterfaceEditor`

The relays that build an editor gate over outlets or fakes and reach its internal members.
It is a class of its own, apart from `TInterfaceConduct` and its sibling conduct relay classes.
Each relay is transparent and carries no test logic of its own.
An editor built here gets a fresh repaint memory, since no atelier stands behind it.

## `internal static CEditor TEditorCreate(LEngine engine)`

Builds the editor over every real outlet on `engine`, with a fake envoy that answers no.
Its marshal runs each notice at once.

## `internal static CEditor TEditorCreate(LEngine engine, Action<Action> marshal)`

Builds the editor over every real outlet on `engine`, as the plain overload does, but with the given `marshal`.
A test thus chooses how the editor's notices reach it, such as holding them back.

## `internal static CEditor TEditorCreate(LEngine engine, CEnvoy envoy)`

Builds the editor over every real outlet on `engine`, as the plain overload does, but with the given `envoy`.
A test thus reads each notice the editor shows.

## `internal static CEditor TEditorCreate(LEngine engine, CPhonologyBundle phonology)`

Builds the editor over real draft and settings outlets and entry bundle on `engine`, but with the given `phonology`.
Its media port is a bare stub, so a test fakes the phonology alone.

## `internal static CEditor TEditorCreate(LDraftPort drafts, CEntryBundle entries, CPhonologyBundle phonology, LSettingsPort settings, LMediaPort media)`

Builds the editor over the given ports, with a fake envoy that answers no.

## `internal static CEditor TEditorCreate(LDraftPort drafts, CEntryBundle entries, CPhonologyBundle phonology, LSettingsPort settings, LMediaPort media, CEnvoy envoy)`

Builds the editor over the given ports and envoy, so a test fakes any one of them.
Its marshal runs each notice at once.

## `internal static void TEditorVistaRestore(this CEditor editor, LVista vista)`

Relays the restore of a saved vista into the editor, which only the navigation runs in production.

## `internal static CEntryDraft? TEditorDraftRead(this CEditor editor)`

The held draft of `editor` mapped through the folio, or null without one.
No driver reads the whole draft back, so tests inspect it through this relay.

## `internal static bool TEditorFinish(this CEditor editor, bool store)`

Relays the editor's finish, stored or dropped as `store` says.
It answers whether the finish went through.
