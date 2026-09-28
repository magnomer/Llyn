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

## `internal static LVistaRow TVistaRowCreate(long id, string? epithet, bool chosen)`

Builds one Latin entry row with the given id, epithet and chosen mark.

## `internal static CCard TCardCreate(LEngine engine, CDesk desk)`

Builds the card gates over `desk` and real outlets on `engine`, as the editor does.

## `internal static CSounding TSoundingCreate(`

Builds the editor's sound sheet over a real desk and ports a test may fake.
The fake ports let a test refuse an engine call and watch the envoy.

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
