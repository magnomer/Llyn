# TInterfaceVista.cs
Hash: `2975d230d7b4d5be`

## `internal static partial class TInterface`

The relays for a vista, the engine's view state for one catalog tab.
The start and the row finds are relayed over the engine, and every change over the vista itself.
Each relay is transparent and carries no test logic of its own.
The engine stores no filter or mode.
So the engine start relay hands it an empty filter and the reading mode.
The posture start relay resolves both from what the posture stored, as the window does.
Both pass the blank flag through with the same default the engine gives it.
The press-open relays pass only the vista and its label or legend, since the engine owns its press.
They answer a null relay when the vista has no row chosen, and the caller disposes any other.
The label overload also answers null for a vista whose subject is not Entry.
The xiaoyun relay takes an optional vista, so a test can pin the order the panel sets.

## `internal static LVista TEngineVistaStart(this LEngine engine, string tab, LCatalogOrder fallback, bool blank = false)`

Starts a vista on `tab` over the engine, in `fallback` order with an empty filter and reading mode.

## `internal static LVista TPostureVistaStart(this LPosture posture, string tab, LCatalogOrder fallback, bool blank = false)`

Starts a vista on `tab` through the posture, which restores the stored order, filter and split.

## `private static LSubject? TVistaSubjectRead(string tab)`

Answers the subject a tab lists, so both start relays need only the tab name.
The two Fanqie tabs list no subject, and any tab not named lists entries.

## `internal static IReadOnlyList<LVistaRow> TEngineEntryFind(this LEngine engine, LVista vista)`

Relays the entry rows of `vista`.

## `internal static IReadOnlyList<LCatalogPronunciation> TEnginePronunciationFind(this LEngine engine, LVista vista)`

Relays the pronunciation rows of `vista`.

## `internal static IReadOnlyList<LVistaRow> TEngineFavoriteFind(this LEngine engine, LVista vista)`

Relays the favourite rows of `vista`.

## `internal static IReadOnlyList<LCatalogTag> TEngineTagFind(this LEngine engine, LVista vista)`

Relays the tag rows of `vista`.

## `internal static IReadOnlyList<LDiwei> TEngineDiweiFind(this LEngine engine, LVista vista, long? chosen, bool final)`

Relays the diwei rows of `vista`, initials or finals as `final` says.

## `internal static IReadOnlyList<LVistaRow> TEngineXiaoyunFind(this LEngine engine, string language, IReadOnlyList<long> diweiIds, string query, LVista? vista = null)`

Relays the xiaoyun rows that match `diweiIds` and `query`.

## `internal static void TVistaOrderSet(this LVista vista, LCatalogOrder order)`

Relays the order change.

## `internal static void TVistaFilterSet(this LVista vista, LCatalogFilter filter)`

Relays the filter change by record.

## `internal static void TVistaQuerySet(this LVista vista, string query)`

Relays the typed query.

## `internal static void TVistaSelect(this LVista vista, long? id)`

Relays the row choice, where null clears it.

## `internal static void TVistaEditingSet(this LVista vista, bool editing)`

Relays the switch between reading and editing.

## `internal static LDraft? TVistaLoad(this LEngine engine, LVista vista)`

Relays the draft load of the chosen row.

## `internal static string TVistaFileRead(this LEngine engine, LVista? vista)`

Relays the suggested file name for `vista`.

## `internal static bool TVistaStoredCheck(long? id)`

Relays the check that `id` names a stored row.

## `internal static LCatalogOrder TVistaOrderRead(LVista? vista)`

Relays the order read of an optional vista.

## `internal static LCatalogFilter TVistaFilterRead(LVista? vista)`

Relays the filter read of an optional vista.

## `internal static void TVistaFilterSet(this LVista vista, IReadOnlyList<string> hidden)`

Relays the filter change by its hidden names.

## `internal static int TVistaUsageRead(this LEngine engine, LVista vista)`

Relays the usage count of the vista's stored row.

## `internal static LRevision? TVistaDelete(this LEngine engine, LVista vista)`

Relays the delete of the chosen row.

## `internal static IReadOnlyList<LVistaRow> TEngineEntryFind(this LEngine engine, LVista parent, LVista child)`

Relays the entries that use the row chosen in `parent`.
They are matched by the query, order and choice of `child`.

## `internal static IReadOnlyList<LCatalogExample> TEngineExampleFind(this LEngine engine, LVista vista, string unknown = "", string unwritten = "")`

Relays the example rows of `vista` with the two fallback words.

## `internal static IReadOnlyList<LCatalogSituation> TEngineSituationFind(this LEngine engine, LVista vista)`

Relays the situation rows of `vista`.

## `internal static IReadOnlyList<LCatalogReference> TEngineReferenceFind(this LEngine engine, LVista vista)`

Relays the source rows of `vista`.

## `internal static IReadOnlyList<LCatalogAuthor> TEngineAuthorFind(this LEngine engine, LVista vista)`

Relays the author rows of `vista`.

## `internal static IReadOnlyList<LCatalogRegister> TEngineRegisterFind(this LEngine engine, LVista vista)`

Relays the register rows of `vista`.

## `internal static bool TTenureFlaggedCheck(this LTenure tenure)`

Relays the pronunciation quill's flagged check over `tenure`.

## `internal static LTranscriptionSheet? TTenureTranscriptionRead(this LTenure tenure)`

Relays the transcription quill's read over `tenure`.

## `internal static bool TEngineFlaggedCheck(this LEngine engine, LEntryDraft draft)`

Relays the engine's flagged check of `draft`.

## `internal static Task<LGalleyRelay?> TEnginePressOpen(this LEngine engine, LVista vista, LPortraitLabel label)`

Relays the print session of the vista's chosen entry.

## `internal static Task<LGalleyRelay?> TEnginePressOpen(this LEngine engine, LVista vista, LPortraitLegend legend)`

Relays the print session of the vista's chosen catalog row.

## `internal static Task TEnginePortraitExport(this LEngine engine, LVista vista, string path, LPortraitMedium format, LPortraitLabel label)`

Relays the export of the vista's chosen entry to `path`.
