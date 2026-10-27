# Outpost

The Outpost folder pushes entries one way into Joplin.
The courier clerk gathers what a push needs and hands it to the `LOutpost` entrance as one `LOutpostCargo`.
The livery clerk builds the pages those notes render from.
Llyn's copy always wins over edits made in Joplin.
Llyn never acts on a Joplin item it did not create.

## `LCourierClerk`

The clerk holds no Joplin protocol.
It reads settings, entries, languages and the realm, then lets the outpost entrance do the sending and the pairing.
So the push order, the manifest and the trash sweep live behind `LOutpost`.
This clerk stays testable with a fake outpost.
The engine's gate guards vault calls only, so a slow Joplin never blocks the rest of the engine.
Only one push or attach runs at a time in the whole process, even across a workspace switch.
Two would race over one manifest.
Settings are read at each push, so a new port or token applies without a restart.
The fault sink goes to the outpost inside the cargo, so every caught failure leaves a trace beyond the receipt.

### `_lCourierClerkBusy`

One while a push or attach runs anywhere in the process, set by compare-and-swap so two callers never both start.
It is static so a second clerk built after a workspace switch meets the same flag.

### `LCourierClerkSend`

Refuses with `LRefusalCourier` while another push or attach still runs.
An empty stored token refuses with `LRefusalMissing`, since Llyn was never connected.
A token that is present but cannot be restored refuses with `LRefusalWarrant`.
Both refuse before anything is read, so the user connects before anything is sent.
The realm and the entry list are read together under the engine's gate, so they describe one workspace.
The language list is read after the gate is released, since it asks no engine state.
The page and language readers pass to the cargo unchanged, and the realm travels as its `N` text.
The busy flag clears in a `finally`, so a failed push never blocks the next one.

### `LCourierClerkAttach`

Asks the outpost for a token and answers it hidden.
The hidden form is the one the engine stores in settings.
It shares the push's busy flag, so a token never changes under a running push.
The stored port is read from the settings at each attach.

### `LCourierClerkCheck`

Whether a token is stored, so the shell shows Llyn as connected.
It only reads the settings and never asks Joplin, so a status bar may ask on every bulletin.
A stored token Joplin no longer honours still reads true until a push meets the refusal.

### `LCourierClerkMatch`

Whether the exception is the refusal of a token Joplin rejected or that cannot be restored.
The engine drops the stored token on it, so the shell offers Connect again.
Only the exception itself is read, since the clerk throws every refusal bare.

## `LLiveryClerk`

Holds the builders `LLiveryFacade` calls to turn engine answers into an `LLiveryPage` or an `LLiveryLanguage`.

### `LLiveryClerkBuild`

The page overload is called once per stored entry by `LLiveryFacade.LEngineLiveryRead`.
It hands each answer unchanged to the `LLiveryPage` constructor, except the flag, pending and enabled arguments.
It builds `LLiveryPageShown` with `LParadigmClerk.LParadigmClerkCheck`, one per paradigm row in row order.
Each check passes pending and enabled and keeps held off, since a page never holds a fill.
That is the check the view's `LEngineParadigmCheck` makes, so both show the same row text.
It collects every language the page names, from the draft, its reflexes, targets, usages and glosses.
The lineage and the relation list pass through unchanged, and no lineage language is collected.
The fold set holds card kind and id pairs, and also passes through unchanged.
The target map is keyed by card kind and id too.
Glosses are read from every card and every nested child card.
It asks the flag function once per distinct language and keeps each path it answers as `LLiveryPageBanner`.
A language the function answers null for gets no entry.

The language overload is called once per language, holding the engine gate.
A null stem clerk means the language has no series rule, so the series list stays empty.
Otherwise it lists every series in name order with an empty query.
Each series carries its `LStemPageRead` page and the entries `LStemEntryScan` finds for it alone.
The page is read with folds, so each member carries its stored Series fold as `LStemMemberOpened`.
The sheet writes that state as an open or closed readings block.
The raw respelling setting is passed, so each member reads its guises as the entry page does.
Those entries are in headword order, since the export has no panel setting to follow.
A null diwei clerk means the language has no fanqie book, so the category list stays empty.
Otherwise it lists the initials, then the rimes, then the tones, each kind in name order.
Each category carries its kind, its `LDiweiPageRead` page and the entries `LDiweiEntryScan` finds for it alone.
Those entries are in headword order too.
The switched, tallied and localize arguments pass unchanged to `LDiweiPageRead`, as `LEngineDiweiResolve` passes them.

## Stamp

Commit: `1f3467bbe5c6`
- `LCourierClerk.cs` `12e80a3284ed799d`
- `LLiveryClerk.cs` `62fa4b1cd055e014`
