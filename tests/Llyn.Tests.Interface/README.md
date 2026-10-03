# Llyn.Tests.Interface

Test layout and policy for the behaviour-focused test suite.

## Platforms

The suite is split into four projects that share the namespace `Llyn.Tests`.
`Llyn.Tests.Interface` targets `net10.0` and holds the relay layer, the fakes, `TWorkspace` and the boundary guard.
`Llyn.Tests.Engine` targets `net10.0` and tests Core, Application, Infrastructure and ShellEngine.
`Llyn.Tests.Conduct` targets `net10.0` and tests the Conduct gates both drivers call.
`Llyn.Tests.Windows` targets Windows and holds every test that compiles only there, Capsule and Deportment among them.
The three behaviour projects reference only this project, so the relay layer is not duplicated.
No portable project references Capsule, which needs Windows.

## Interface boundary

Every production operation a test uses is relayed through `TInterface` or an adapter under `/Interface`.
This covers pure value operations, object construction, and store and engine calls alike.
A test body never calls a method or a constructor from `src` on its own.

An adapter may translate, invoke, observe, and clean up.
It must not repair the behaviour under test.
Each relay delegates to one production operation and returns what that operation returned.
It never reimplements production logic and never reports a success the production path did not produce.

`TInterfaceBoundary` enforces the rule for every test written from here on.
`TWorkspace` owns the throwaway workspace folder, its database, and the engine bound to it.

## Naming

Name a test file and its class after the behaviour it covers, not after the production type it drives.
Name a test method `MethodUnderTest_Scenario_ExpectedResult`, such as `EntrySave_NoHeadword_RefusesAndWritesNothing`.
The first part names the operation the test drives, the second the condition it sets up, and the third the observable outcome.

`tests/RulesTestName.md` is the authoritative rule set, and it covers the length budget and the wording every part is held to.

## Layout

- `Llyn.Tests.Interface` — the relays, fakes, fixtures and workspace at its root, and the boundary guard.
- `Llyn.Tests.Engine/Configuration/` — the view state and the settings a workspace carries between runs.
- `Llyn.Tests.Engine/Database/` — schema migration and session behaviour.
- `Llyn.Tests.Engine/Entry/` — saving, loading, updating, and deleting an entry.
- `Llyn.Tests.Engine/Draft/` — held drafts and the tentative links waiting on them.
- `Llyn.Tests.Engine/Lexicon/` — the records an entry owns or refers to.
- `Llyn.Tests.Engine/Engine/` — the answers the engine composes above the stores, such as an entry's expected forms.
- `Llyn.Tests.Engine/Catalog/` — the orderings and matches a browsed catalog is listed under.
- `Llyn.Tests.Engine/Pronunciation/` — the lookup seams below the engine: sources, readings, the language pack, and the trove.
- `Llyn.Tests.Engine/Markup/` — the markup reader and its import into a workspace.
- `Llyn.Tests.Engine/Portrait/` — portraits and their citation pages.
- `Llyn.Tests.Engine/Vault/` — the vault behind entries, drafts, identity, languages and settings.
- `Llyn.Tests.Conduct/Conduct/` — the gates both drivers call.
- `Llyn.Tests.Windows/Interface/` — the Capsule relay, kept out of the portable relay layer.
- `Llyn.Tests.Windows/Capsule/` — Capsule's file contract.
- `Llyn.Tests.Windows/Deportment/` — the GUI driver over its Windows controls.

Convention tests live in `tests/Llyn.Tests.Convention` and audit source names directly.
They are the one suite exempt from the interface boundary.
