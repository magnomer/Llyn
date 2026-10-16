# Clerk

The clerks that read, fetch, store and reconcile an entry's pronunciation data for one rig.
They cover diwei, fanqie rows, phonetic series, frequencies, recordings, script images and transcriptions.
They stand apart from the relays, which only carry lookup results to a listener.
These clerks own the stores and the pending state.

The fanqie, script and shengfu background fetches share the following pattern.
Pending fetches and remembered misses are keyed by language and character, since two entries share a character.
The engine's gate is shared, so a fetch that lands writes under the same lock as every other vault call.
Their callers start background tasks without awaiting them, so each runner records failures.
Cancellation by the fetch's own token is not a failure and is not recorded.
An answered empty fetch is remembered, but an unreachable source remains eligible for another attempt.
A failure still raises the bulletin so the panel stops waiting.

## `LDiweiClerk`

Diwei read, found and composed into a page, with the tally of anchored reflexes.
The rows themselves are rebuilt by the fanqie clerk when a fetch lands.

## `LDiweiClerk.LDiweiClerkFind`

The list is narrowed and sorted by the shared catalog rule in `LCatalogClerk`.
So the engine only marks the chosen row.

## `LDiweiClerk.LDiweiKindRead`

A fanqie row without a parsed initial or rime carries a blank key.
Pressing it opens nothing, so the kind is none.

## `LDiweiClerk.LDiweiEntryScan`

Entries come back through `LCatalogEntry.LCatalogEntrySort`, the owner every entry list shares.
The archive reads in storage order, so no caller may show its rows unsorted.

## `LDiweiClerk.LDiweiPageRead`

The respellings show only when `switched` stands.
The tally shows only when both flags stand.

## `LDiweiClerk.LTallyRead`

Reflexes anchored to the diwei's rows are grouped by heading and character.
The heading is the division, or the place of articulation for a rime.
Languages and kinds are ranked in the language's reflex order the pack declares, never by its fetch rules.
The anchor query's own order only gathers rows, so it never shows.

## `LFanqieClerk`

Fanqie rows fetched per character and book, stored, and rebuilt into diwei.

## `LFanqieClerk.LFanqieClerkRead`

Characters follow the headword.
Each character's rows follow `LFanqieRow.LFanqieRowSort` over the pack's books.
The archive returns rows in storage order, so this is the one place the flat list gains its order.
The portrait and the anchor picker read this flat list.

## `LFanqieClerk.LFanqieClerkDivide`

Rows are grouped by book, each block placed by its first row in the declared order.
The stored phonetic series of each character rides on that character's first block.

## `LFanqieClerk.LFanqieClerkSet`

The new rank is resolved by `LFanqieRow.LFanqieRankResolve`.
A plain press marks an unmarked row last and unmarks a marked one.
A raising press moves a marked row one place up, and the first row wraps to last.

## `LFanqieClerk.LFanqieClerkRebuild`

It forgets the misses of the entry's characters and fetches them again.

## `LFanqieClerk.LFanqieClerkScan`

It sends one request per book, spaced by the book's interval.

## `LFanqieClerk.LShengfuStoredScan`

This clerk only reads the series that `LShengfuClerk` stored.
The box printing the books is the box printing the series.

## `LFanqieClerk.LFanqieClerkRun`

A fetch is admitted through a single-wide gate.
Rows found are stored and the character's diwei reapplied.

## `LFrequencyClerk`

Word frequencies fetched, stored and graded for one rig.
The pending fetches and the entries a source answered nothing for are the clerk's own state.

## `LFrequencyClerk.LFrequencyClerk`

The settings are read at fetch time, so a setting turned off mid-fetch discards the answer.
A fault of the background fetch goes to the owner given at build.
The owner records it, and the clerk decides nothing more.

## `LFrequencyClerk.LFrequencyClerkResolve`

The entry's regraded rows are gathered into the one answer a frequency chip shows.

## `LFrequencyClerk.LFrequencyClerkRead`

Rows are regraded under the current pack and come back in `LFrequency.LFrequencySort` order.
So the gauge, its tooltip, Livery and the portrait agree.
A regrade that changed is written back.
An entry with no rows starts a fetch.
None starts while one is pending or after a source answered nothing.
A false `fetch` starts none, so a pass over every entry never floods the sources.

## `LFrequencyClerk.LFrequencyClerkStart`

It cancels a fetch already pending.
Nothing starts when the setting is off or the language declares no source.

## `LFrequencyClerk.LFrequencyClerkClear`

The per-entry overload also clears the entry's stored rows.

## `LFrequencyClerk.LFrequencyClerkScan`

It asks every source for the word and keeps the first phonetic figure each one gives.
The flag `reached` says whether any source answered at all.

## `LFrequencyClerk.LBandResolve`

Thresholds are tried first for numeric figures, then declared patterns if no threshold yields a band.

## `LFrequencyClerk.LBandMatch`

A pattern match has a one-second patience.
A timeout reaches the catch of `LFrequencyClerkRun`, which hands it to the fault owner.

## `LFrequencyClerk.LFrequencyClerkRun`

A fetch is admitted through a four-wide gate.
The answer is written only when the fetch still stands and the setting is still on.
The entry must still read the same headword and language.

## `LPronunciationClerk`

The pronunciations of an entry reconciled to its draft.
A draft holds an ordered list of pronunciation rows by id, and a save makes the store say the same.
The same reconciliation serves a create and an update, since a fresh entry is an entry with nothing stored.

## `LPronunciationClerk.LPronunciationClerkFind`

Each entry shows its first reading.
That is the stored respelling where `respelled` answers true for the entry's language and one is stored.
The pending ordering keeps only entries with no reading, and every other ordering keeps only those with one.
The phonology list thus never mixes the two kinds.

## `LPronunciationClerk.LPronunciationClerkFind`

Rows of a hidden language are left out, in the order given.
The engine calls here so that the filter rule has one owner.
It is static, since filtering reads no store and needs no gate.

## `LPronunciationClerk.LPronunciationClerkSync`

Only a seeded row still holding no IPA, respelling, recording or syllables is left out.
The draft seeds that row itself for a first reading, so emptied again it holds nothing written.
A blank row that was not seeded is kept and written.
A positive id naming no stored row of this entry refuses the commit rather than rebinding to a fresh row.
A stored row the draft no longer names is deleted, its recording with it.
Every row kept is rewritten only when it changed, so an unchanged row raises no change.
A new row is appended and then the whole list is placed in draft order.
The order is rewritten only when a row moved or the count changed, so an untouched list writes nothing.
Every row written is recorded on `changes`, on a create as on an update.

## `LPronunciationClerk.LPronunciationConsonantRead` and `LPronunciationClerk.LPronunciationVowelRead`

They hand the IPA charts up for every driver's input aid.

## `LPronunciationClerk.LPronunciationClerkScan`

It keeps the rows worth writing, with reading and respelling trimmed.
A seeded row still empty is the blank the form keeps to type into and is dropped.
The engine's draft match filters the same way, so a held draft and its origin compare on the same rows.

## `LPronunciationClerk.LPronunciationClerkSave`

A kept row keeps its id and its place.
So the recording hanging from it stays attached across an edit of the IPA.

## `LPronunciationClerk.LPronunciationClerkInsert`

The minted id is recorded against the draft id it replaces.

## `LPronunciationClerk.LAudioSync`

The draft carries the recording as a full path and the row stores it relative to the workspace.
So the comparison is made in stored terms.
An unchanged recording writes nothing, and an empty one leaves the stored recording alone.

## `LPronunciationClerk.LRecordingFormat`

The path is made relative to the workspace, so a moved workspace keeps its audio.
A path outside the workspace is stored as given.

## `LPronunciationClerk.LPronunciationClerkMatch`

The syllables are compared without their parent id, since the draft's carry none yet.

## `LRecordingClerk`

Recordings harvested, stored, resolved and swept for one rig.
The harvest sources of each language are built once from the pack and kept until the rig changes.
The session trove that remembers a harvest stays in the engine, so the clerk only scans and publishes.

## `LRecordingClerk.LRecordingClerkSave`

It answers the stored path, or null only when nothing was saved.
An `LVaultFault` means the download was refused or timed out, or a file was not written.
That fault reaches the caller unrecorded, since the gate that shows it records it once.
The shell names no file or network type, since the fault wraps the cause.
Any other fault, a cancellation the caller asked for among them, reaches the caller too.

## `LRecordingClerk.LRecordingClerkPrepare`

It fetches a playable local file without storing it, or null only when nothing was prepared.
Faults reach the caller as in `LRecordingClerkSave`.

## `LRecordingClerk.LRecordingClerkPlay`

A missing or unnamed file plays nothing, so the phonograph never opens a path that is not there.
The file is resolved through the trail, so a relative path plays from the workspace root.
It answers a fresh ticket naming this play, or zero when nothing played.
A `volume` that is not a finite number keeps the current level.
The app shares one phonograph, so a caller keeps its ticket to reach only its own play.
The list overload plays a run of the existing files in order, all under one ticket.
The first file plays at once.
Each next file plays when the phonograph raises `LPhonographFinished`.
Any new play replaces the queued rest of a run.

## `LRecordingClerk.LRecordingClerkStop`

It stops the phonograph and drops the run's queued files only while the ticket names the latest play.
A view clearing after another view played thus leaves that other sound running.

## `LRecordingClerk.LRecordingClerkClear`

It stops the phonograph whatever play is running.
The engine calls it on the old clerk before a workspace switch.
The phonograph outlives the clerk, so the old play would otherwise keep sounding.
The new clerk's tickets start over, so no ticket from before could reach that play.

## `LRecordingClerk.LRecordingClerkAdjust`

The level is clamped so the player never takes a level outside zero to one.
The app has one audio level, so a change reaches whatever plays.
A `volume` that is not a finite number is ignored.

## `LRecordingClerk.LRecordingClerkResolve`

Every recording path in the draft is made absolute, so a form can play it.

## `LRecordingClerk.LRecordingClerkSweep`

It deletes every stored recording that no pronunciation row and no draft names.
The claim clerk supplies the held drafts the sweep must keep.
Retained local recordings are resolved through the trail and compared without regard to case.

## `LScriptClerk`

Script images fetched per character and style, and stored.

## `LScriptClerk.LScriptClerkRead`

Images follow the order `LScriptImageSort` declares.
The headword's characters are handed to the sort, so the blocks follow the headword's spelling.
The portrait and the grouped read both read through here, so they show one order.

## `LScriptClerk.LScriptClerkRebuild`

It drops the stored images of every character of the entry and fetches them again.
The missed mark goes too, so a character the sources once denied is asked once more.
The script bulletin is raised once the fetches are started.

## `LScriptClerk.LScriptClerkScan`

It sends one request per style, in order.

## `LScriptClerk.LScriptClerkRun`

A fetch is admitted through a two-wide gate.

## `LShengfuClerk`

Fills and keeps the phonetic series of the characters an entry is written with.
It fetches one character at a time behind one gate, as the fanqie clerk does for rime books.
The rows it stores are read back by the fanqie clerk, which prints them above each character's books.
Storing a row also links its character to the Stem series the row names.

## `LShengfuClerk.LShengfuClerkStart`

A character already fetched, pending, or known to have none is left alone.

## `LShengfuClerk.LStemApply`

It builds the series of every pack that declares a series source again.
A workspace opens through it, so a workspace whose rows predate the links still gets them.

## `LShengfuClerk.LShengfuClerkScan`

It sends one request for the character, spaced by the rule's interval from the last one.
A language whose pack declares no rule answers nothing and is not reached.

## `LShengfuClerk.LShengfuClerkRun`

A fetch is admitted through a single-wide gate.
A series found is stored and linked to its Stem keys.
The bulletin names the fanqie subject, because the fanqie panel prints the series.

## `LStemClerk`

The read side of the phonetic series the xiesheng panel browses by.
It writes nothing.
The links are made where a series is stored, in `LShengfuClerk`.
A member's fold is written by `LFoldClerk`, and only read here.

## `LStemClerk.LStemClerk`

It takes the reflex clerk for the pack's reflex order, so members sort their rows as the entry page does.
It takes the language clerk, the one owner of each row's guise.
It keeps the fold store too, to read each member's own fold.

## `LStemClerk.LStemClerkFind`

The list is narrowed and sorted by the shared catalog rule in `LCatalogClerk`.
So the engine only marks the chosen row.

## `LStemClerk.LStemEntryScan`

Entries come back through `LCatalogEntry.LCatalogEntrySort`, the owner every entry list shares.
The archive reads in storage order, so no caller may show its rows unsorted.

## `LStemClerk.LStemPageRead`

Characters follow `LGlyphOrder`, by code point, so every view lists them alike.
The store hands them in the order their Shengfu rows were saved, which is only fetch order.
The flag `fold` says whether members read their own fold.
Without it no fold store is queried, and every member reads closed.
Each member gets its guises from `LLanguageGuiseScan` over its reflex rows, under the `respelled` setting.
So the series page and the Outpost series note show the rows the entry page shows.

## `LStemClerk.LStemEntryFind`

The entry of a member character is matched as the page matches it, or null for a bare member.
The engine finds the entry a member's fold is written on here.

## `LStemClerk.LStemMemberScan`

Entries the series reaches are read in one batch by `LStemEntryRead`.
They are matched to the characters by headword through `LStemMember.LStemMemberFind`.
It only reads, so a character without an entry stays bare and no entry is made for it.
Without characters or a language, member assembly uses `LStemMember.LStemBareScan` rather than entry and reflex reads.

## `LStemClerk.LStemEntryRead`

Entries come in id order, or none for a blank language.
The page and the fold write share it, so both match a character to the same entry.

## `LStemClerk.LStemMemberRead`

The member carries the character's representative readings and the reflex rows of its entry.
Its own fold is read from the fold store by entry and series key.
A bare member reads closed, and so does every member when `fold` is false, with no fold query.
The entry id stays local to this read, so the member keeps no entry id.
The reflex rows are read alone, not the whole entry.
They are then put in the pack's order and stripped of unwritten rows.
That is the order and the filter the entry page's reflex table shows.

## `LTranscriptionClerk`

Transcriptions of an entry and the lookups that feed them.
The pronunciation lookup and the per-scheme lookup both run here, over sources built once per language.
The tenure builds the receiver and hands it in, so it can read afterwards whether the lookup ended it.

## `LTranscriptionClerk.LTranscriptionClerkFind`

The lookup uses the pack's varieties and cleanups.
A pack with respelling groups puts a receiver in front of `receiver` that respells each candidate first.
A pack load or source build that throws here leaves `receiver` unended, since the lookup never started.

## `LTranscriptionClerk.LTranscriptionClerkFind`

The lookup runs over the sources of one scheme, raw.

## `LTranscriptionClerk.LTranscriptionClerkPublish`

The instance form replays remembered candidates respelled the way a fresh lookup would be.
The static form hands every held candidate to `receiver` and finishes the lookup.

## `LTranscriptionClerk.LTranscriptionClerkSync`

An unchanged list writes nothing.
A positive id naming no stored row refuses the commit.
A blank or repeated scheme refuses the commit.
The minted ids are recorded against the draft ids and one change summarises the list.

## `LTranscriptionClerk.LTranscriptionClerkReset`

Every positive id is cleared, for a fresh entry that has no stored rows to match.

## `LTranscriptionClerk.LSchemeSourceRead`

The sources come from the scheme's declaration or the glyph's source for it.

## `LTranscriptionClerk.LTranscriptionClerkMatch`

Row ids, schemes and texts must all match in order, so equal ids alone never skip a changed transcription.

## Stamp

Commit: `bd299c638b2c`
- `LDiweiClerk.cs` `ac530a588201a30b`
- `LFanqieClerk.cs` `9bb477462abde638`
- `LFrequencyClerk.cs` `270209f96a539217`
- `LPronunciationClerk.cs` `e2299bcbc0df24e1`
- `LRecordingClerk.cs` `48000bca3b2ab2a7`
- `LScriptClerk.cs` `dfb8ad93fee23103`
- `LShengfuClerk.cs` `aa18d84636cbfc5d`
- `LStemClerk.cs` `492dc81c614a52d1`
- `LTranscriptionClerk.cs` `54eefcb680d49b9a`
