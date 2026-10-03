# TInterfaceConductPanel.cs
Hash: `e5d3e89585c69b40`

## `internal static class TInterfaceConductPanel`

The relays for Conduct's panel area and the catalog maps its panels read.
They reach the panel, the oeuvre, the occurrence list, the atlas, the example list, the footnote and the imprint.
They also reach each panel's offered file name, its leave question and its startup editor restore.
Each relay is transparent and carries no test logic of its own.

## `internal static bool TCatalogFilterMatch(this CCatalogFilter filter, string? language) =>`

Relays whether a catalog filter admits a row of the given language.

## `internal static CVistaRow TPanelRowRead(LVistaRow row)`

Relays the internal entry row map.

## `internal static CPanel TPanelCreate(CEnvoy envoy, string? deleteScope, Func<bool> changeSeam, Func<bool, bool> finishSeam) =>`

Builds a bare panel over the fake settings port, with the given delete scope and change and finish seams.
Its load-failed key is fixed and its last seam always answers yes.
A test thus varies only the seams it passes.

## `internal static void TPanelVistaRestore(this CPanel panel, LVista vista) => panel.CPanelVistaRestore(vista);`

Relays the panel's restore of a saved vista, which only the navigation runs in production.

## `internal static LVista? TPanelVistaRead(this CPanel panel) => panel.CPanelVista;`

Reads the vista the panel currently holds, or nothing when it holds none.

## `internal static CCatalogOrder TPanelOrderRead(LCatalogOrder order) => CPanel.CPanelOrderRead(order);`

Relays the panel's turning of a stored catalog order into the one its list shows.

## `internal static LCatalogOrder? TPanelOrderRead(CCatalogOrder? order) => CPanel.CPanelOrderRead(order);`

Relays the panel's turning of a shown catalog order back into the stored one.
A missing order stays missing.

## `internal static CCatalogFilter TPanelFilterRead(LCatalogFilter filter) => CPanel.CPanelFilterRead(filter);`

Relays the panel's turning of a stored catalog filter into the one its list shows.

## `internal static LSubject TPanelSubjectRead(CSubject subject) => CPanel.CPanelSubjectRead(subject);`

Relays the panel's turning of a shown subject into the stored one.

## `internal static COeuvre TOeuvreCreate(LEngine engine)`

Builds the oeuvre over a real entry outlet on `engine`, as the guild does.

## `internal static void TOeuvreVistaRestore(this COeuvre oeuvre, LVista roll, LVista vista)`

Binds the oeuvre to the two vistas through its internal helper, as the guild's restore does.

## `internal static IReadOnlyList<CCatalogAuthor> TOeuvreAuthorRead(this COeuvre oeuvre, IReadOnlyList<LCatalogAuthor> rows)`

Maps roll rows through the oeuvre's internal helper, as the guild's roll read does.

## `internal static COccurrence TOccurrenceCreate(LEngine engine)`

Builds the occurrence list over real outlets on `engine`, as the repertoire does.

## `internal static void TOccurrenceVistaRestore(this COccurrence occurrence, LVista roll, LVista vista)`

Binds the occurrence list to the situation vista and its own, as the repertoire's restore does.

## `internal static void TAtlasVistaRestore(this CAtlas atlas, LVista vista)`

Binds the atlas to the situation vista through its internal helper, as the repertoire's restore does.

## `internal static IReadOnlyList<CCatalogSituation>? TAtlasRowsRead(this CAtlas atlas)`

Reads the atlas rows through its internal helper, without the repertoire's stale-selection close.

## `internal static IReadOnlyList<CCatalogSituation>? TAtlasFailRead(LEngine engine, CEnvoy envoy)`

Reads the atlas rows over an entry port whose situation find throws, and answers what the read answers.

## `internal static CSituationDraft? TAtlasDraftRead(LSituation? situation)`

Wraps the Situation in a bare draft and maps it through the atlas's internal helper.
A null Situation reads as no draft at all.

## `internal static CSituation? TAtlasSituationRead(LSituation? situation)`

Wraps the Situation in a bare draft and maps it to the vignette's ready page through the atlas.
A markdown port runs the real parser, so the description's blocks are real.

## `private static LDraft? TAtlasDraftCreate(LSituation? situation)`

The bare repertoire draft both atlas relays hand on.
A null Situation reads as no draft at all.

## `internal static CAnthology TAnthologyCreate(CAtelier atelier, CDesk desk, CEnvoy envoy)`

Builds the example list through its internal factory, always shown and always finishing.

## `internal static void TAnthologyVistaRestore(this CAnthology anthology, LVista vista)`

Hands the example list its vista through the internal helper the corpus calls.

## `internal static IReadOnlyList<CCatalogExample> TAnthologyRowsRead(this CAnthology anthology)`

Reads the example rows through their internal helper, without the corpus's stale-selection close.
A failed read throws, so a test never mistakes a failure for an empty list.

## `internal static CExample? TAnthologyExampleRead(LExample? example, string citation, string tally)`

Maps a stored Example and a ready citation line through the list's internal helper.

## `internal static void TFootnoteVistaRestore(this CFootnote footnote, LVista parent, LVista vista) =>`

Relays the footnote's restore of its parent vista and its own, which only the navigation runs in production.

## `internal static void TFootnoteEntryCreate(this CFootnote footnote) => footnote.LFootnoteEntryCreate();`

Relays the footnote's start of a new entry.

## `internal static void TImprintOpen(this CImprint imprint, long? id) => imprint.LImprintOpen(id);`

Relays the imprint's opening of the given record, or of a new one when the id is missing.

## `internal static void TImprintCancel(this CImprint imprint) => imprint.LImprintCancel();`

Relays the imprint's cancel of its open edit.

## `internal static void TImprintSave(this CImprint imprint) => imprint.LImprintSave();`

Relays the imprint's save of its open edit.

## `internal static LVistaRow TVistaRowCreate(long id, string epithet, bool chosen)`

Builds one Latin entry row with the given id, epithet and chosen mark.

## `internal static string TFavoriteFileRead(this CFavorite favorite) =>`

Relays a panel's internal offered file name, as the siblings below do for their panels.

## `internal static long TPanelChosenRead(this CPanel panel) => panel.LPanelChosenRead();`

Relays the record a panel stands on, which only the navigation reads in production.

## `internal static void TPanelStationAttach(this CPanel panel, Action record) => panel.LPanelStationAttach(record);`

Hands a panel the station record its tab's area would attach, so a test can see when it runs.

## `internal static void TPanelScribeRestore(this CPanel panel, bool editing) => panel.LPanelScribeRestore(editing);`

Relays the startup editor restore, which only the navigation runs in production.

## `internal static void TGuildScribeRestore(this CGuild guild, bool editing) => guild.LGuildScribeRestore(editing);`

Relays the guild's startup editor restore, as the navigation runs it.

## `internal static bool TRepertoireLeaveConfirm(this CRepertoire repertoire) =>`

Relays the repertoire's leave question as the navigation asks it when the tab is left.

## `internal static bool TShelfLeaveConfirm(this CShelf shelf) => shelf.LShelfLeaveConfirm();`

Relays the sources tab's leave question.

## `internal static bool TShelfChangeRead(this CShelf shelf) => shelf.LShelfChangeRead();`

Relays whether the sources tab holds an unsaved change.

## `internal static bool TShelfDraftFinish(this CShelf shelf, bool store) => shelf.LShelfDraftFinish(store);`

Relays the sources tab's finish of its draft, stored or dropped as `store` says.
It answers whether the finish went through.

## `internal static CExample? TCorpusTranscriptRead(this CCorpus corpus) => corpus.LCorpusTranscriptRead();`

Relays the corpus's read of the Example its transcript desk holds, which its two transcript events hand on.

## `internal static void TCorpusEntryResonate(this CCorpus corpus) => corpus.LCorpusEntryResonate();`

Relays the corpus's answer to the chosen entry's notice, which its quotation observers run.

## `internal static bool TCorpusLeaveConfirm(this CCorpus corpus) => corpus.LCorpusLeaveConfirm(true);`

Relays the corpus's leave question, which the navigation's tab and the excerpt's word click ask.
