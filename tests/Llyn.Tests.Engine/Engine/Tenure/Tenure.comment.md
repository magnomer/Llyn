# Tenure

This folder holds the tests of the tenure sequence, subject commits and field edits.
They also cover the quills, easel and forays over it, and a new gloss language.

## `TEasel.cs`

The easel's media edits run one fact per member, each over a fresh situation tenure.
Child card facts hold a fresh entry instead, since only an entry has cards.
`TEaselHold` is a wait no test outlives, so a deferred edit stays queued until the fact flushes it.
An added picture or video row starts with no location and is dropped by its id.
A row on a child card is dropped too, since the card find walks the children.
A typed picture location waits in the queue and lands on the flush.
A chosen video file lands at once, even under a long delay.
A typed timestamp lands on the video row.

## `TEngineGloss.cs`

It covers the language a new translation of an Example starts in.
The default settings name English, which the loaded packs hold, so English is picked.
A settings file naming a language with no loaded pack falls back to the first loaded language.

## `TEngineTenure.cs`

It holds the tenure's sequence, one fact per promise the panel relies on.
Each fact starts a tenure on a fresh workspace and drives it through the relays alone.
The delay seam is a long hold where the queue must be seen, and zero where writes land at once.
The commit of each subject lives in `TEngineTenureSubject`, and starts from a given row in `TEngineTenureVista`.
`TTenureHold` is a wait no test outlives, so a deferred request stays queued until the fact flushes it.
`TEngineTenureSubject` and `TEngineTenureVista` wait through it too.
Two deferrals of the same field leave only the second, and nothing is written until the flush.
Finishing a draft nobody changed stores nothing and drops the draft, answering null.
A request whose draft is gone halts the tenure.
After that the finish refuses and the undo steps nowhere.
An undo writes the deferred request first, so it steps back from what was on screen.
The clock is advanced past the chronicle's merge window, so the two writes are two steps.
A fresh blank entry reads unchanged and not storable.
Two deferrals of one gloss text reach the chronicle as one step, so one undo clears the gloss.
A request the engine refuses is dropped and the tenure runs on.
The headword deferred after it still applies, and a draft bulletin is raised so a panel refills.
Finishing without storing cancels before the queue is written, so the discarded text never reaches the draft.
No draft bulletin is raised then, since nothing was applied.

## `TEngineTenureSubject.cs`

It covers how a finished tenure commits each subject it can hold, on a fresh workspace.
A changed example, entry, situation or reference is stored, its draft dropped and its id answered.
The situation title names the Situation the draft holds, as the scenario's own edit does.
An author tenure is ready only once its name is written.
The ready check writes the queued name first, so the long hold never has to pass.
A tenure on a stored Author renames it in place.
Finishing a tenure that blanks a stored Author's name throws the name-missing `LRefusal` and keeps the stored name.
Only the ready check sets the delay to `TEngineTenure.TTenureHold`, and the rest set it to zero.

## `TForay.cs`

It covers a foray as the tenure hands it out.
That means its search, the cancel that ends it, and the pick that follows.
A cancelled search never tells its listener it finished, so the menu is not told a lie.
A pick after the headword moved attaches nothing.
A pick over an unchanged draft attaches the audio and writes the recording's variety onto the row it filled.
A preview fetch answers the cached file and attaches nothing.
A refused host makes the preview fetch and the pick pass the vault fault up.
So does an audio or cache folder blocked by a file.
The recording clerk lets the archive's `LVaultFault` pass up to the gate.
A flag folder blocked by a file stores no flag, since the flag loader in Infrastructure answers no path.
A cancelled tenure takes its foray down with it.
A recording or transcription start searches the trimmed headword, and a blank one starts nothing.
A start over a draft the workspace change made stale starts nothing and throws nothing.
A pack that shows no flags loads none.
The gated client holds every source open until the test releases it.
The recording pack is internal, so `TForayFailure` runs the same pack without a copy.
How a search fails or finishes lives there.

## `TForayFailure.cs`

It covers how a foray's search fails or finishes, and what it writes to the audit file on the way.
A search that answers tells its listener it finished exactly once and writes no fault.
A search that throws is written to the workspace's audit file, and its listener is told it finished exactly once.
That holds for a source failing inside the harvest or lookup, whose relay sends the end.
It holds as well for a source factory failing before either starts, where the tenure sends it.
Each case waits, within a bounded limit, until the audit file holds the asserted fault and the end has arrived.
Then it counts the end steps.
The expected factory fault is read from `TRigFake`, so the text lives in one place.
A sink that throws on the tenure's end is recorded beside the first fault.
The end is still sent once.
The recording pack comes from `TForay`, and the lookup pack is this file's own.

## `TForaySettle`

It polls the condition until it holds or five seconds pass, then asserts it.
A slow machine fails with a plain message instead of a fixed sleep racing the work.

## `TForayAuditRead`

It answers the workspace's audit file text, or empty while the file is missing or held by its writer.

## `TQuill.cs`

It covers the per-subject quills' edits to a held draft, one fact per edit.
Each fact starts a tenure on a fresh workspace with no delay, so a deferred edit lands at once.
A typed author name or example text lands and marks the draft changed.
Typed example text lands as known even over an unknown value.
A picked speaker language lands on the held Example.
A picked Source becomes the held Example's citation, or a card sentence's citation.
An inserted Gloss takes the language the engine resolves for a new Gloss.
A typed gloss text and then a chosen language both land on the same Gloss.
An added Gloss is dropped by its id.
A field left empty whose held value is unknown stays unknown, while the typed title lands.
The scenario's three edits are sent one field at a time, as the form types them.
A typed narrative lands on the held entry's etymology.
A source added at position zero goes ahead of one appended at the end.
It is dropped by its entry id.
A span of the narrative links to the given entry at the given offset.
A failing court delete after a translation removal reaches the caller instead of leaving an orphan unseen.
The chip runs over a fake draft port whose court delete throws.
`TQuillEntrySave` stores an English Entry with one sense, and no fact calls it.

## `TTenureField.cs`

It covers the tenure's entry field edits, one fact per member.
Each fact but the static note check runs over a fresh entry with no delay.
A typed headword lands and marks the draft changed.
A typed note lands without its trailing line breaks.
A typed text holds the note when only trailing line breaks differ.
A space or a leading break still makes it another note.
An empty language choice leaves the chosen language as it was.
Typed speech lands trimmed as a pending part of speech once the deferred request is flushed.
In an English draft the answer opens the catalog offer, since the catalog names a noun.
A typed phonetic reading and a typed respelling land on the primary pronunciation.

## Stamp

Commit: `c2f335c331fe`
- `TEasel.cs` `f85c1bca81bff00c`
- `TEngineGloss.cs` `c1402f7638e6237b`
- `TEngineTenure.cs` `ac07d8236f6f41a4`
- `TEngineTenureSubject.cs` `f6a05df885035ad7`
- `TForay.cs` `c61708681cad4fd0`
- `TForayFailure.cs` `039821f324ec1d94`
- `TQuill.cs` `4d5a62840b1fac55`
- `TTenureField.cs` `b93c929cdeed7a0c`
