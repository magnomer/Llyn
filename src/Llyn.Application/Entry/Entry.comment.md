# Entry

The Entry folder holds the clerks over stored entries and the objects they reference.
It covers the lifecycle, the lookups, the grasp rating, the revision stamp and usage counts.
Each clerk runs over the ports of one rig and holds no gate, no observer and no cache.
The engine calls them under its own gate and raises the bulletins.
Searching and grasp stay apart from the lifecycle, since neither edits the word.
The etymology map's clerks sit in the `Pedigree` child folder.

## `LCatalogClerk`

The catalog helpers the shell reaches without naming the catalog itself.
`LCatalogClerkFormat` formats an absent filter as the empty one.
`LCatalogClerkCreate` builds the filter by the filter's own rule, and a list hiding nothing is the shared empty filter.

### `LCatalogClerkFind`

The one rule every catalog column narrows and sorts by, so series and diwei lists agree.
Rows whose key holds the trimmed query stay, ignoring case, and a blank query keeps all.
Reverse sorts keys descending, usage sorts by count descending then key, and any other order sorts keys ascending.

## `LEntryClerk`

The interactor over the entry lifecycle the engine does not own itself.
An Entry is created, read, loaded back as a draft and deleted with the history that leaves.
The engine raises the bulletin a save or a delete deserves.
The delete is the shape every spanning operation here follows.
That shape is several stores, one session, one decision.
The commit of a draft into an entry is composed out of the clerks for each part of an entry.
The revision clerk records every revision and moves the workspace row onto it.
The recording clerk resolves the recording paths of a loaded draft.
The frequency clerk clears a renamed entry.

### `LEntryClerkLoad`

The entry as a draft, or null when no entry has that id.
Every recording path is made absolute, so a form can play it and a draft match compares like with like.
Its reflex rows come in the order the pack declares, through `LReflexClerkSort`.
The entry facade, citation clerk, outcome clerk, markup intake and portrait load through here.
So none of them sees storage order.

### `LEntryClerkDelete`

Deletes the entry with everything it owns, then writes the history the deletion leaves behind.
That is a revision carrying one delete change for the entry.
It is also a tombstone filed under that revision, and the workspace row moved onto it.
The delete runs first.
So a refused delete records no history at all and throws its own message through.
A link from another entry does not refuse the delete.
The schema deletes the linking row with the entry.
All four writes share one session, so they are one transaction.
Without it a failure part-way would leave an entry deleted with no tombstone naming it.
That is history that no longer describes the file.

### `LEntryClerkSave`

The create overload saves the whole input form as one new entry and records the revision that creates it.
That is the headword row with its lexical unit and freedom, and its meanings and collocations in card order.
It is also its note, inflections, pronunciations, transcriptions, reflexes and ancestor boxes.
The note is stored as Markdown normalized by `LMarkdown`.
The revision holds the entry's create first and then a change for every child row written.
Each child is written through the same clerk and seam an update uses, onto an entry with nothing stored.
So a create records exactly what an update adding the same rows to an empty entry would.
A blank headword is refused before any session opens.
The outcome clerk normalizes the draft and trims the headword before handing it over.
Everything runs inside one session, which makes a half-written entry impossible.
A failure at any write rolls back every write before it, leaving no entry row behind.
A card with every field blank writes nothing.
The form keeps an empty card on screen to type into and refuses to remove a list's last card.
Deciding that a blank one is neither belongs here, not in the form.
The entry row is written bare, and its parts of speech and forms follow through the update seams.
Text naming a preset the entry's language declares is stored as that preset's stable id.
Text naming no preset is stored as typed, because the field is editable.
A positive pronunciation, transcription or reflex id the draft still holds names a row of an entry since deleted.
It is reset to zero first, so the commit writes the row anew instead of refusing it.
A negative id is kept, because the identity map records what it became.

The seam overload applies the draft to an existing entry and returns it as it now stands.
An id no entry carries is refused before anything is written.
The entry keeps its opaque id and its `added_utc`.
Only `updated_utc` moves, and only when the draft differs from the entry as loaded with its recordings resolved.
The headword, language, lexical unit and freedom are written together, only when the draft differs.
A changed headword or language clears stored frequencies through the clerk.
No stale figure then appears under the new headword.
Every change is appended to the changes list and no revision is recorded here.
The markup import fills many entries under one revision through the same seam.
Which stored card a draft card is comes from `LCardDraft.LCardDraftId`, never from its place in the list.
The meaning clerk reconciles the tree and the card clerk the flat list.
Every part is compared before it is written.
A field that did not change writes no row and records no change.
The ancestor boxes are saved after the other child rows, through `LEntryClerkAncestor`.
The paradigms are refreshed last.

### `LEntryClerkUpdate`

The save onto an entry with its revision recorded, in one session.
A save that changed nothing records no revision.

### `LHeadwordValidate`

Refuses a draft whose headword is blank, the one refusal both saves share.

## `LEntryClerkField`

The pure field helpers of the entry save.
They sit beside the entry clerk so the clerk stays under its line ceiling.
`LPronunciationReset` clears every positive id, for a fresh entry that has no stored rows to match.
`LFormUpdate` rewrites the forms from the draft when they differ, with one change recorded.
`LNoteUpdate` saves, rewrites or deletes the note to match the draft, and an unchanged note writes nothing.

## `LEntryClerkTwin`

The twin naming every catalog of entries or labels runs through.
Two rows sharing a name are numbered so a list never shows the same words twice unmarked.
Nothing here reads a store, so the helpers are static over the rows handed in.
The engine's vista and the markup clerk number through it.

### `LTwinRead`

The twin name of each entry, by position, numbered by entry id.
So the older entry is `(1)` in every view.
Only entries of one language are twins, so one headword in two languages stays bare.
The generic overload numbers any row kind, given how to read a row's name, group and id.

### `LTwinNameResolve`

The labels made distinct in their given order, numbered where two share a name, for the compass rows.
Labels carry no language, so they all form one group.

### `LTwinNameRead`

The wording a catalog row shows for a state value.
An unknown value shows the unknown text, an empty one shows the fallback, and any other shows its text.

## `LEntryQueryClerk`

The lookups over stored entries, apart from the lifecycle that writes them.
It reads the entry port and writes nothing.
The facades, the vista, the courier and the markup intake all search through here.

### `LEntryFind`

The text search returns entries whose headword contains the query, ordered by headword.
It returns every entry when the query is empty or all whitespace.
Matching is a contains whose case is folded over the whole of Unicode.
So an accented headword is found typed in either case.
The order overload lets the store answer which entries match and orders over what it returned.
The filter overload leaves out the entries in a hidden language.
The library panel lists through it, so the filter is applied here and never in the shell.
The tag, register, situation, example and reference overloads match by id and order by headword.
A zero id stands for nothing chosen and returns every entry, except for a register.
They take typed objects rather than text so the searches cannot be confused.
A card cites a Source through the Example it holds, so the reference search goes through that Example.

### `LEntryHeadwordFind`

The stored entries with the headword in the language, both trimmed.

### `LEntryLanguageRead`

The language an entry is filtered by.
Both filtered lookups share it, so the filter never reads the language two ways.

### `LEntryMatch`

The entries whose headword matches by the catalog match, and a blank query narrows nothing.
The engine's vista narrows a child list the same way, so it calls here.
The filter overload also applies the language filter and orders with the entry catalog's sort.
So equal headwords follow its tie rule, and the shell holds no filter or order rule.

### `LEntryTallyFormat`

The library's count line, worded through `LCatalogCountFormat` under the `Library.Shown` keys.
It reads no store, so the vista facade calls it with the count of the entries the vista lists.

### `LEntryEpithetScan`

The epithet of every listed entry that has one, keyed by id, in one session.

## `LGraspClerk`

The user's grasp of an entry, a half-step rating stored on the entry row.
A rating is a reading mark.
So it stays apart from the clerk that edits the word and records no revision.
`LGraspClerkStep` is the number of steps for the shell to draw.
`LGraspClerkScan` gives the star answers, so no layer above Application calls Core for the fill rule.

### `LGraspClerkSet`

The range is checked before any write, so a bad value reaches neither the store nor a subscriber.

## `LRevisionClerk`

The revision stamp every clerk that writes history shares.
A stamp records one revision and moves the workspace row onto it.
Entries, translation stubs, citations and the markup import all stamp through here.
So the pointer to the current revision can never drift from the revision recorded.

### `LRevisionClerkRecord`

Every recorded revision moves the pointer, so the current revision is read from where it is recorded.
Both writes share one session, so a caller's open session folds them into its own transaction.
The internal overload records a single change for a target and subject, reading create when fresh and update otherwise.
Every citation commit stamps through it, so authors, examples, sources and situations word their change alike.

## `LUsageClerk`

The clerk over usage, asked of every object a card references rather than owns.
An Example, a Situation, and a Reference are all independent data cited from any number of places.
Each is browsed by a panel that must say how often one is cited and by what.
So the question is asked once here for every kind, rather than once under each name.
An Author is asked the same question, answered by tracing the Sources they are credited on.
`LOwner` names the kind being asked about, not the referring side.
The referring sides come back inside the answer.

### `LUsageClerkRead`

The count overload gives the reference count of every object of that kind at once.
Reading it once per catalog fill costs one statement rather than one per row.
Only an Example, a Situation and a Reference are counted this way, and any other side is refused.
The itemized overload says which side cites an object, and which id the row is followed by.
An Author is traced through Source, Example and card up to the Entry.
Nothing stores that trace, so no row can contradict the chain it is read from.

### `LUsageTallyRead`

The citation line of one Source, Example or Situation, from its count and its realm's usage wording.
A missing record or one nothing cites reads as none.

### `LUsageClerkResolve`

Twins the headwords within each language and fills each row's epithet on request.
The translation clerk names its incoming rows through it too.

## Stamp

Commit: `121691f60459`
- `LCatalogClerk.cs` `56685f63108c1e84`
- `LEntryClerk.cs` `f9953b583dee4375`
- `LEntryClerkField.cs` `810515c631a82ce2`
- `LEntryClerkTwin.cs` `190654e34a95c306`
- `LEntryQueryClerk.cs` `f74133704df6ad53`
- `LGraspClerk.cs` `910cf0c01ca7378d`
- `LRevisionClerk.cs` `742cdb16bc8c0050`
- `LUsageClerk.cs` `824bbb12aa126b62`
