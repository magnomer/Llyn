# Vista

The engine's facades that browse and edit stored records, plus the vista registry that feeds the catalog panels.
Each facade takes the engine gate and hands the work to the clerk that holds the rules.
Each facade implements its port itself, so Host hands it to Conduct with no outlet between.
Each constructor stores the hearth and the sibling facades it calls, all built by `LEngine` before it.
Siblings are taken rather than the engine, so a facade names only the facades it uses.
The gate, the staff and the shared state are read through the hearth.

The vista overloads stay in these facades, because a vista is the shell's and twin names are numbered per panel.
No browse panel keeps its own order and filter or applies them itself.
Draft start and commit for Authors, Examples, Sources and Situations live here too, through the citation clerk.
Start mints a draft id and writes the first file.
It refuses a record that is gone before any file is written.
Commit turns a held draft into a stored record, announces it and returns it.
Commit refuses an id from a closed workspace before anything is written.
Every change is announced from the facade, outside the gate.

## `LVistaFacade`

Where a panel asks for a vista, it gets the engine's view state for one catalog tab.
The same goes for the rows it lists.
The engine holds the choices and returns rows already filtered, sorted, numbered and marked.
The facade keeps the vista registry and delegates stored data to the other facades and staff.
Favorites live in `LCatalogFacade`, beside Tags and Registers.

The tab map holds the vista now standing for each tab, so a restart can detach the one it replaces.
The count hands out ids, so each vista's bulletin names one vista.

`LEngineVistaStart` starts a vista on the order, filter and mode the caller hands it.
The shell's posture resolves those from what it stored, so the engine holds no view state of its own.
The subject names the stored kind the tab lists, so the engine holds no table of panel names.
A blank vista lists nothing for an empty query, which only the duplex wings ask for.
The vista is attached as an observer before it is returned, so it carries bulletins to the panel.
The vista started earlier for the same tab is detached first, so a switched workspace leaves no ghost.

`LEngineVistaRead` answers the vista a bulletin names, or nothing once a restart replaced it.
The posture asks after each vista bulletin, so it stores the order and filter the vista now holds.

`LEngineEntryFind` over one vista lists the entry rows, ready to show.
Query, order and filter are the vista's.
A blank vista with nothing typed answers no rows before the lock is taken.
The rows are built by the shared builder under one lock.

`LEngineEntryFind` over a parent and child lists the entries the parent's chosen record reaches.
The parent's language filter applies, since a catalog's filter hides languages from its chosen record's entries.
The child's query narrows the rows, and the child's chosen row is marked.
The query clerk's match orders by the child's ordering, so equal headwords go by the entry tie rule.
Store return order never decides.
The entry facade probes the parent's chosen record by subject and id.
A missing vista, or a parent subject that reaches no entries, answers no rows.

`LEngineNameResolve` makes labels distinct in their given order, numbered where two share a name, for the compass rows.

`LEngineVistaLoad` loads the vista's selected record according to its subject, under the gate.
The result is a snapshot, with no claim file, editing identity or registered tenure.
Reference snapshots include credits.
Tag and register loads carry no record, only proof that the chosen row still exists.
A stored choice that no longer loads is dropped here, so a panel never branches on the missing answer.
The overload with an id selects the row first and restores the previous choice when the load throws.
So a failed click leaves the panel where it stood.

`LEngineFileRead` gives the file name an export of the vista's entry is offered under.
It is the headword with barred characters replaced.
The settings facade's `LEngineFileFormat` owns the barred characters and the `entry` fallback.
A missing vista hands it a blank headword.
A failed load is not caught here and travels up to the export gate.

`LEngineSideSave` writes the chosen entry to the left or right slot of the workspace state.
The next run reopens each duplex side on the entry it last showed.
The vista says which side it is, so no caller copies that.

`LEngineUsageRead` counts the places the chosen stored record reaches, which a delete would drop.
An Example, a Situation and a Source count the entries that cite them.
An Author counts the Sources crediting it.
Any other subject, or no stored choice, reaches nothing.

`LEngineTallyRead` gives the line a vista shows beneath its list, ready to show.
An entry vista counts the entries it lists, worded by `LEntryTallyFormat`.
An Example, a Situation and a Source give the citation line of the chosen stored record.
Any other vista refuses, and no stored record reads as cited nowhere.

`LEngineVistaDelete` deletes the selected record of its subject and clears the selection afterwards.
Only an entry delete records a revision, so every other subject answers null after deleting.
A catalog record is detached from every owner first, as the panel already confirmed the usage.
Tag, register and structural vistas delete nothing and answer null.

## `LVistaRowFacade`

Builds the rows every catalog of entries shows, so no facade numbers twins or reads epithets itself.
It holds only the hearth, so the facades that build rows need not reach the vista facade.

`LEngineVistaBuild` turns entries in their listed order into rows ready to show, with twin name, epithet and chosen mark.
An entry with no epithet carries an empty one, so no reader of a row falls back on its own.
Twins are numbered by entry id, so the older entry is `(1)` in every view.
Only entries of one language are twins.
The epithets come from one scan, so a long list costs one statement rather than one session per row.
The chosen row is the one whose id equals `chosen`.
It takes the lock for the epithet scan, which reads the hearth's settings and staff.

## `LEntryFacade`

Entry lifecycle goes to `LEntryClerk`, searches to `LEntryQueryClerk` and the grasp to `LGraspClerk`.
The glyph resolve and the grasp, establishment and usage reads sit here, each one call on an entry.
It implements both the entry port and the grasp port.

`LEngineEntryRead` answers null when no entry has the id.

`LEngineStampRead` answers whether the entry is stored, with creation and update times already worded, since the draft carries no clock.
It words them in the culture its caller passes.
`CDisplay` and `LLiveryFacade` pass `LEngineCultureRead`, the interface language.
An id no entry has answers false with empty times, so the reading view hides its stamp row.

`LEngineStampFormat` turns a stored ISO 8601 UTC stamp into local time in the short general format of the current culture.
A missing or unreadable stamp answers empty.

`LEngineEntryFind` by subject lists the entries the catalog record reaches.
The record is probed by id alone, so only the id of each blank probe matters to the query clerk.
A subject that reaches no entries answers no rows.
The catalog vista's child list asks here, so the probe shapes stay beside the entry rows.

`LEngineEntryLoad` gives the entry as a draft, or null when it no longer stands.
The entry clerk resolves its recordings to absolute paths.

`LEngineEntryDelete` runs the clerk's delete under the gate, then raises the entry bulletin outside it.
It reads the orphaned card count around the delete, and a change raises the deck bulletin with id 0 too.
It returns the recorded revision.

`LEngineReflexRead` lists the reflex rows that carry written text, in draft order.
The filter is `LReflexClerk.LReflexWrittenScan`, which the series page's members share.

`LEngineGlyphResolve` gives the entry a character stands for in a language, made when none exists yet.
Only an exact headword in that language counts, so a Mandarin entry of the same character is never taken.
The candidates come from the query clerk's search on the headword.
Creation goes through `LEngineTranslationCreate`, so the revision is recorded and the frequency fetch starts.

`LEngineGraspStep` is the last grasp step, handed out so the shells draw the stars without naming the Core constant.
`LEngineGraspRead` reads the half-step grasp stored on the entry.
A missing entry reads zero, the same as an entry never rated.
`LEngineGraspSave` writes the grasp under the gate, then raises a Grasp bulletin for the entry.

`LEngineEstablishmentRead` counts the held drafts that differ from their origin, the stored entries and the database bytes.
Only drafts this engine holds are counted.
Each held draft is measured as the leave dialog measures it, so the bar and the dialog cannot disagree.
The entry count comes from the query clerk and the bytes from the workspace clerk.

`LEngineUsageRead` hands the usage clerk's counts and itemized rows under the gate, and `LEngineTallyRead` its citation lines.
Itemized rows carry the epithet the settings ask for.

`LEngineCardFind` says which card list of a draft holds a card, and at which place, or null when neither does.
The card clerk scans the draft.

## `LCardFacade`

Card storage access uses the hearth's shared gate, keeping the shell behind the engine's clerks.
Only Entry owners may read Meanings through the owner overload.
Meanings belong only to Entries, so another owner is rejected before storage is read.
`LEngineOwnerRaise` gives unsupported owners one exception shape.
It returns the exception so switch arms can throw it directly.

`LEngineMeaningRead` by entry leaves reading order and fallback naming to `LMeaningClerkSort`.
The caller supplies the fallback localization key, and the settings facade supplies its wording.

`LEngineSenseRead` answers null when no linked Mention lies under the selection, so no sense menu opens.

`LEngineMeaningScan` reads the Meanings of the held draft, not of the store, so unsaved Meanings are offered.
`LMeaningClerkScan` owns reading order, fallback naming and paths, with the wording of the key.
A tenure holding no draft answers an empty list.
The path of each row is its outline number under its parents, such as `2.1`.

`LEngineCollocationRead` forwards to `LDraftClerkSense.LCollocationLayoutRead`, which applies the link rules to the draft.
It reads only the draft and the handed targets, so it takes no lock and reaches no store.

`LEngineTranslationFind` offers translation targets for a typed word.
A card cannot translate its own stored Entry, so that row is excluded from the offer.
Exclusion follows vista construction, preserving the numbering assigned before filtering.
Workspace languages supply creation choices, with the held draft's language last.

`LEngineProspectFind` takes its mention-language context from the held draft, so callers pass no separate language.
The tenure protects the draft read before the shared storage gate is taken.

`LEngineTranslationResolve` by word resolves only on one normalized whole-headword match.
No match or ambiguity answers null, and the supplied Entry is excluded.

`LEngineTranslationCreate` makes a translation stub.
A committed stub receives frequency work and an entry bulletin.
The bulletin leaves the shared gate before notifying readers.

`LEngineTargetRead` by ids reads stored targets without any draft's court links.
The owner overload supplements them with the owner's court links, so tentative targets can be named.

`LEngineTranslationRead` keys every top-level meaning and collocation, even without translation targets.
Distinct target ids share one stored read, omitted when no ids exist.
Storage failures propagate to the caller's failure boundary.

The static `LEngineTranslationResolve` over cards keeps each card's link order and omits targets absent from the read.
The supplied cards receive dictionary keys even when no links resolve.

Missing stored targets are omitted, preventing blank chips.

`LEngineIncomingRead` gives both incoming card kinds the hearth's current epithet setting.

`LEngineFoldRead` covers folded cards of both kinds for one entry under the shared gate.
Folding and unfolding bypass the held draft, so they cannot dirty lexical data or enter undo.
Both raise an entry-scoped fold bulletin after the gate is released, so other views of the entry re-read their folds.
Even an ignored nonpositive card id raises that bulletin.

## `LCatalogFacade`

The facade for the shelves a card or an entry is filed under, meaning Tags, Registers and favorites.
It names no sibling facade, since every shelf it serves sits in the catalog staff.
It implements the tag, register and favorite ports.

`LEngineTagRead` reads every Tag once, in alphabetical order.

`LEngineTagFind` for a card offers the stored Tags its tag field offers for the text it keeps.
Those the held draft's card already shows are left out.
The draft is read before the gate is taken, as the tenure guards itself.

`LEngineTagFind` for a vista lists the taxonomy panel's tags with the vista's query and order.
The vista's filter hides languages from the entries of the chosen tag, not tags, so it is not applied.
A vista whose chosen tag no longer answers is deselected.

`LEngineTagCreate` runs the tag clerk's create, then raises the tag bulletin outside the gate.
The catalog is announced so every panel listing Tags shows the new row.

`LEngineRegisterFind` for a card offers stored Registers by the same rules as tags.
The language whose pack seeds the shelf is the held draft's own, so no caller passes one.
The vista overload mirrors the tag one: the filter is not applied, and a vanished chosen Register is deselected.

`LEngineRegisterCreate` runs the register clerk's create.
The bulletin is raised after the lock is released, so the panel lists and selects the row.

`LEngineFavoriteFind` lists marked entries matching the query, with those in a hidden language left out.
The vista overload returns vista rows, twins numbered and epithets read in one scan, so the panel only copies them.
The row of the entry the vista stands on comes back marked chosen, so the panel keeps no choice.

`LEngineFavoriteSave` marks and `LEngineFavoriteDelete` unmarks, each announcing the change.
Marking changes no lexical data and keeps the entry's identity.
Unmarking leaves the entry reachable through the entry catalog.

## `LAuthorFacade`

Provides the workspace browsed by the people its Sources credit.
Draft start and commit run through the `LAuthorCitation` the citation clerk holds.

`LEngineAuthorFind` has four forms.
By query and order it lists Authors as browsed rows carrying their Source count and citation count.
By id it answers one row or null.
By name it lists matches, leaving one Author out, capped by a limit.
By vista it reads query and order off the vista and marks the row the vista stands on as chosen.

`LEngineRollFind` lists the roll the authors panel shows, headed by the row standing for Sources crediting nobody.
Before the panel binds its vista it lists nothing.
That row is added only while the vista carries no query, and only when such Sources exist.
Its counts are the uncredited Sources and the places citing them.
Its name is the localized uncredited word, so the panel hands in no text.

`LEngineUnionFind` lists the Authors the autograph may fold its stored Author into.
Blank text or no stored Author on the roll lists nothing.
The stored Author itself is left out.

`LEngineUnionRead` gives the two names of the union question, after the held tenure's deferred requests are applied.
The author clerk trims them and reads a missing Author as empty.

`LEngineVitaRead` gives the read sheet of the Author the roll stands on, with its fellows and the places citing it.
With no stored Author chosen it is the sheet of nobody, worded as unnamed.

`LEngineOeuvreFind` lists the Sources crediting an Author, narrowed by query and by the kinds left shown.
The vista form takes query and order from the oeuvre vista.
It takes the chosen Author and kind filter from the roll vista.
Before the panel binds both vistas it lists nothing.
A null author lists every Source, and zero lists the Sources crediting nobody.

`LEngineWorkFormat` and `LEngineBylineRead` give the author clerk's wording of a work count and a byline.

`LAuthorFacadeLimit` caps the union search and the byline list, few enough to read at a glance.

`LEngineAuthorAbsorb` folds one Author into another and deletes the dropped row.
Observers hear of both ids, because a panel standing on the dropped Author must move to the kept one.

`LEngineAuthorRead` by id answers null when none has that id.
By owner it reads the Authors credited on a Reference, in that Reference's own author order.

`LEngineBylineFind` lists the byline rows a typed credit may already name.
The draft is read here by its id, so the editor never carries the credits it holds back down.
No draft offers nothing, and neither does a blank word or a draft raised in a workspace no longer open.
Nothing is caught, so the stale refusal reaches the Conduct gate that shows it.

`LEngineCreditRead` gives the credit rows of the held draft, after its deferred requests are applied.
No held draft has none.

`LEngineAuthorDelete` deletes the Author and announces it.
With detach set it drops every credit first.

## `LReferenceFacade`

The bibliographic half of the engine.
A Reference is the work an Example is drawn from.
A citation is a reference to a row, never ownership of it.
Draft start and commit run through the `LReferenceCitation` the citation clerk holds.
Start returns the held Reference with its credits.

`LEngineCitationResolve` finds or creates the Reference a typed citation names, under one hold of the gate.
The reference clerk reads the Reference cited now off the held draft, so no shell carries it in.
Zero card and sentence ids name the draft's own Example, as on the corpus panel.
The clerk decides the match, and only a created Reference is announced.

`LEngineReferenceFind` has several forms.
By query and order it lists rows already carrying name, credits and citation count.
For a card sentence it offers the stored Sources its citation field shows, ready to show.
That offer reads the Source the sentence cites off the held draft, so its own byline offers nothing.
By vista it reads query and order off the vista and marks the chosen row.
With no argument it lists every Source ordered by author.

`LEngineReferenceRead` over found rows numbers their twin names and marks the chosen one.
The sources panel and the oeuvre list share it.

`LEngineReferenceDelete` deletes the Reference and announces it, with detach clearing every citation first.

`LEngineColophonRead` gives the read sheet of a Source draft, under the gate.
Its citation line counts the Source the draft holds, which is the one its panel has chosen.

`LEngineImprintRead` gives the edit sheet of a Source draft, or of a blank Source with no draft.
It reads no store, so it takes no gate.

`LEngineCitationRead` gives the line each cited Source is shown under, ready for display.
The entry form keys the lines by id, and the draft form serves the corpus.

`LEngineKindRead` gives the kind menu of the source editor, each option's tag and localization key, in menu order.
The reference clerk owns the menu, which is the same for every Source.

## `LExampleFacade`

An Example is independent data owned by nothing, so both card sides may reference the same one.
Draft start and commit run through the `LExampleCitation` the citation clerk holds.
Start returns the held sentence.

`LEngineExampleRead` answers null when no Example has the id.

`LEngineExampleFind` by query and order lists Examples as rows already carrying their cited name and quotation count.
The vista overload lists the corpus panel's rows and marks the chosen one.
The twin names are read from the sentence.
The panel hands in the words shown for an unknown or unwritten one.
Each row's text is worded by the same rule, so the list shows it ready.

`LEngineExampleDelete` drops every reference to the Example first when the user asked for that.

`LEngineTextMatch` says whether a field showing one text already shows another, a blank field reading as nothing recorded.
The example clerk owns the rule.

`LEngineLineRead` gives what a reading card shows for one sentence row.
That is the frame, the sentence divided around its Mentions and the Source line.
The example clerk composes them, so the display holds no composing rule.

## `LSituationFacade`

A Situation is independent data owned by nothing and referenced by any number of cards.
Draft start and commit run through the `LSituationCitation` the citation clerk holds.
Start returns the held Situation.

`LEngineSituationFind` by query and order lists Situations as rows already carrying how many places reference them.
The card form offers the stored Situations its field shows for the text it keeps, ready to show.
Those the held draft's card already links are left out.
The draft is read before the gate is taken, as the tenure guards itself.
The vista form lists the repertoire panel's rows and marks the chosen one.
The twin names are read from the title.
The panel hands in the words shown for an unknown or untitled one.
The kind is worded the same way, with the unknown wording for an unknown kind.

`LEngineSituationRead` answers null when none has the id.

`LEngineSituationDelete` drops every reference to the Situation first when the user asked for that.

## `LMentionFacade`

The facade for mention operations.
Every find and resolve takes the gate and hands the work to `LMentionClerk`, which holds the rules.
The span and offset reads are pure and take no gate.
It calls no sibling facade, so `LEngine` builds it among the first.
Resolves over a held draft read the draft before the gate is taken, as the other held reads do.

`LEngineMentionFind` runs the clerk's find over a stored Example.
It can also run over one sentence row of the entry the reading view shows.

`LEngineSpanRead` gives a field's selection as the span a Mention request carries.
The engine measures it so the shell never counts.

`LEngineSpanCheck` says whether a selection spans any code point, so a link command may run.

`LEngineUnitRead` and `LEngineOffsetRead` convert between a code-point offset and a UTF-16 index.

`LEngineMentionResolve` gives the clerk's mention line for one Example of the held draft.
Card 0 and sentence 0 address the draft's own Example.
Without a card it gives the lines for every sentence row of the held draft.

`LEngineLinkRead` gives the entry a clicked link names, or null when it names none.
A link names its entry only through a stored id, so an empty id names nothing.

## Stamp

Commit: `bd299c638b2c`
- `LAuthorFacade.cs` `a7c57e4576ed1f96`
- `LCardFacade.cs` `fd835262e150b403`
- `LCatalogFacade.cs` `8545ab05f0c69039`
- `LEntryFacade.cs` `c0d6c5fdd16cd585`
- `LExampleFacade.cs` `f20eb09a45abf04e`
- `LMentionFacade.cs` `2cfb6578c504b8a7`
- `LReferenceFacade.cs` `b73762c75a1cefad`
- `LSituationFacade.cs` `534a19d43f1ba0ba`
- `LVistaFacade.cs` `aba8d22b409fa49e`
- `LVistaRowFacade.cs` `c984c1184a7c7741`
