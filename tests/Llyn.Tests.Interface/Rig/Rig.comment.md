# Rig

This folder holds the fake rig.
It also holds the fake ports a test engine, clerk or gate is built over.
It also holds the relays for the clerks of the application ring.

## `TClockFake`

The clock every test engine is built over, running on the system clock until a test sets it.
Setting it is the one way to freeze or advance time inside an engine.
Setting a reader lets a test advance a captured variable between requests.
The pause is a real wait through `Task.Delay`, so test timing stays as it was.
Setting the clock never shortens that wait.

## `TEngineFake`

Builds a port from a table of answers, so a deportment test runs without an engine or a database.
A member the table names is answered by its function over the call's arguments.
Every other member throws, so a deportment reaching the engine where the test expected none fails loudly.
One generated proxy per port spares the suite a hand-written class for every port member.
The stub variant answers nothing, for ports a deportment holds but the test never drives.

## `TEngineProxy`

The proxy the runtime generates the port over, public and constructible as `DispatchProxy` requires.

## `TEngineFault`

Builds a port or vault that forwards every call to a real instance except one member, which faults.
`TEngineFake` cannot serve here, since it throws on every member its table does not name.
The fault sweep thus runs a gate over the real engine and faults only the member the gate reaches.
The member is named `Interface.Member`, so a member of the same name on another port keeps forwarding.
With `thrown` the member throws, otherwise it answers a faulted task.
A member that answers no task always throws, since it has no task to fault.

## `TEngineFaultProxy`

The proxy the runtime generates the interface over, public and constructible as `DispatchProxy` requires.
Static interface members never reach it, so they keep their own bodies and are not faulted.
A forwarded call does not wrap the real member's exception, so a real fault reaches the gate unchanged.

## `TEnvoyFake`

Builds fake envoys over `TEngineFake`, so a gate's questions are answered and recorded without a driver.
Each fake answers only the questions its facts expect, and any other question throws.
Every fake records each failure key it is shown, so a fact reads the questions and the failure in order.
The plain create records every key it is asked in `asked`.
A confirm answers `answer`, or no when it is null.
The leave question records `Leave` and answers as given.
The union question records its key and both names joined by `>`.
The file variant answers `path` and `medium` and records `File:` plus the offered name.
The press variant hands the print window question to `answer` and leaves that question unrecorded.
Its file question answers `path` as a PDF, `press.pdf` when none is given, and records `File:` plus the offered name.
A null press path stands for a reader who declines the Save as PDF file dialog.
The coinage variant records `Coinage:` plus the message key and answers `wording`.
The tally variant records a confirm's key, tally key and tally joined by `>`, and answers `answer`.
  A plain confirm records its key alone, so a fact reads whether the count was shown.
The markup variant answers its file question unrecorded, so a fact counts the notices alone.
The media variant records `Media:` plus the title key and answers `path`.
The folder variant records `Folder:` plus the starting folder and answers `folder`.
It records a failure with a notice as the key and the notice's reason joined by `>`.
So a fact reads which refusal the gate showed.

## TInterfaceClerk

The relays for the clerks of the application ring, built over a rig of fakes.
Each relay is transparent and carries no test logic of its own.
Most relays pass one clerk call through, so only the builders and the odd ones are described here.

## `TRigClerkCreate`

A fake rig over `entries` as both entry ports, with in-memory workspace, revision, tombstone, pronunciation, transcription, reflex and Ancestor stores.
That is enough for an entry clerk save to run without SQLite.

## `TRigClaimCreate`

The clerk rig with the draft, claim and court ports seated by in-memory fakes, posing as process one.
The process setter makes the same rig pose as another process, its claim fake naming that process from now on.

## `TCourtClerkCreate`

The court clerk takes its own issuer and chronicle over the same rig.
The claim clerk shares its issuer and chronicle with the court clerk it holds.
The court's translation clerk stamps through a revision clerk over the same rig.

## `TRequestStray`

A request of a kind no switch knows, so a test can prove the default arm throws.
The production code never declares such a kind.

## `TEntryClerkCreate`

An entry clerk over the rig with every clerk it composes, the transcription, reflex and recording clerks included.
Its translation and entry clerks each stamp through their own revision clerk over the same rig.
The reflex and frequency clerks get a throwaway gate and a bulletin that goes nowhere.
The paradigm, transcription, reflex, recording and frequency clerks share one language cache.

## `TRecordingClerkCreate`

A recording clerk over the rig with a fresh language cache, trail clerk and claim clerk.
The play relay answers the play's ticket and plays at full volume by default.
A second play relay takes a list of files and a volume, and plays them in turn.

## `TMarkupIntakeCreate`

The markup intake with the clerk graph an import needs, built over the rig.
The revision and query clerks it adds read the same rig, so the import stamps where the test reads.
Its draft clerk resolves Ancestor links through the same link clerk as the intake.

## `TPortraitClerkCreate`

The portrait clerk with every clerk a page composes from, built over the rig.
Its fold clerk reads the entry's stored folds for the page.

## `TMarkupExportCreate`

A markup entry clerk over the rig, with a reflex clerk of its own.
So an export reads the pack's reflex order.
Its card clerk holds the example clerk.
An Ancestor markup clerk over a link clerk of its own carries the etymology map.
The markup clerk exports through the entry clerk this builds.

## `TRecordingFormat`

A recording path made workspace-relative through the trail of the rig.
A path the trail cannot make relative comes back unchanged.

## `TSettingsRead`

The default settings, in English, that every clerk built here reads.

## `TEntryClerkAdd`

Creates a bare entry with the headword and language of the given entry, as a translation link does.

## `TPhonographFake`

The phonograph every test engine is built over, which plays nothing.
A test never opens a platform player, so no run makes a sound.
The played list holds every file the phonograph was asked to play, in order.
A test reads it to prove a call did or did not reach the player.
The stopped count says how often it was asked to stop.
The volume holds the last level it was set to, or -1 before any.
It raises `LPhonographFinished` only when a test asks, so the clerk's next file starts on the test's cue.

## `TPostureFake`

The posture port kept in a dictionary, so a fake-rig engine starts a posture without a file.
A save keeps the state under the name, replacing the last one.
A read answers nothing before a save.

## `TRigFake`

Builds a rig from fakes, so an engine test runs without a workspace folder or a database.
The ports the engine touches at start are answered by small in-memory fakes.
Every other port is a stub that throws on any call.
A test reaching storage it did not seat therefore fails loudly.

## `TRigFakeStart`

Binds an engine to the rig, whose workspace change builds a fresh fake rig.
The engine's pointer callback does nothing, so no pointer is recorded.

## `TRigFakeBuild`

The plain forms build the rig over a `TVaultFake` entry port, on the bare `fake` root.
The workspace overload stands on the root named by the label.
The root is a bare label, since no fake here touches disk.
Two rigs built with two labels let a test prove a rig apply moved the engine.
The language port is a `TRigFakeLanguages` and the usher is a throwing stub.
The overload over language packs and an usher lets a flag fill run through the real engine.
The case decides every fetch and file.
The private build is the one all public forms reach, so each fake stays in one place.
The entries fill both entry slots, so a stored row is found by the same fake that stored it.
The trail is the real system adapter, since path rules touch no disk, and the clock is a `TClockFake`.
The press is a `TPress`, the printer queue a `TPressQueue` and the print setting store a `TPressVault`.
The warrant is a `TWarrantFake`, so no test touches the operating system store.
The process id is one.
The posture is a `TPostureFake` and the phonograph is a `TPhonographFake`.
The pronunciation and reflex ports are an empty `TVaultFakePronunciation` and `TVaultFakeReflex`.
The workspace open reads them for every entry to renew stale respellings, so they answer instead of throwing.
Every other port, besides the fakes below, is a throwing stub, the livery, manifest and outpost ports among them.

## `TRigSourceCreate`

A source factory that throws on every build, for a search that must fail before its sources exist.

## `TRigFaultRead`

The message a stub throws when the method is called.
A test asserting a recorded stub fault reads it here instead of copying the text.

## `TRigStubCreate`

One throwing stub for a port, generated over the proxy below.
A generated stub per port spares the suite a hand-written class for every one of the fifty stubbed ports.

## `TRigFakeProxy`

The proxy base every stub derives from.
Any call throws `NotSupportedException` with `TRigFaultRead` of the member that was asked.
It is public because the runtime derives a type from it in a dynamic assembly.

## `TRigFakeVault`

The root port, whose session commits nothing and which reports no migration.
Its session's commit and disposal do nothing.
The audit shelf records nothing and answers no path.
The doctor port's database creation always answers healthy.

## `TRigFakeSettings`

Settings kept in memory, reading an English default until the engine saves once.
Existence follows the save.

## `TRigFakeLanguages`

A language port listing no packs and validating no name.
Its flag read and its stored flag find answer none, and reading a pack throws.

## `TWarrantFake`

A warrant that marks and reverses the token, so tests need no operating system protection.
The output is visibly not the plain token and is exactly undone by restoring.
A fixed marker lets restoring tell this fake's text from anything else, as the real adapter rejects foreign data.
Hide refuses a null or blank token like the real adapter, so a test cannot hide nothing by accident.
Otherwise it returns the marker followed by the reversed token.
Restore answers null for blank text, text lacking the marker, or nothing after the marker.
The real adapter answers null for anything it cannot recover.
One reversal is shared between hiding and restoring.

## Stamp

Commit: `bd299c638b2c`
- `TClockFake.cs` `dffa2b2fcf999593`
- `TEngineFake.cs` `364e90d7a21a82f8`
- `TEngineFault.cs` `deab7d5e61cdcbfb`
- `TEnvoyFake.cs` `6d5bffee0960fa06`
- `TInterfaceClerk.cs` `08c1766833cedf3f`
- `TPhonographFake.cs` `78d1f82eea929e6a`
- `TPostureFake.cs` `85c13ab0bfe0439a`
- `TRigFake.cs` `c83bd318f58f76b1`
- `TWarrantFake.cs` `7cef68fe9f435952`
