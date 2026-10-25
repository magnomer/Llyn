# Outpost

The Outpost folder pushes entries one way into Joplin.
The courier clerk runs the push and delegates notes, reconstruction notes and pairing to three smaller classes.
The livery clerk builds the pages those notes render from.
Llyn's copy always wins over edits made in Joplin.
Llyn never acts on a Joplin item it did not create.

## `LCourierClerk`

The engine's gate guards vault calls only, so a slow Joplin never blocks the rest of the engine.
Only one push or attach runs at a time in the whole process, even across a workspace switch.
Two would race over one manifest.
Overwriting a note Llyn created is accepted policy, since the push is one way.
Attachments a user pastes into a Llyn note lose their reference when the note is rewritten.
Joplin's own cleanup may later delete such attachments.
Settings are read at each push, so a new port or token applies without a restart.
The fault sink receives every caught failure before it is turned into a refusal or skipped.
So a failure leaves a trace beyond the receipt.

### `LCourierSystem`

The title of the notebook under the top one that holds the style note.
So the CSS never sits among the entries of any language.
`LCourierStyle` titles the note whose CSS every entry note imports.

### `LCourierStall`

How many notes in a row may fail on an unreachable Joplin before the push stops.
Three rules out one unlucky timeout, yet spares the user waiting through every remaining note.
It is internal so `LCourierLanguage` and `LCourierWarrant` stop on the same count.

### `_lCourierClerkBusy`

One while a push or attach runs anywhere in the process, set by compare-and-swap so two callers never both start.

### `LCourierClerkSend`

The page reader answers null when the entry is gone, which fails that entry.
It runs one push, refusing with `LRefusalCourier` while another push or connect still runs.
The busy flag clears in a `finally`, so a failed push never blocks the next one.

### `LCourierClerkAttach`

Asks Joplin for a token through `LCourierWarrant.LCourierWarrantAttach` and answers it hidden.
The hidden form is the one the engine stores in settings.
It shares the push's busy flag, so a token never changes under a running push.
The stored port is read from the settings at each attach.

### `LCourierClerkCheck`

Whether a token is stored, so the shell shows Llyn as connected.
It only reads the settings and never asks Joplin, so a status bar may ask on every bulletin.
A stored token Joplin no longer honours still reads true until a push meets the refusal.

### `LCourierLinkBuild`

The link map the livery uses to turn rime-card chips into links to reconstruction notes.
It maps a language, a kind and a key to `LLivery.LLiveryIdFormat` over the kind, realm stamp, language and key.
It holds every series and category with a non-empty key, and anything else maps to empty.
`LCourierLanguage.LCourierLanguageSend` takes its note ids from this map alone, so the two never drift.
`LCourierBatchSend` filters it afterwards by the notes Joplin answered, so a chip never links to a missing note.

### `LCourierBatchSend`

Pushes the notebooks, the style note, the reconstruction notes and every entry, then trashes gone notes.

Notebooks:
  The top notebook holds every other one and takes its title from `LSettingsNotebook`, read at each push.
  Its id never follows that title, so a changed title renames the same notebook and moves nothing.
  Each language gets a notebook under it, titled with the language's name.
  Each notebook id comes from `LLivery.LLiveryIdFormat`, a language's over its trimmed name.
  So a notebook keeps its id across pushes, and a renamed one gets its title back.
  A language with no entry gets no notebook, so packs never leave empty notebooks.
  An entry without a language stays in the top notebook itself.
  A note left in the top notebook by an older push moves when it is next sent.

Refusals:
  An empty stored token refuses with `LRefusalMissing`, since Llyn was never connected.
  A token that is present but cannot be restored refuses with `LRefusalWarrant`.
  Both refuse at once, so the user connects before anything is sent.
  A Joplin that answers on no port refuses too, since every call would fail the same way.
  A refusal or a cancellation stops the push, because every later note would meet the same cause.
  A failure while saving the notebook or the style note stops the whole push.
  A `TimeoutException` there goes to the fault sink, then refuses with `LRefusalOutpost`.
  So every cause of an unreachable Joplin reads the same to the user.

Reconstruction notes:
  Every language holding entries is read before the first note is sent.
  Each language is read inside its own guard, so one failed read costs only that language.
  Its failure goes to the fault sink, and it gets no reconstruction notes and no chip links.
  Its entries still go, as plain text where a chip would link.
  Any failed read skips the trash sweep for this push, as a foreign manifest does.
  So a transient database fault never wipes that language's notes from Joplin.
  The manifest keeps their digests, so the next whole push sweeps what is truly gone.
  Each language's notes are sent before the entries, with ids from the link map.
  Each note Joplin answers, sent or kept, joins a set.
  The entries' link map holds only ids in that set.
  So a failed reconstruction note leaves its chips plain rather than linking to nothing.
  A series or category gone from the read is trashed.
  A failed reconstruction note joins the failed list by title, and its timeouts share the entries' row.
  A reconstruction note counts as saved or kept exactly as an entry does.

Entries:
  Each entry's note id comes from one note map that `LCourierNote.LCourierNoteBuild` builds.
  The same map goes to the livery, so a body can name another entry's note.
  A failure inside one entry costs just that entry.
  That entry's fault goes to the fault sink, and its headword joins the receipt's failed list.
  Three entries in a row failing with `TimeoutException` refuse with `LRefusalOutpost`, as Joplin stopped answering.
  Any other outcome breaks the row, so scattered timeouts never stop a healthy push.

Manifest and trash:
  A manifest whose realm is set and differs from this workspace's realm is treated as empty.
  Nothing from it is trashed, and it is replaced on save.
  A rebuilt database or a foreign file must never drive trashing.
  A manifest with an empty realm was written before realms existed, so it is adopted and stamped.
  Every manifest id outside the current set is trashed afterwards, unless a language read failed.
  The current set holds the style note, every entry note and every reconstruction note tried.
  A note that failed this time stays in the set, so it is never trashed for one failure.
  Each trash passes every Llyn notebook id and the livery's mark, so only proven Llyn notes go.
  Those ids cover every language pack too, so a note whose language lost all its entries still goes.
  Each scanned language adds its notebook ids from `LCourierLanguageFolder` as well.
  So a reconstruction note is proven Llyn's even when its notebook was never saved this time.
  A note in the notebook of a language neither scanned nor still used is left alone.
  A trash that answers leaves the manifest whether or not it trashed.
  It counts as removed only when the outpost answers true.
  A failed trash goes to the fault sink and keeps its id, so the next push tries again.
  Three trashes in a row failing with `TimeoutException` refuse with `LRefusalOutpost`.
  Any other outcome breaks that row, as in the entry loop.
  The manifest is saved even on a stop, so the next push skips what already arrived.
  The style note counts in no tally, since it is Llyn's own plumbing rather than content the user wrote.

## `LCourierLanguage`

Sends one language's reconstruction notes for `LCourierBatchSend`.
It stands apart so the courier clerk stays within the file size limit.
It holds no state between pushes, so the clerk builds one per push.
Its failures leave the same trace as an entry's.

### `LCourierLanguageFolder`

The id seeds of the series, rime-table, initial and rime notebooks, in that order.
`LCourierLanguageSend` saves its notebooks from them, and `LCourierBatchSend` hands them to the trash sweep.
One list keeps the notebooks Llyn saves and the notebooks it proves as its own from drifting apart.

### `LCourierLanguageSend`

Writes one note per series, initial, rime and tone into the language's notebooks.
Series notes go under `Navigation.Xiesheng`, tone notes under `Navigation.Yunjing`.
Initial and rime notes go under `Yunjing.Shengmu` and `Yunjing.Yunmu` inside the rime-table notebook.
Every notebook title comes through the lookup.
A tone note is titled through `Display.FanqieTone` over its key.
Series, initial and rime notes are titled with their page key, as the panels name them.
Every note title is looked up and formatted inside that note's own failure guard.
So a malformed localized string fails only that note.
Each notebook id comes from `LLivery.LLiveryIdFormat` over its folder seed and the language.
A notebook is saved only when it will hold a note, so a language without books leaves none behind.
The rime-table notebook is saved when it or one of its two inner notebooks holds a note.
A `TimeoutException` while saving a notebook goes to the fault sink, then refuses with `LRefusalOutpost`.
Any other failure while saving a notebook stops the whole push, as it does for the clerk's notebooks.
Every note id comes from the link map and nowhere else, so a chip and its note always agree.
The clerk passes the unfiltered id map here, and filters the entries' map by what answered.
A note whose id the map answers empty is not sent, which drops a blank key.
Each id joins the current set before sending, so a failed note is never trashed.
Every note goes through the clerk's own note send, so keeping and digests work as for entries.
That send also counts the note and records its id once Joplin answers.
A failed note goes to the fault sink, and its title joins the failed list.
A note whose title itself failed joins the failed list under the language name instead.
The stalled count carries the row of timeouts in and out, so reconstruction notes and entries share one row.
Three in a row refuse with `LRefusalOutpost`, and any other outcome breaks the row.
It answers the row as it stands after the last note.

## `LCourierNote`

Renders and sends one note for `LCourierBatchSend`, keeping it when Joplin already holds it.
It stands apart so the push's order and tallies stay in the clerk, and one note's sending lives here.
It holds no state between pushes, since the manifest digests travel in with each call.

### `LCourierTagPrefix`

The prefix on every tag Llyn attaches, the language tag included.
Llyn's tags can then never coincide with the user's own tags.

### `LCourierNoteBuild`

The note map handed to every entry send and to the livery.
An entry id in the list maps to `LLivery.LLiveryIdFormat` over the realm stamp and the id.
Any other id maps to empty, so the livery keeps that reference as plain text.
So a link never points at a note this push does not write.

### `LCourierEntrySend`

Reads the entry's page, then renders and sends its note.
The page reader takes the engine's gate itself, so this clerk takes no lock here.
The page is read without fetching, so a push over every entry starts no network fetch.
An entry gone since the listing throws, so it counts as failed rather than silently kept.
The tags come from the page's draft.
The link map goes to the livery unchanged, so the rime card's chips point at this push's reconstruction notes.

### `LCourierNoteSend`

Sends one note unless it is kept, answering whether it was sent.
A note is kept only when the manifest holds its digest and Joplin returns exactly its body.
Joplin returns that body only while the note keeps its title.
That check repairs a note edited, renamed, moved or trashed in Joplin, or missing from a fresh profile.
A kept note still gets its tags saved, so tags edited inside Joplin are repaired.
It stays counted as kept.
The digest and every note id come from the livery, which owns the hashing.
Parcels go first, so the note never links to a resource Joplin lacks.
The digest is recorded only after the tags land, so a failed note is retried next time.

### `LCourierTagRead`

Every card tag of the entry, through meanings, collocations and their children, plus its language.
Each one carries the prefix, so it is plainly Llyn's.
The set is sorted and blank-free, so the same entry always yields the same tags.
The tag save makes a Llyn note's tag set exact.
So old unprefixed tags on Llyn notes are detached on the next push.
The user's tags themselves stay untouched.

## `LCourierWarrant`

Pairs Llyn with Joplin by asking for a token and polling until the user decides.
It stands apart from `LCourierClerk` because pairing shares no push state but the busy flag.
The clerk keeps that flag, so a pairing never runs beside a push.
A pairing failure leaves the same trace as a push failure.

### `LCourierWarrantPatience`

How many times a connect polls for the user's decision, one interval apart.
Two minutes leaves time to find Joplin's prompt without leaving a forgotten request polling forever.
`LCourierWarrantInterval` is the wait before each poll, about a second.

### `LCourierWarrantAttach`

Asks Joplin for a token and answers it hidden, the form the engine stores in settings.
The stored port from the settings is tried before Joplin's own range.
A Joplin that answers on no port refuses with `LRefusalOutpost`, since no request can reach it.
Any other failure to start the request goes to the fault sink, then refuses with `LRefusalOutpost`.
Each poll waits first, because the user needs time to see Joplin's prompt.
A poll that throws `TimeoutException` goes to the fault sink and counts as still waiting.
`LCourierClerk.LCourierStall` such polls in a row refuse with `LRefusalOutpost`, since Joplin stopped answering.
Any poll that answers breaks the row.
A rejection refuses with `LRefusalWarrant`, and the user may simply try again.
Running out of polls refuses with `LRefusalPending`, so the request does not hang the shell.

### `LCourierWarrantCheck`

Whether the exception is the refusal of a token Joplin rejected or that cannot be restored.
The engine drops the stored token on it, so the shell offers Connect again.
Only the exception itself is read, since every courier refusal is thrown bare.

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

Commit: `8c403d1df768`
- `LCourierClerk.cs` `eef468b0b538d2a2`
- `LCourierLanguage.cs` `30d6bd5fcaad5e62`
- `LCourierNote.cs` `d978ffb8d947f5b3`
- `LCourierWarrant.cs` `f0cbd592b106bcaa`
- `LLiveryClerk.cs` `aae0e7be00d8b022`
