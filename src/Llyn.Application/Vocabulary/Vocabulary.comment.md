# Vocabulary

The clerks over the controlled vocabulary and the forms built on it.
They cover parts of speech, morphology, inflections, paradigms, lacunae, units and freedoms.
They stand apart from the pronunciation clerks because they work on grammatical identity, not on sound.

## `LFreedomClerk`

It offers the freedoms an entry may take, and the key that names each.
Every language and every unit offers the same freedoms, because the user alone decides which one fits.
The offered list is Dependent and Standalone, in the order the dropdown shows them.
`LFreedomFormat` reads `LFreedomKey` for the engine, which names no Core rule itself.

## `LUnitClerk`

It offers the units an entry may take, and the key that names each.
Every language offers the same units, because the user alone decides which one fits.
The offered list is Morpheme, Word and Expression, in the order the dropdown shows them.
`LUnitFormat` reads `LUnitKey` for the engine, which names no Core rule itself.

## `LInflectionClerk`

It is the clerk over the inflected forms an Entry owns.
Each carries the ordered grammatical features that say what form it is.
Unlike a Tag or an Example, an inflection is not independent data.
It belongs to its Entry and is identified by its place in that Entry's list.
That is why these seams are keyed by the Entry, not by an id of their own.
The clerk deletes the Entry's lacuna rows when a hand edit invalidates them.
Cancelling the fetch that may still be filling them is the engine's, since the engine owns the task.
The regular flag is not judged here, and the entry save asks the paradigm clerk for it after its writes.

### `LInflectionClerkValidate`

It refuses a list naming a part of speech or a morphology value the workspace no longer holds.
The store would refuse it as a foreign-key failure.
That reaches the reader as a crash and not as an answer.
Every other link a draft carries is checked before it is written, and an inflection is no different.

### `LInflectionClerkReset`

It deletes the Entry's lacuna rows, so a form the web could not name before may be asked again.

### `LInflectionClerkUpdate`

The stored list becomes the list the draft holds, which is what an editor showing every form does.
A feature list that differs at one place is a different inflection, so the whole set is rewritten.
An unchanged set writes no row and records no change.
A new entry is saved through here too, so its first inflections are recorded as a create.

### `LInflectionClerkMatch`

It compares two lists for the same forms with the same features in the same order.
The engine's draft match compares a held draft to its origin through it.

## `LLacunaClerk`

Morphology answers fill unspecified paradigm slots without overwriting specified forms.
A held draft with unsaved changes to the forms or what decides them blocks fetching and applying.
A held draft that still matches the entry there is filled with the stored forms instead.
So an editor reopened on a saved entry neither loses the answer nor erases it on its next save.
The shared gate serializes fetch bookkeeping and vault access.
The settings are read at fetch time, so a setting turned off mid-fetch discards the answer.

### `LLacunaClerkStart`

Nothing starts for an entry whose fetch went unanswered earlier in this session.
Nothing starts when the setting is off or the language declares no morphology source.
It first checks the stored analysis against the pack's rule book.
A blocking draft or a pending fetch stops it.
That check runs whatever the setting and the slots.
A stored form a slot shows is stale when its stamp differs from the book's.
A missing stamp counts as different, so rows written before the book and imported rows are caught.
Covered and uncovered rows alike carry the stamp once analysed, so each is analysed once per stamp.
The entry is then analysed again from its stored text by `LParadigmClerkUpdate`, with no network.
The slots are read once and serve both the stale check and the fetch check.
A re-analysis raises the inflection subject, so a shown box repaints with the new marks.
A blocking held draft or a pending fetch leaves the entry alone, as its forms are about to change.

### `LLacunaClerkCancel`

It cancels the pending fetch and deletes the lacunae, before a save rewrites the inflections.
It also forgets an unanswered fetch, so an edited entry is asked again.

### `LDraftFind`

A draft blocks when its headword, language, parts of speech or forms differ from the stored entry.
Any other change leaves the stored forms valid, so that draft does not block.
A draft whose entry can no longer be loaded blocks.

### `LDraftPropagate`

It writes the entry's stored forms into every held draft of the entry and returns their ids.
It runs only after an answer was stored, when no held draft blocks.
Each of those drafts matched the entry before the answer, so it loses no edit of its own.
Without this, the draft's next save would set the entry's forms back to the draft's empty list.

### `LLacunaClerkScan`

It asks every source for the word and keeps the first form each reading variety got.
The flag tells whether any source answered at all, so silence is told apart from an empty answer.
A variety is a slot key, a bare code or the joined codes of a multi-value cell.

### `LLacunaClerkApply`

It appends one inflection per unspecified slot the answer filled and records the rest as lacunae.
The answer is keyed by the slot key, so a multi-value cell finds the reading named after its codes.
The inflection carries every value of the cell.
A missed multi-value cell is stored with its key, and a missed one-value slot by its id alone.
It returns the ids of the held drafts it filled, or none when the answer added no form.

### `LLacunaClerkRun`

The answer is written only when the fetch still stands and the setting is still on.
The entry must still read the same and no blocking draft may hold it.
A source that never answered is not an answer and stores nothing.
An unanswered entry is remembered in memory for the session only.
The same fetch does not start again until a restart, a clear or an edit.
The fetch is never awaited, so a failure is recorded in the fault log rather than lost unobserved.
Cancellation by the fetch's own token is not a failure and is not recorded.
A finished fetch raises the inflection subject, answered or not.
Then the draft subject is raised for each filled draft.

## `LParadigmClerk`

It answers which forms an entry is expected to have and which of them are stored.
A language pack declares the forms by code on a part of speech.
The clerk resolves those codes to the workspace's rows and pairs each with a stored inflection.
Persisted lacunae distinguish reached-but-missing forms from slots that have not been answered.
A lacuna without a cell key or morphology id names no slot and is ignored.
Speech packs remain cached for the clerk's lifetime.

### `LParadigmClerkRead`

It reads every slot of an entry already in hand, shown or not.
The background fetch check shares it, so an entry is read once per question.
Lacuna rows with a cell key mark missed multi-value cells, and the rest mark one-value slots by id.

### `LParadigmClerkShow`

It drops the slots the display need not show.
A specified slot whose stored form carries the regular flag is dropped.
So a noun whose plural is `cats` shows no plural row, while `mice` keeps one.
The stored regular flag controls hiding even when the paradigm declares no fallback rules.
The flag was derived when the form was stored, so showing runs no rule.

### `LParadigmRowRead`

Slots declared on the part a rule book's layout names are dropped.
The inflection view shows that part instead, so the list never repeats it.
A pack without a layout keeps every display slot.

### `LParadigmLanguageRead`

It reads every slot, not only the shown ones.
So an entry whose forms are all regular still names its language for the view's font.

### `LParadigmClerkBuild`

It answers null for a missing entry, a blank language, or a pack without a layout.
It also answers null when no slot lies on the layout's part.
It hands the view the language's rule book and the headword, so custom cells can split root from ending.
The flags `pending` and `enabled` feed each cell's status, as they do for the list.
The flag `held` passes on unread to the view, which picks each lost cell's tip by it.
The flag `custom` picks the custom sheets with their marks, or the default sheets without marks.
Custom cells carry the stored marks, widened to the root or ending when the cell is divided.
Every cell of both tables also carries its painted pieces from `LParadigmClerkScan`.

### `LParadigmClerkScan` table

The clerk owns this call, so Conduct maps pieces as data and never reaches Core.

### `LParadigmClerkFind`

An entry without a language has no pack, so it answers null.
The list, the view and the judgement all reach the rule book and its layout through it.

### `LParadigmClerkUpdate`

It judges every answered slot and stores the result where it changed.
A pack without a rule book judges by `LParadigmClerkMatch` and stores the flag alone.
A pack with a book predicts each answered slot once with `LInflectionBookResolve`.
A covered slot's stored text is compared with that prediction by `LInflectionDifference.LInflectionDifferenceScan`, through the book's folds.
It stores the prediction, the marks, the book's stamp and the flag together through `LInflectionAnalysisSave`.
A slot the book does not cover stores a null prediction and null marks.
It carries the book's stamp all the same.
So the background check reads an uncovered row as analysed by this book and leaves it alone.
Its flag falls back to `LParadigmClerkMatch`, so the pack's own `regular` rules judge it.
A covered form is regular when nothing is marked and it is not empty.
So an absent form never counts as regular and stays shown.
An entry without a language has no pack and no paradigm, so nothing is judged.
Analysis depends on the forms, headword and parts of speech, so callers must refresh it after those change.
It also runs when the background fetch check finds an analysis made by an older book.
It reads only stored text and never the network.

### `LParadigmClerkScan` pack

It collects the paradigms that apply to the part of speech named by the code.
It first walks the part's parent chain, nearest first.
The walk stops at a repeated code or after as many hops as the pack has parts.
A paradigm declared on any code of the chain applies.
It is skipped when one of its excepted codes is also on the chain.
So a plural declared on nouns reaches countable nouns and stops at uncountable ones.

### `LParadigmClerkResolve`

A part typed by hand has no code and answers nothing.
Each code of a cell is resolved to a morphology row.
A cell with a code the workspace lacks is skipped.
A set of rows already given a slot by a nearer paradigm is not given a second.
The slot's inflection is the first stored one carrying every row of the cell and naming this part or none.
A one-value slot without one is unknown when its morphology id is in `missed`.
Any slot without one is unknown when its key is in `missedCells`, and unspecified otherwise.
Each slot keeps the paradigm it came from, so the display seam can judge regularity without scanning again.

### `LMorphologyResolve`

The pack says which feature and which part the code belongs to.
The archive is asked for exactly that row.
A code the pack does not declare answers `null`.

### `LParadigmClerkCheck`

It asks `LParadigmSlot.LParadigmSlotShow` of the row's first slot.
So the list and the inflection view share one status rule and one wording.

### `LParadigmClerkMatch`

Each rule matching the headword predicts a spelling, and the form is regular when it equals any prediction, case ignored.
A paradigm stating no rules answers false, so a form is never regular by default.
An empty headword or form answers false.

## `LVocabularyClerk`

It is the clerk over the controlled vocabulary.
It covers the parts of speech a language uses and the morphology features each of them takes.
Lexical rows store only stable ids.
An Entry's part of speech is `noun`, not the word "noun".
What an id is called in a language is resolved here.
So a display name is never copied onto an entry's rows, and renaming one renames it everywhere.
The one exception is deliberate.
The part-of-speech field is editable, so text no preset declares is stored as text.
A part of speech a pack has not thought of is still the one the user meant.

The vocabulary is seeded from the language packs, not from anything compiled in.
Every pack on disk declaring a `vocabulary.json` is written into the workspace when the engine binds to it.
The rows are keyed by language, pack code and their parents.
So a second run rewrites the same rows, keeps their ids, and adds nothing.

### `LSpeechRead`

The parts of speech are ordered by `LSpeechPack.LSpeechPackSort`.
So the pack's values come in pack order, and the typed ones follow alphabetically.
Every view reaches the list through here, so no caller sorts it again.
The pack is read once per language and kept, since a pack file does not change while the app runs.
Those are the presets the shell's part-of-speech field offers in its dropdown.
A language whose pack declares none returns an empty list, and the field is still editable.

### `LSpeechAdd`

It takes a part of speech no preset declares and makes it one of the language's presets.
A name a preset already carries is returned as it stands rather than declared twice.
Matching is on the trimmed name, without case, because the field is typed into by hand.
A new preset takes a negative code, outside the space a pack owns.
Its stored position is the next free one, but no view orders by it.
It returns `null` when either argument is blank.

### `LSpeechShow`

It names the parts of speech for the surfaces that show words rather than ids.
A draft that names nothing shows nothing.

### `LSpeechResolve`

A draft linking a value row that still exists is stored under that link untouched.
A draft linking a value row that is gone is refused rather than quietly dropped or rebound to its text.
A draft carrying only typed text is looked up in the language's vocabulary first.
Text that names a value is filed under that row, and text that names none is kept as typed.

### `LSpeechUpdate`

The assignments are one owned set with an entry parent and position identity and nothing referencing them.
So they are replaced wholesale rather than matched row by row.
A field cleared leaves the entry with no assignment, which is the set replaced by an empty one.
A field that comes back the same row writes nothing and records no change.

### `LSpeechFormat`

A linked value shows its current name, and custom text shows itself.

### `LSpeechMatch`

A row is the same row when it links the same value row at the same position.
It is also the same when it carries the same typed text at the same position.

### `LSentenceOrderRead`

It reads which of the two Example frame fields the language writes first.
A language naming neither, or naming no pack at all, reads back the marker first.

### `LSentenceParticleRead` and `LSentenceDependenceRead`

They read the markers or roles already saved under Entries written in the language.
An unnamed language reads back nothing rather than every marker of every language.

### `LLanguageImport`

Every language pack's vocabulary is written into the workspace, in one session.
The parts of speech go first, then the features under them, then the values under those.
A pack names its parents by code, and the import maps each to the row id it was given.
A feature or value whose parent the pack never declared is skipped.
Each row is keyed by its parent and code.
So writing it again updates the wording and keeps the row id.
The pack's retired parts are applied last, so their links move onto rows already written.
A pack that declares no vocabulary contributes nothing and is not an error.

## Stamp

Commit: `c2f335c331fe`
- `LFreedomClerk.cs` `6ec52b4723d6fc17`
- `LInflectionClerk.cs` `12e79993ef68b91e`
- `LLacunaClerk.cs` `0edf452de718c310`
- `LParadigmClerk.cs` `b0cc836f2362ab02`
- `LUnitClerk.cs` `338cd4c7dbfba260`
- `LVocabularyClerk.cs` `3886e24328b42f7c`
