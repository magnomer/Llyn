# Engine

This folder holds the relays for the engine's operations.
They cover entries, content, references, fanqie, language fills, markup, portrait, settings and workspace.

## TInterfaceContent

The relays for the engine operations over what an entry owns.
That is meanings, examples, situations, tags, translations, and the rest.
It also relays the language pack reads an entry draws with, its schemes, glyph and font.
A relay without its own section passes one engine or clerk call through, at most picking one row.

## `TEntryPronunciationRead`

The pronunciation rows of a stored entry as the entry form loads them, in their stored order.
A row's id is the stored pronunciation's id, and its audio path is resolved against the workspace.

## `TEntryInflectionSave`

Rewrites the inflections of a stored entry through the entry update.
The update also drops the entry's stored lacunae.

## `TParadigmMorphologyRead`

The morphology behind the paradigm slot named by `name` that the entry's form shows.
A test takes a morphology id from here, as the paradigm grid does.

## `TEntryAnchorApply`

Anchors reflex rows of a stored entry to exactly the fanqie rows named, as the anchor menu does.
Each row is toggled one fanqie row at a time in a draft, and the draft is committed once.
A position limits the change to that row, and without one every row gets the same anchors.

## TInterfaceEngine

The relays for the engine operations over an entry as a whole.
That is saving, loading, updating, and deleting one, and the drafts and revisions around it.
The court relays and the chronicle undo and redo with their two checks are relayed here too.
The author draft start is relayed here beside the entry draft start.
The draft save writes a held draft's content through the claim clerk, as an editor's edit does.
Grasp, favorite, card fold, collocation layout, reflex opening, box opening and mention lookup relays also live here.
The leftover read and sweep, the request apply and the engine start over a hand-built rig sit here as well.
Most relays are transparent and carry no test logic of their own.
A few rebuild a read or a write the engine no longer offers from the clerks it keeps.
Court, chronicle, fold, grasp and favorite relays go through a facade, never a clerk.
The fold relays key a fold by card kind and card id.
So the read gives pairs, and the save and delete take the kind.
The draft scan and save reach the held staff's claim clerk rather than inventing another draft inventory.

## `TEngineEntryFind`

Tag and register lookup use the entry facade.
Text queries instead reach the held staff's query clerk.

## `TEngineMentionFind`

Stored example lookup uses the mention facade.
Explicit text and mention lists instead use the held staff's mention clerk.

## `TEngineDraftCommit`

This relay exposes only the committed entry, not the full outcome.
`TEngineOutcomeCommit` retains the full draft-commit outcome.

## `TEngineDraftCheck`

Saveability comes from the draft facade.
The overload with an out parameter also returns the refusal wording.

## `TEngineStampRead`

Stamp reads retain the entry facade's presence verdict and two strings.
The relay passes the engine settings' interface culture, as a driver does.
The format relay words a time in the invariant culture.

## `TEngineLineRead`

Relays the example line read.
It answers the frame, the sentence pieces and the Source line.

## `TEngineEntryCommit`

Writes a whole draft the way the editor does.
The draft is started, its content normalized and saved, and then committed.
A new entry and an existing entry follow the same path, the latter with its id.
The commit starts the fetches a real save starts.
A test that must not fetch turns the setting off first.

## `TEngineCollocationRead`

The card facade places the supplied draft's collocations, with the targets as the link targets the row flags come from.
A fake card port answers through it, so a display read with failing ports still places collocations.

## `TEngineBoxCheck`

Box kind and entry id jointly identify the opening read.
The box write is identified the same way.

## `TEngineLeftoverRead`

The drafts a leftover sweep leaves for the user.
Each is saveable, held by no one here, and claimed by no other process.

## `TEngineCreate`

Starts an engine over the rig that answers the same rig for any workspace and keeps no workspace pointer.
A fault fact thus runs the engine over a rig whose vaults it has replaced.

## `TInterfaceEngineFanqie`

The relays for the engine's fanqie spellings and their diwei.
That is the fanqie read, set, check, start and divide, the diwei find and the tally read.
Each relay is transparent.

## `TInterfaceEngineLanguage`

The relays for the engine's language fills over an entry.
That is the ensign load, the frequency, the inflection, the script, the reflex and the stem.
The paradigm scan and the language the paradigm resolves for an entry are relayed too.
Each fill is relayed with its read, its pending check, its start and its rebuild where it has them.
The sound start relays the reading view's start, which starts every fill of the entry at once.
The frequency also relays its gauge, the answer the entry view's chip and tooltip show.
Every relay but the ensign load is transparent.
The guise relay takes the reflex rows, so a test can check each row's shown text.
The kindred relay takes an optional vista, so a test can pin the order the panel sets.

## `TEngineEnsignLoad`

The engine hands the rows to a callback, so the relay keeps them and answers them once the load ends.
The overload for one language and its varieties keeps them the same way.
The overload with a store hands the test's own store to the engine's fill and answers the languages it returns.
It is the call the settings outlet forwards to, so a test reads both the stored rows and the answer.

## `TEngineInflectionRead`

The pending and enabled flags stand for the fetch check and the morphology setting the shell passes in.
The held flag defaults off, as the reading view and the note ask.
A test sets them directly, so it reaches every cell tip the view shows without a live fetch.

## `TEngineStemSpread`

Relays a series member's fold toggle, the call the xiesheng gate makes.

## `TInterfaceEngineMarkup`

The relays for the engine's markup exchange.
That is the markup read, the target find, the import and the export.
Every relay but the import is transparent.

## `TEngineMarkupImport`

The engine reports only what an import left behind, so the stored entries are found again here.
A merge or a replace names its target.
A new entry is the match its headword did not have before the import.

## `TInterfaceEnginePortrait`

The relays for the engine's likeness of an entry or a page, and for its Joplin reads.
Each relay is transparent.
So a fact reaches the internal facades without the shell's ports.

## `TEnginePressOpen`

Relays the print session to `LEnginePortrait.LEnginePressOpen`, which loads the sheet or page into the rig's press.
The caller disposes the answered relay.
The engine never answers null here.
The relay widens the type, so a fact holds it like the relay a vista print answers.

## `TLiveryRead`

Relays `LEngineLivery.LEngineLiveryRead`, the Joplin page read of one entry.
It passes pending false, as the Joplin push does.
The language overload relays the reconstruction read, with a localizer that answers each key unchanged.
So a fact can assert on keys instead of shipped text.

## `TInterfaceEngineSettings`

The relays for the engine's settings.
That is the settings and text reads, the localization, and each switch save with its check.
The custom analysis switch is among them, so a fact can turn the custom sheets off.
The notebook relay answers the title the engine stored, so a fact can check the trim and the fallback.
Each relay is transparent.

## `TInterfaceEngineWorkspace`

The relays for the engine's workspace, its rig and its state.
That is the workspace start, open, change, read and format, the folder open and the location reads.
The rescue read, the status bar's establishment read and the revision and state reads are relayed here too.
The leased engine construction is relayed too, so a fact can hold or refuse a workspace lease.
The observer attach and detach and the recording sweep are relayed here as well.
The rig apply, the usher swap and the three record builders sit here too.
Each relay is transparent.

## `TEngineWorkspaceOpen`

Builds a rig for the path through the real factory, then applies it as the bootstrap does.
Its sources, press, printer queue, warrant and phonograph are all fakes.
The clock is set to a `TClockFake`, which reads the real time until a test sets it.

## `TBulletinCreate`

Builds a bulletin for a subject and id, so a conduct test can send one to the observers it holds.
The bulletin carries no refusal.

## TInterfaceReference

The relays for the engine operations over references and their authors.
That is reference and author reads, writes and deletes, the authors an owner cites, and the oeuvre search.
The fellow find, the union read and the author absorb are relayed too.
Each relay is transparent, except that the author create and update also raise the author bulletin.

## Stamp

Commit: `c37ca1837ded`
- `TInterfaceContent.cs` `2f1032ea1bb4d887`
- `TInterfaceEngine.cs` `918679693bd37518`
- `TInterfaceEngineFanqie.cs` `3e7c158c1c15322e`
- `TInterfaceEngineLanguage.cs` `60527ff66ae06733`
- `TInterfaceEngineMarkup.cs` `26a61d688b58bff1`
- `TInterfaceEnginePortrait.cs` `0b846a638698dde8`
- `TInterfaceEngineSettings.cs` `86ffacd21d12d364`
- `TInterfaceEngineWorkspace.cs` `d2bd34564bfd2ac8`
- `TInterfaceReference.cs` `33defb3a50ab83aa`
