# TInterfaceConduct.cs
Hash: `c59ff8e7a63ff17f`

## `internal static class TInterfaceConduct`

The relays that build Conduct's atelier and reach its workspace and voyage.
It also keeps the media and settings fakes the other conduct relay classes hand on.
Each relay reaches a conduct member or builds one over outlets, so none builds a WPF object.
Each relay is transparent and carries no test logic of its own.

## `internal static CAtelier TAtelierCreate(LEngine engine) => new(`

Builds the atelier over real outlets on `engine`, with a stub player that accepts a stop but never plays.
The stub resolves every media location to no address and no screen, so a scenario's media rows still read.
A workspace engine is needed, since disposing sweeps drafts through the real draft outlet.
Disposing also runs each area's registered close, and the corpus's close stops its display's playback.

## `internal static CAtelier TAtelierCreate(LEngine engine, LMediaPort media) => new(`

Builds Conduct's atelier over outlets on `engine` as Host does, with `media` standing in for the player.
The draft port is a fake that sweeps nothing, since the fake rig holds no drafts.

## `internal static CAtelier TAtelierMediaCreate(LEngine engine, LMediaPort media) => new(`

Builds Conduct's atelier over every outlet on a real workspace engine, with `media` standing in for the player.

## `internal static CAtelier TAtelierCreate(LEngine engine, LSettingsPort settings) => new(`

Builds an atelier over a fake `settings` port, for the ledger's reads and saves.
Its draft port answers the sweep and the observer attach and detach.
The ledger can thus attach without a real engine.

## `internal static CAtelier TAtelierCreate(LEngine engine, Dictionary<string, Func<object?[]?, object?>> answers)`

Builds an atelier whose draft, entry and phonology ports answer from `answers`, for the text gates.
The settings and portrait ports are outlets on `engine`, and the media port answers from `answers` too.
It adds the leftover sweep and the recording stop, so disposing the atelier needs no answer from the test.

## `internal static LMediaPort TMediaCreate(LEngine engine) => new LMediaOutlet(engine);`

Builds the real media outlet over the given engine, in place of the fake port.

## `internal static LMediaPort TMediaCreate() =>`

A media port that only resolves locations and screens, each to nothing.
The card and atlas map relays hand it on, so a fact without an engine still reads its rows.

## `internal static LSettingsPort TSettingsCreate() =>`

A settings port that answers only failure notices and text keys.
A notice reads as the unexpected key it is handed, and a text key as the key itself.
A test thus sees which wording a gate chose.
A gate over fakes can then show its failure without a real engine behind the notice.

## `internal static CVoyageState TVoyageRead(this CVoyage voyage) => voyage.LVoyageRead();`

Relays the voyage's state, and the three relays below relay its record and its two steps.

## `internal static bool TAtelierSplitRead(CAtelier atelier) => atelier.LAtelierSplitRead();`

Relays the stored split the navigation restores the open tab's editor from.

## `internal static CEstablishment TAtelierEstablishmentRead(this CAtelier atelier) =>`

Relays the establishment the atelier holds, which the navigation reads on a tab change.

## `internal static void TWorkspaceDraftAdd(this CWorkspace workspace, Func<bool> pending, Func<bool, bool> closure) =>`

Hands the workspace a draft's pending question and its closing step.
A test can then see how the workspace asks and closes drafts.

## `internal static void TWorkspaceStateAdd(this CWorkspace workspace, Action<CWorkspaceState> heard) =>`

Subscribes `heard` to the workspace's internal state event, so a test hears what the wings restore from.

## `internal static CWorkspaceState? TAtelierStateOpen(this CAtelier atelier)`

Opens the atelier and returns the workspace state it announced, or nothing when none came.
The subscription is removed again before it returns.
