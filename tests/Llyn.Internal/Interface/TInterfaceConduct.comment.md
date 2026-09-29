# TInterfaceConduct.cs

## `internal static class TInterfaceConduct`

The relays for the conduct rules the reading view and the dialogs read.
It is a class of its own rather than a part of `TInterface`.
Each relay reaches a static rule or builds a conduct over outlets, so none builds a WPF object.
Each relay is transparent and carries no test logic of its own.

## `internal static LFont TFontCreate(string family, double size) => new(family, size);`

Builds an engine font for a fake settings port to answer, so a test never constructs a Core record.

## `internal static CAtelier TAtelierCreate(LEngine engine) => new(`

Builds the atelier over real outlets on `engine`, with a stub player.
A workspace engine is needed, since disposing sweeps drafts through the real draft outlet.

## `internal static CAtelier TAtelierCreate(LEngine engine, LMediaPort media) => new(`

Builds Conduct's atelier over outlets on `engine` as Host does, with `media` standing in for the player.
The draft port is a fake that sweeps nothing, since the fake rig holds no drafts.

## `internal static CAtelier TAtelierCreate(LEngine engine, LSettingsPort settings) => new(`

Builds an atelier over a fake `settings` port, for the ledger's reads and saves.
Its draft port answers the sweep and the observer attach, so the ledger can attach without a real engine.

## `internal static CAtelier TAtelierCreate(LEngine engine, Dictionary<string, Func<object?[]?, object?>> answers)`

Builds an atelier whose draft, entry and phonology ports answer from `answers`, for the text gates.
The settings and portrait ports are outlets on `engine`, and the player is a stub.
It adds the leftover sweep, so disposing the atelier needs no answer from the test.

## `internal static CEnvoy TEnvoyCreate(bool? answer, List<string> asked)`

Builds an envoy that records every key it is asked in `asked`.
A confirm answers `answer`, or no when it is null.
The leave question records `Leave` and answers `answer` as given.
The union question records its key and both names joined by `>`, and answers like a confirm.

## `internal static CDesk TDeskCreate(LEngine engine, string scope, CEnvoy envoy)`

Builds a desk over a real draft outlet on `engine`, as the owners in Deportment do.

## `internal static CVistaRow TPanelRowRead(LVistaRow row)`

Relays the internal entry row map.

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

## `internal static IReadOnlyList<CCatalogSituation> TAtlasRowsRead(this CAtlas atlas, string unknown, string untitled)`

Reads the atlas rows through its internal helper, without the repertoire's stale-selection close.

## `internal static CSituationDraft? TAtlasDraftRead(LSituation? situation)`

Wraps the Situation in a bare draft and maps it through the atlas's internal helper.
A null Situation reads as no draft at all.

## `internal static CAnthology TAnthologyCreate(CAtelier atelier, CDesk desk, CEnvoy envoy)`

Builds the example list through its internal factory, always shown and always finishing.

## `internal static void TAnthologyVistaRestore(this CAnthology anthology, LVista vista)`

Hands the example list its vista through the internal helper the corpus calls.

## `internal static IReadOnlyList<CCatalogExample> TAnthologyRowsRead(this CAnthology anthology, string unknown, string unwritten)`

Reads the example rows through their internal helper, without the corpus's stale-selection close.

## `internal static CExample? TAnthologyExampleRead(LExample? example)`

Maps a stored Example through the list's internal helper.

## `internal static void TDeskOccurrenceStart(this CDesk desk, long? situation)`

Starts a fresh entry already linked to the Situation, as the repertoire's new occurrence does.

## `internal static void TDeskQuotationStart(this CDesk desk, long? example)`

Starts a fresh entry already citing the Example, as the corpus's new quotation does.

## `internal static LVistaRow TVistaRowCreate(long id, string? epithet, bool chosen)`

Builds one Latin entry row with the given id, epithet and chosen mark.

## `internal static CCard TCardCreate(LEngine engine, CDesk desk, CEnvoy envoy)`

Builds the card gates over `desk` and real outlets on `engine`, as the editor does.
`envoy` is where a failed translation is shown.

## `internal static CSounding TSoundingCreate(`

Builds the editor's sound sheet over a real desk and ports a test may fake.
The fake ports let a test refuse an engine call and watch the envoy.
The settings port is `TSettingsCreate`, so a refusal reaches the envoy with a notice.

## `internal static LSettingsPort TSettingsCreate()`

A settings port that only reads failure notices, each as the unexpected key it is handed.
A gate over fakes can then show its failure without a real engine behind the notice.

## `internal static CSentenceOrder TCatalogOrderRead(LEngine engine, string language)`

The word order a language's pack gives, mapped as the sentence frame maps it.

## `internal static CSentence TSentenceCreate(LEngine engine, CDesk desk)`

Builds the sentence gates over `desk`, a real phonology outlet and an atelier's catalog on `engine`.

## `internal static CStateValue TCardStateRead(LStateValue value)`

Relays the internal written-value map.

## `internal static CEntryDraft TCardEntryRead(LEntryDraft draft)`

Relays the entry draft map, so the facts pass engine drafts through the boundary.

## `internal static CSession TSessionCreate(CDesk desk, IReadOnlyList<Func<bool>> pending, Func<bool> readySeam, Action<long> storedSeam)`

Builds a session over `desk` alone, starting it by vista as the guild does.
The overload over an editor desk records each editor finish in `seen`.

## `internal static LExample TExampleMentionAdd(this LExample example, LMention mention) =>`

The Example with `mention` as its only Mention, so a test builds an Example that links.

## `internal static CMentionResult TMentionResultRead(int offset, LMention? stored) =>`

Maps a click result holding only `stored`, so a test builds no engine record itself.

## `internal static LPortraitMedium TPortraitMediumRead(CPortraitMedium medium) =>`

Relays the internal export format map.

## `internal static LPressTicket TPortraitTicketRead(CPressTicket ticket) =>`

Relays the internal print ticket map.

## `internal static LPortraitLabel TPortraitLabelRead(LSettingsPort settings) =>`

Relays the label wording, so a fact reads each key through a fake settings port.

## `internal static LPortraitLegend TPortraitLegendRead(LSettingsPort settings, string realm) =>`

Relays the legend wording of `realm`.

## `internal static IReadOnlyList<CPortraitChoice> TPortraitChoiceRead() =>`

Relays the export formats the file question offers.

## `internal static Task TPortraitFileExport(CEnvoy envoy, string file, Func<string, LPortraitMedium, Task> export) =>`

Relays the shared export core with its question and failure policy.

## `internal static Task TPortraitTicketPrint(CEnvoy envoy, Func<LPressTicket, Task> print) =>`

Relays the shared print core with its question and failure policy.

## `internal static string TFavoriteFileRead(this CFavorite favorite) =>`

Relays a panel's internal offered file name, as the siblings below do for their panels.

## `internal static CEnvoy TEnvoyFileCreate(string? path, CPortraitMedium medium, List<string> asked) =>`

A fake envoy whose file question answers `path` and `medium` and records `File:` plus the offered name.
It records each failure key it is shown, so a fact reads the question and the failure in order.

## `internal static CEnvoy TEnvoyTicketCreate(Func<CPressTicket?> answer, List<string> asked) =>`

A fake envoy whose printer question records `Ticket` and answers from `answer`, which may throw as a failing dialog does.
It records each failure key it is shown.

## `internal static long TPanelChosenRead(this CPanel panel) => panel.LPanelChosenRead();`

Relays the record a panel stands on, which only the navigation reads in production.

## `internal static void TPanelStationAttach(this CPanel panel, Action record) => panel.LPanelStationAttach(record);`

Hands a panel the station record its tab's area would attach, so a test can see when it runs.

## `internal static void TPanelScribeRestore(this CPanel panel, bool editing) => panel.LPanelScribeRestore(editing);`

Relays the startup editor restore, which only the navigation runs in production.

## `internal static bool TAtelierSplitRead(CAtelier atelier) => atelier.LAtelierSplitRead();`

Relays the stored split the navigation restores the open tab's editor from.

## `internal static void TGuildScribeRestore(this CGuild guild, bool editing) => guild.LGuildScribeRestore(editing);`

Relays the guild's startup editor restore, as the navigation runs it.

## `internal static bool TRepertoireLeaveConfirm(this CRepertoire repertoire) =>`

Relays the repertoire's leave question as the navigation asks it when the tab is left.

## `internal static bool TShelfLeaveConfirm(this CShelf shelf) => shelf.LShelfLeaveConfirm();`

Relays the sources tab's leave question.

## `internal static CVoyageState TVoyageRead(this CVoyage voyage) => voyage.LVoyageRead();`

Relays the voyage's state, and the three relays below relay its record and its two steps.

## `internal static void TNavigationTabAdd(`

Registers a tab on a navigation as a panel area does, with its hooks handed in.
The landed record is handed to `arrival`, as the area's own open would take it.

## `internal static void TNavigationStationAdd(this CNavigation navigation) => navigation.LNavigationStationAdd();`

Relays the station record the areas' row gates make.

## `internal static void TNavigationDiweiAttach(this CNavigation navigation, Action<string, string, string> open) =>`

Attaches a rime-cell open to a navigation as the yunjing area does.

## `internal static void TNavigationStemAttach(this CNavigation navigation, Action<string, string?> open) =>`

Attaches a series open to a navigation as the xiesheng area does.

## `internal static bool TNavigationStemOpen(this CNavigation navigation, string language, string? key) =>`

Relays the series jump, which only the display asks in production.

## `internal static void TCorpusExampleOpen(this CCorpus corpus, long id) => corpus.LCorpusExampleOpen(id);`

Relays a panel's arrival open, as the navigation runs it.
The relays below do the same for the other areas.
