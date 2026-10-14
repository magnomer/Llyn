# Llyn.Tests.Interface

The relay layer, the one place the behaviour tests reach production code.
A test outside this project may not call or construct production code itself.
It stands apart from the behaviour projects because it holds only fakes, fixtures and transparent relays.
A relay reaches one production member or builds one object over fakes.
It carries no test logic of its own.

## `Llyn.Tests.Interface.csproj`

The relays drive only portable projects, so they target the portable framework.
The project carries `TInterfaceBoundary`, which guards the relay rule for every behaviour project.
A Windows API reached from here fails the build, as it does in every portable project.
The engine seeds the controlled vocabularies from the language packs when it binds to a workspace.
The packs copy beside these binaries, and each referencing test project receives them with the reference.
The relays are internal, so the Engine, Conduct and Windows test projects each hold an access grant.
The Windows tests reach the relays through that grant instead of keeping a second relay layer.
Fakes and fixtures build Core records directly, and `TWorkspace` opens a real workspace over the Infrastructure stores.
Conduct is the deepest portable layer the tests drive, through its gates.

## `TEditorFixture`

The relay a test holds instead of the editor composer itself.
It hands each test the one facet the test reads, plus the few editor actions tests drive.
The fixture adds no state of its own, so two fixtures over one editor read alike.
The create method builds the editor the way production does, over the atelier's ports and wiring.
The wrapped editor stays available, since a vista restore, a draft read and a finish reach the editor itself.
The transcription, image and video facets are built fresh on each read, since they hold no state.
The owned flag tells whether the restored vista is the input one, which decides where a stored fresh entry goes.
Closing drops the draft and any running errand.
Undo reopens the stored entry and drops every unsaved change.

## `TInterfaceBoundary`

Guards the one rule the suite is built on.
A test outside `Interface` may not call or construct production code itself.
It scans every test source outside an `Interface` folder in the behaviour projects.
The failure names the offending files so the missing relay is obvious.
Two patterns match a call on a production name, static or on an instance, and a construction with `new`.
The relay project itself is left out of the scan, since relaying production code is its whole job.
The scan walks up from the test binaries to the tests folder, since it reads sources rather than types.
A folder counts as the root only when it holds every behaviour project.

## `TInterfaceCitation`

The relays that build a desk, an imprint and a card over engine ports whose Source or Author call fails.
It also reads the corpus list's internal draft map.
Each relay is transparent.
The fail desks have a real tenure but a title resolve that throws.
The corpus tests build an Example desk with the scoped overload.
The imprint relay builds a Source imprint over the atelier's ports and repaint memory, as the shelf does.
So the byline and imprint tests never call the internal constructor.
The fail imprint holds a fresh repaint memory of its own, and its byline find throws.
The byline tests prove a failed find reaches the notice with it.
The anthology draft read reads the tally from the anthology's own panel, so the relay hands none.

## `TInterfaceMarkdown`

The relays for the one Conduct map from a text to ready Markdown blocks.
Each relay is transparent.
The fake Markdown port runs the real engine parser, so a map sees real blocks.

## TInterfaceSettings

The relays for the settings file and the posture the shell keeps beside it.
The settings record is built here with each switch named, custom analysis on unless a test turns it off.
The layout and catalog filter records are built here too.
The settings loader is relayed here, so the file contract can be checked without an engine.
The posture file adapter is relayed the same way, so the posture text contract can be checked without a keep.
The adapter itself is built here over a keep file, and its read and save are relayed.
The trail adapter is built and relayed here too, since it stands beside the posture in the rig.
The print setting file is built and relayed the same way, since it keeps its JSON beside the posture.
The print setting clerk is built over a fake rig with a given store.
So its restore policy is checked alone.
The opening setting, a typed page range and the fake queue's printers are relayed for those checks.
The posture itself is started over an engine and pushed a field at a time, as the atelier pushes it.
The engine's left and right entry saves are relayed here too.
Each relay is transparent.

## `TPostureVolumeSave`

Sets the volume and then saves it, the two steps the atelier takes.
The set relay changes the volume without a save.

## `TPressPrinterRead`

Answers the fake queue's two printers, `Office Laser` as the default and `Plotter`.

## `TWorkspace`

A throwaway workspace folder with its own database, so every test runs against a database nothing else has touched.
Disposing removes the folder.
The connection pool is cleared first.
A pooled connection would otherwise keep the file open and the delete would fail on Windows.
Disposing a workspace clears every pooled SQLite connection in the process.
So test parallelization is disabled for the assembly, and two workspaces must not be alive in parallel.
The create method makes an empty folder whose database is not initialized yet.
The prepare method makes a folder whose database is initialized and ready to use.
The clock, press, printer queue, print setting store and phonograph are the fakes every rig of this workspace carries.
A print test sets the queue failure to see how a broken spooler reaches the user.
A print test hands two workspaces one print setting store, so a second window reads what the first kept.
Set it before the engine starts, since the rig takes it then.
The clock set makes the clock read time from the given function.
This holds for every engine and clerk built over this workspace.

## `TWorkspaceMarkupSave`

Writes markup text into the workspace folder and returns the path it stands at.
The engine imports from a path, so a test with a document in hand needs a file to hand over.

## `TWorkspaceEngineStart`

Binds an engine to this workspace folder, so a test never constructs one itself.
The plain form fetches through a stub that answers 404 to every address, so no test reaches the network.
A saved entry starts a background frequency fetch, and a real client would hit the language pack's web sources.
The client form fetches through the given client, so a discovery test runs against a stub handler.
The rig is built through the real factory, so the suite starts its engines the way the bootstrap does.
A workspace change builds its rig the same way, and no test ever writes the user's workspace pointer.
The clock is swapped for the workspace's `TClockFake`, so a test can set time through `TWorkspaceClockSet`.
The rig carries the workspace's `TPress`, `TPressQueue`, `TPressVault` and `TPhonographFake`, and a `TWarrantFake`.
So a test reads what reached them.
The source form fetches over a client that answers 404, with the given factory as its source factory.
A workspace change keeps the same factory.
A throwing factory lets a test fail a search before its harvest or lookup starts.
The private build is the one every public form reaches, so the rig shape lives in one place.
Null sources keep the factory the rig built.
An engine over fakes instead starts from `TRigFake`.

## `TWorkspaceRigCreate`

Builds a rig over the workspace for a clerk test.
Its web is a client that answers 404, or the given client.
The workspace's clock, press, printer queue, setting store and phonograph stand in place, with a `TWarrantFake`.

## `TWorkspaceConnectionRead`

Opens a raw connection to the workspace database, bypassing the store layer.
The count, column and row reads run one statement against it.
The column read is for a test that must see a column the store layer hides.
The row read writes a null as a value of its own.
So a null field never reads as an empty string.
Its fields are joined by a separator no stored text carries.

## `TWorkspaceScriptRun`

Runs statements that set the database up in a shape the store layer would not produce.

## `TWorkspace.Dispose`

Clears every pooled connection, then deletes the workspace folder.
A delete that fails on an unreleased file is ignored.

## Inline notes

### TWorkspace.cs: end of file

A file the operating system has not released yet is not a test failure.

## Stamp

Commit: `c6e60141a172`
- `Llyn.Tests.Interface.csproj` `5a64ed92eefef899`
- `TEditorFixture.cs` `a42623dacfe4b6f9`
- `TInterfaceBoundary.cs` `c098561a2150175f`
- `TInterfaceCitation.cs` `e5b81a7ec75e2151`
- `TInterfaceMarkdown.cs` `e3d48d9e8ac783fa`
- `TInterfaceSettings.cs` `53890c16052cbbfe`
- `TWorkspace.cs` `2cc686a2ca627691`
