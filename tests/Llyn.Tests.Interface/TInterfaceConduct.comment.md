# TInterfaceConduct.cs
Hash: `b874782b38994b42`

## `internal static class TInterfaceConduct`

The relays that build Conduct's atelier and reach its workspace and voyage.
It also keeps the media and settings fakes the other conduct relay classes hand on.
Each relay reaches a conduct member or builds one over outlets, so none builds a WPF object.
Each relay is transparent and carries no test logic of its own.

## `internal static CAtelier TAtelierCreate(LEngine engine)`

Builds the atelier over real outlets on `engine`, with a stub player that accepts a stop but never plays.
The stub resolves every media location to no address and no screen, so a scenario's media rows still read.
A workspace engine is needed, since disposing sweeps drafts through the real draft outlet.
Disposing also runs each area's registered close, and the corpus's close stops its display's playback.

## `internal static CAtelier TAtelierCreate(LEngine engine, LMediaPort media)`

Builds Conduct's atelier over outlets on `engine` as Host does, with `media` standing in for the player.
The draft port is a fake that sweeps nothing, since the fake rig holds no drafts.

## `internal static CAtelier TAtelierFaultCreate(LEngine engine, string member, bool thrown)`

Builds the atelier over every real port on `engine`, each wrapped by `TEngineFault` on `member`.
Only the port declaring `member` faults it, so the fault sweep reaches the real engine everywhere else.
`thrown` picks a throw over a faulted task, as the sweep runs both.

## `internal static CEntryBundle TEntryBundleCreate(LEngine engine)`

The bundle Host builds, every port the facade on `engine` that owns it.

## `internal static CEntryBundle TEntryBundleCreate(LEngine engine, object swap)`

The same bundle, with `swap` in place of every port it implements.
A fake made for one port thus stands in for that port alone.

## `internal static CEntryBundle TEntryBundleCreate(LEngine engine, string member, bool thrown)`

Every facade port on `engine`, each wrapped by `TEngineFault` on `member`.
Only the port declaring `member` faults it, so a fault key names the narrow port.

## `internal static CEntryBundle TEntryBundleCreate(Dictionary<string, Func<object?[]?, object?>> answers)`

Every port a fake answering from `answers`, so an empty map gives a bundle of bare stubs.

## `internal static CPhonologyBundle TPhonologyBundleCreate(LEngine engine)`

The phonology bundle Host builds, every port the facade on `engine` that owns it.

## `internal static CPhonologyBundle TPhonologyBundleCreate(LEngine engine, string member, bool thrown)`

Every sound facade port on `engine`, each wrapped by `TEngineFault` on `member`.
Only the port declaring `member` faults it, so a fault key names the narrow port.

## `internal static CPhonologyBundle TPhonologyBundleCreate(Dictionary<string, Func<object?[]?, object?>> answers)`

Every sound port a fake answering from `answers`, so one map answers each port a fact reaches.
The scale, the cell kind and the paradigm status answer by the engine's own rule unless `answers` names them.
Those were rules with no engine state, so a fake keeps them true without a test naming them.
The inflection view answers null unless `answers` names it, so a fact sees the list alone as before.

## `internal static LSettingsPort TSettingsOutletCreate(LEngine engine)`

The settings port over the real `engine`, so a test drives the engine through the port the atelier holds.

## `internal static CAtelier TAtelierMediaCreate(LEngine engine, LMediaPort media)`

Builds Conduct's atelier over every outlet on a real workspace engine, with `media` standing in for the player.

## `internal static CAtelier TAtelierCreate(LEngine engine, LSettingsPort settings)`

Builds an atelier over a fake `settings` port, for the ledger's reads and saves.
Its draft port answers the sweep and the observer attach and detach.
The ledger can thus attach through a fake draft port.

## `internal static CAtelier TAtelierCreate(LEngine engine, LSettingsPort settings, LDraftPort drafts)`

Builds an atelier over a given settings port and draft port.
A fact can then hold the bulletin observers the draft port is handed and send a bulletin itself.

## `internal static CAtelier TAtelierCreate(LEngine engine, Dictionary<string, Func<object?[]?, object?>> answers)`

Builds an atelier whose draft, bundled entry and bundled sound ports answer from `answers`, for the text gates.
The settings and portrait ports are outlets on `engine`, and the media port answers from `answers` too.
It adds the leftover sweep and the recording stop, so disposing the atelier needs no answer from the test.
It also relays the side save to the real `engine`, so a stored side survives a fake vista port.

## `internal static LMediaPort TMediaCreate(LEngine engine)`

Builds the real media outlet over the given engine, in place of the fake port.

## `internal static LMediaPort TMediaCreate()`

A media port that only resolves locations and screens, each to nothing.
The card and atlas map relays hand it on, so a fact without an engine still reads its rows.

## `internal static LSettingsPort TSettingsCreate()`

A settings port that answers only failure notices, text keys and fonts.
It also takes the fold event's subscribe and unsubscribe, since the editor attaches to that event.
A notice reads as the unexpected key it is handed, and a text key as the key itself.
A test thus sees which wording a gate chose.
A font reads as one with nothing set, as for a language whose pack sets none.
The font rule catches no refusal, so without that answer a font read would throw.
A gate over fakes can then show its failure without a real engine behind the notice.

## `internal static CVoyageState TVoyageRead(this CVoyage voyage)`

Relays the voyage's state, and the three relays below relay its record and its two steps.

## `internal static void TVoyageStationAdd(this CVoyage voyage, string tab, long id)`

Relays the record of a station, so a test can lay down the trail it walks.

## `internal static bool TVoyageUndo(this CVoyage voyage, string tab, long id, Func<string, long, bool> show)`

Relays the step back, which hands `show` the station it lands on.

## `internal static bool TVoyageRedo(this CVoyage voyage, string tab, long id, Func<string, long, bool> show)`

Relays the step forward, which hands `show` the station it lands on.

## `internal static bool TAtelierSplitRead(CAtelier atelier)`

Relays the stored split the navigation restores the open tab's editor from.

## `internal static CEstablishment TAtelierEstablishmentRead(this CAtelier atelier)`

Relays the status read of the atelier's workspace, which the workspace raises on every open and bulletin.

## `internal static void TWorkspaceDraftAdd(this CWorkspace workspace, Func<bool> pending, Func<bool, bool> closure)`

Hands the workspace a draft's pending question and its closing step.
A test can then see how the workspace asks and closes drafts.

## `internal static void TWorkspaceObserverAttach(this CWorkspace workspace, Action<Action> marshal)`

Attaches the workspace observer with the given `marshal`, as the atelier open does.

## `internal static void TWorkspaceStateAdd(this CWorkspace workspace, Action<CWorkspaceState> heard)`

Subscribes `heard` to the workspace's internal state event, so a test hears what the wings restore from.

## `internal static void TLedgerFailureShow(this CAtelier atelier, CEnvoy envoy, string key, Exception exception)`

Relays a gate's failure policy over the atelier's own settings port, so a test can show a chosen key twice.

## `internal static void TLedgerRepaintShow(this CAtelier atelier, CEnvoy envoy, string key, Exception exception)`

Relays a repaint read's failure through the atelier's repaint memory, so a test can watch repeats merge.

## `internal static CLedgerNotice TLedgerNoticeRead(this CAtelier atelier, Exception exception)`

Relays the ledger's notice read over the atelier's own settings port, so a test can read a failure's notice.

## `internal static List<CLedgerState> TLedgerShowRead(CAtelier atelier)`

Attaches a view that keeps every state it is shown, opens the atelier through `TAtelierStubOpen`, and answers that list.

## `internal static LSettingsPort TLedgerPortCreate(Dictionary<string, Func<object?[]?, object?>> answers)`

A fake settings port that answers the ledger's reads for a Korean localization, over any answers the test sets.
It also answers an empty workspace status and an empty establishment read, so an open shows no failure.
It answers custom analysis on, so the Inflection page summary reads without a test answer.
The ledger facts and the ledger notice facts both build their atelier on it.

## `internal static CWorkspaceState? TAtelierStateOpen(this CAtelier atelier)`

Opens the atelier through `TAtelierStubOpen` and returns the workspace state it announced, or nothing when none came.
The subscription is removed again before it returns.

## `internal static void TAtelierStubOpen(this CAtelier atelier)`

Opens the atelier with a stub envoy and a marshal that runs at once.
So a test opens it without a window.
The stub answers every call with nothing.

## `internal static void TAtelierOpen(this CAtelier atelier, CEnvoy envoy)`

Opens the atelier with the given `envoy` and a marshal that runs at once.
So a test reads the notices the open shows, a layout failure among them.
A test that opens twice hands a second envoy, and sees which one later notices reach.
