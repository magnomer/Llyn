# Llyn.Tests

Test layout and policy for the behaviour-focused test suite.

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
Name a test method after the observable behaviour, such as `ADraftWithNoHeadwordIsRefusedBeforeAnythingIsWritten`.

## Layout

- `Interface/` — the relay layer and the boundary guard.
- `Database/` — schema migration and session behaviour.
- `Entry/` — saving, loading, updating, and deleting an entry.
- `Draft/` — held drafts and the tentative links waiting on them.
- `Lexicon/` — the records an entry owns or refers to.
- `Markup/` — the markup reader and its import into a workspace.

Convention tests live in `tests/Llyn.Convention.Tests` and audit source names directly.
They are the one suite exempt from the interface boundary.
