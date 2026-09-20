# LDraftPort.cs

## `public interface LDraftPort`

The slice of the engine a deportment sees when it holds or links a draft.
It starts a tenure, drops a draft, attaches an observer, and answers the link lookups a card edit asks.
It also divides a sentence around its Mentions, converts text offsets, and toggles or matches anchors.
`LEngine` implements it today, and the draft clerk takes it over when the parts are dismantled.
Member names keep the engine's `LEngine*` form until that hand-over renames them once.

## `LTenure LEngineTenureStart(LVista vista, long? id);`

Starts a tenure over the entry the vista chose, or a fresh entry when `id` is null.

## `LTenure LEngineTenureStart(string origin, LSubject subject, long? id);`

Starts a tenure for a non-entry subject from a named origin, as the corpus and repertoire holds do.

## `IReadOnlyList<LVistaRow> LEngineProspectFind(string query);`

The entries a typed translation may link to, one row per match plus one per language for a new entry.
