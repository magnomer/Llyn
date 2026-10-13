# Clerk

This folder owns draft request orchestration, entry fields, equality, shared list rules, normalization, panel drafts and speech chips.
Concern-specific card and row handlers live in its child folders.
A form sends one edit at a time, and the clerks answer with a new draft, never a mutated one.
This folder stands apart from Court because it edits draft content and touches no tentative-record court.
Existing items are addressed by id, while additions and shifts carry positions.
So a form and a clerk cannot disagree about which existing item is meant.

## `LDraftClerk`

It applies one `LRequest` to one held draft over the ports of a rig.
It holds no gate, no observer and no file.
The engine keeps those and calls it under its own gate.
Each request family is applied by its concern class, including owners in the child folders.
Each such class answers null for a request not its own, so the clerk asks them in turn.
The clerk keeps only their construction, the default card and sentence targets, and the order they are asked in.
A clerk is built over one rig and is rebuilt when the engine takes a new rig.
It reads no port after construction that the rig did not hand it.

## `LDraftClerk.LDraftClerk`

Situation and Register chips each get their own class, `LSituationChip` and `LRegisterChip`, over their own shelf.
`identity` mints the negative id every new row carries.
`languages` answers the pack of a language, cached, for respelling and anatomy.
Neither is a port, so the engine hands them in beside the rig.

## `LDraftClerk.LDraftClerkNormalize`

It gives every unidentified row an id, through the mint.
The mint also drops the blank rows of the kinds it treats as nothing.

## `LDraftClerk.LDraftClerkApply`

The draft form asks the owners of the kinds that reach past the entry content, in turn.
Situation media go first, then the sentence, source and credit panels, Mentions, Glosses and situation fields.
Everything else falls through to the content chain.
The content form asks the entry fields, the cards, then the reading, reflex and etymology rows.
The card lists are asked last.
A kind no owner knows is a programming error, not a refusal, since no form can send one.
A null text or value is read as empty.
So a request can never leave a null where the draft holds text.

## `LDraftClerk.LDraftListApply`

It settles a request naming card or sentence zero onto a default target first.
Every other kind goes to the sentence, situation, Register, tag, translation, picture and video owners in turn.
A kind none of them knows is a programming error, not a refusal.

## `LDraftClerk.LCardResolve`

A request that named card zero is applied to the first Meaning card, made when the draft has none.
A browsing panel planting a Tag, Register, Situation, Example or Source into a fresh entry asks this way.
The panel need not read the draft back to learn which card the reset made.

## `LDraftClerk.LSentenceResolve`

A request that named sentence zero is applied to the card's first sentence row, made when the card has none.
A card the draft does not hold is refused, as any card request is.

## `LDraftClerkEntry`

The entry-level fields of a draft: headword, language, note, parts of speech, lexical unit and freedom.
Each is held by the entry itself and replaced whole by its request, outside any card or row list.
The pack cache lets a language change rebuild respellings and anatomy.

## `LDraftClerkEntry.LEntryApply`

It routes every entry-field request to its field, and answers null for any other request.
A language change derives every pronunciation respelling again and recuts every reflex row's anatomy under the new pack.
A language change leaves the unit and the freedom as they are, since the user alone chooses them.
A null text is read as empty, so a request can never leave a null where the draft holds text.
A headword or language change drops no recording here.
The engine does that after the apply, since only it can read the stored entry the draft edits.

## `LDraftClerkEntry.LNoteResolve`

It is the one owner of the note's trim, read by the set and by the check.
A text box carries trailing line breaks that the note never keeps.

## `LDraftClerkEntry.LNoteCheck`

It uses the same trim the set applies.
A driver asks it before painting, so a line break just typed is not overwritten.

## `LDraftClerkEquality`

It decides whether two forms of the same record say the same thing.
Every call is static and answers by content, because records compare lists by reference.
A reference comparison would call every reloaded draft a change and never settle.
This is what tells held work apart from the record it was started from.
The engine asks it for the dirty check and the chronicle, and the save asks it whether anything changed.

## `LDraftClerkEquality.LDraftMatch`

The held-draft form compares drafts whatever kind of record they hold.
An example, situation or reference draft is compared through that record's own clerk.
A reference draft also compares its author draft through the author clerk.
An author draft is compared by held name, trimmed, because a name is all it carries.
An entry draft is compared through its content, so blank scaffolding is ignored as it is everywhere.

The entry form compares field by field.
Language sits beside the headword, because changing only the tongue is still an edit.
The lexical unit and freedom are compared too, so choosing either alone is an edit.
The headword is compared trimmed and the note canonical, since the commit stores both that way.
A form that differs only there would otherwise report itself changed forever.
The lists inside are compared by their contents.
The Meaning and Collocation cards are compared through `LCardEquality`, which owns the card rule.

## `LDraftClerkEquality.LTranscriptionScan`

The rows that count are every row the user added, and a seeded row once it holds text.
A seeded row still blank is left out, because the form offered it and nobody asked for it.
The transcription sync writes the same rows, so the match and the save agree on what a row is.

## `LDraftClerkEquality.LReflexScan`

It keeps the reflex rows that carry something, in their original order.
The reflex sync and the reflex fetch read the same rows.

## `LDraftClerkEquality.LPronunciationMatch`

A seeded row still blank is left out first, because the form offered it and nobody asked for it.
A row the user added counts even blank, so adding one is a change.
The lists inside a row are compared by content, because two equal lists are rarely the same object.

## `LDraftClerkEquality.LTranscriptionMatch`

Transcriptions compare in order, with a seeded blank row left out as it is for pronunciations.

## `LDraftClerkEquality.LReflexMatch`

Reflexes compare in any order, with blank rows left out.
Every view lists reflexes in the pack's declared order, so row order is no change.
So a draft held from before the order was declared does not look dirty.
The anchors are compared by content, and the rest of the row as a record.

## `LDraftClerkEquality.LSpeechMatch`

Parts of speech compare in the same order.
A language-pack value and a custom name of the same wording are two different parts.

## `LDraftClerkList`

The list arithmetic every row kind of a draft shares.
A sentence row, a chip, a media row and a credit are all lists of items addressed by id.
So one set of insert, remove, move and change routines serves all of them, told the id to read.
The concern classes hand these routines the list and the key and add nothing but the item to build.

## `LDraftClerkList.LDraftListAdd`

It places a new item at the position asked for, clamped to the list.

## `LDraftClerkList.LDraftListInsert`

It places an existing item at the position asked for, unless the list already holds its id.
A picked row is one thing however many times it is picked, so the second pick is nothing.

## `LDraftClerkList.LDraftListChange`

The former-item form places the item as an insert does, then drops the former item the field stood for.
A former item the list does not hold is nothing to drop.
The caller supplies the insertion position independently of the former item.
The id form applies one change to the item carrying the id.
It answers null when the list does not hold it.
It answers null rather than refusing.
A shared row is looked for in every card, and most cards will not hold it.

## `LDraftClerkList.LDraftFormerCheck`

An item that is its own former changes nothing, in any list.
`LDraftListChange` and the quill's Author insert both ask it.

## `LDraftClerkList.LDraftListRemove`

It drops the item carrying the id, and refuses when none does.

## `LDraftClerkList.LDraftListMove`

The position is clamped after the removal, so the last place is reachable.

## `LDraftClerkList.LDraftListFind`

It answers the place of the item carrying the id, or minus one.
An id of zero is refused here, so no list routine can be asked about nothing.

## `LDraftClerkList.LDraftRowChange`

It offers every card, nested ones included, one change and keeps the ones that took it.
A shared row is edited this way, since the same id may sit in any number of cards.
No card taking the change means the draft holds no such row, and that is refused.

## `LDraftClerkList.LDraftListNormalize`

It drops every item `blank` marks and passes the rest through `name`, which issues an id to a new one.
The list comes back untouched when nothing was dropped or renamed, so a save can tell by reference.

## `LDraftClerkList.LDraftListParse`

It reads a typed list of chips, split at commas, and answers what the entry keeps.
Each completed part is trimmed, a blank part is skipped, and the rest are handed to `accept` in order.
A part `accept` refuses is kept, so an unresolved word stays for the user to fix.
While the user types, the part after the last comma is not complete yet and stays, its leading space dropped.
Text without a comma is then kept whole, since nothing is complete.
A settled list, as on Enter or on leaving the field, completes every part.
It holds in a markup import as much as in a field, so it is a data rule.

## `LDraftClerkList.LDraftHeldCheck`

The chip clerks and the chip finds ask it, so a typed duplicate and a held suggestion are one rule.

## `LDraftClerkMint`

Normalization names unidentified rows and drops only the blanks each row kind treats as absent.
A held draft is normalised before every save, so the store never sees an unidentified row.

## `LDraftClerkMint.LDraftNormalize`

The same instance comes back when nothing changed, so a caller can tell a no-op by reference.

## `LDraftClerkMint.LReflexNormalize`

Each list is normalised by the same rule, which drops what is blank and gives the rest ids.
Blank unidentified reflex, pronunciation and tag rows are dropped.
Untitled situations and unnamed registers are dropped.
Images and videos are only given ids.

## `LDraftClerkMint.LEtymologyNormalize`

Mentions with no link are dropped and the rest are given ids.

## `LDraftClerkMint.LTranscriptionNormalize`

Only an unidentified blank row without a scheme is dropped, so an identified row survives normalization.

## `LDraftClerkMint.LCardNormalize`

Every card and its children are given ids, their lists normalised in turn.

## `LDraftClerkMint.LSentenceNormalize`

Only an unidentified example with empty text is removed, so a stored example survives an empty draft field.

## `LDraftClerkPanel`

The requests of the drafts that hold no entry: the sentence panel, the source panel and the authors panel.
Each edits the one field of the draft that carries its kind, and refuses a draft of another kind.
The situation panel shares the situation routines with the chips and is not here.

## `LDraftClerkPanel.LPanelApply`

It routes every sentence, source and credit request to its handler, and answers null for any other request.
The clerk then hands that request on to the Mentions.
The example and source panels edit their own field of the draft, and the credits edit its author list.
The authors panel renames the one Author its draft holds.

## `LDraftClerkPanel.LExampleChange`

It applies one change to the sentence the draft holds, and refuses a draft holding none.
`LReferenceChange` does the same for the source.

## `LDraftClerkPanel.LAuthorNameCheck`

A blank text credits nobody, so the field it was typed into keeps its former name.

## `LDraftClerkPanel.LAuthorAdd`

A name the draft already credits, or the store already holds, is that Author rather than a second one.
Only a name nobody has is minted new, so two Sources by one person credit one Author.
The credit the field stood for gives way to the new one, and stays when the name resolves to itself.
The name is compared folded, as the catalog compares it, because case is not a different person.
An Author already credited is left where it is, as picking one is.
A blank name is an argument error, since the form never offers one.

## `LDraftClerkPanel.LAuthorMatch`

It finds the Author in the list whose folded name is the folded name given, or none.

## `LDraftClerkPanel.LAuthorInsert`

It credits the stored Author named, unless already credited.
An id naming no stored Author is refused.
The credit the field stood for gives way to the picked one, and stays when the pick is itself.

## `LDraftClerkPanel.LAuthorChange`

It writes the typed name over the Author an author draft holds.
A draft holding no Author is refused, because the request names work of another kind.

## `LDraftClerkPanel.LAuthorApply`

It rewrites the credit list through the routine given, on a draft that holds a source.
Crediting somebody marks the authorship known, because a source with a credit cannot be one whose authors are unknown.
Removing or moving a credit leaves that mark alone.
That is what the form did before the list was held here.

## `LSpeechClerk`

The rules of an entry's parts of speech as the user builds the list.
The committed parts are chips, and the text still being typed is one pending part.
Names match without regard to case, so one part of speech never shows twice.

## `LSpeechClerk.LSpeechParse`

It appends the trimmed typed text to the committed parts as a custom part.
A blank text or a name already held adds nothing.

## `LSpeechClerk.LSpeechPendingRead`

It answers the trimmed typed text when it appends a part to the chips, else null.

## `LSpeechClerk.LSpeechChipRead`

It gives the draft's parts as chips, cleaned.
The last part is left out when it is the pending custom one.

## `LSpeechClerk.LSpeechAdd`

A blank name answers null.
A name already held answers the list unchanged, so the caller still sends it once.
`declare` answers the catalog value, and a refused declaration keeps the name as a custom part.

## `LSpeechClerk.LSpeechFind`

It finds the catalog's parts whose name holds the trimmed typed text, each marked when the chips already hold it.
A blank text matches every part.
The offer shows only while the text is not blank and some part matches.

## `LSpeechClerk.LSpeechRemove`

It drops the one part shown under this exact name.

## `LSpeechClerk.LSpeechNormalize`

It gives the draft's parts as chips, trimmed, never blank, and each name once.

## `LSpeechClerk.LSpeechSettle`

The held-and-typed pair is kept while the draft shows exactly what they build.
Otherwise the draft changed from elsewhere, so its parts become the committed ones and the typed text clears.

## Stamp

Commit: `c2f335c331fe`
- `LDraftClerk.cs` `edc00d06da99f22c`
- `LDraftClerkEntry.cs` `162ea5dbe85d7507`
- `LDraftClerkEquality.cs` `f7ecfcf9bc4abcb4`
- `LDraftClerkList.cs` `aa0f8b2734e80cb2`
- `LDraftClerkMint.cs` `d7a979c75a1bf87e`
- `LDraftClerkPanel.cs` `184b5dd53dfe60c0`
- `LSpeechClerk.cs` `1215322af9ec046e`
