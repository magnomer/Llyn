# LEngineDraft.cs

## `public sealed partial class LEngine`

The one write path for an input-form draft.
The whole form goes in as a single value.
It comes out as the rows one entry is made of.
It is the inverse of `LEntryLoader`.
So it lives on its own rather than among the engine's lookup and workspace calls.
Everything it writes shares one session, so an entry is written whole or not at all.

## `public LEntry LEngineEntrySave(LEntryDraft draft)`

Saves the whole input form as one new entry.
That is the headword row and its meanings and collocations in card order.
It is also its note and pronunciation when they carry text.
It is also the revision recording the create.
The workspace row is moved onto that revision.
Returns the stored entry with its assigned id and timestamps.

A blank headword is refused before any connection opens, so a save that cannot be made costs nothing.
Everything after that runs inside one session, which makes a half-written entry impossible.
A failure at any write rolls back every write before it, leaving no entry row behind.
That is also why this is a single engine call.
The shell has no way to compose a partial write out of several of them.

A card with every field blank writes nothing.
The form keeps an empty card on screen to type into.
It refuses to remove a list's last card.
So it always hands over at least one Meaning card and one Collocation card.
Deciding that a blank one is neither belongs here, not in the form.
A headword saved on its own therefore gains no meaning row and no collocation row.
A blank card between two filled ones leaves the two stored at positions 0 and 1.

A recording the downloader saved is written as the pronunciation's audio row.
It is written in the same transaction.
Its path is made relative to the workspace, so a moved workspace keeps its audio.
Because the row hangs off the pronunciation, a recording with no typed IPA still creates the pronunciation to hang from.

The Example and Situation text a card carries is written as independent data.
Each value in those ordered sets becomes a new row of its own entity.
The card's meaning or collocation then references it at that value's position.
The entry owns none of them, so clearing a card would only detach what it points at.
Tags are not that.
A Tag is its own text, so the card's Tag line is written onto the card itself.
Nothing is left over when it is cleared.

Text is never matched against an existing Example or Situation.
Every non-empty value creates a new row, even when the same words were saved before.
Matching needs a picker that resolves typed text to a chosen row, and none exists yet.
A Tag needs no such picker — the same words are already the same Tag.

The part of speech is written as one owned row of the entry, from the text the field holds.
Text naming a preset the entry's language declares is stored as that preset's stable id.
So renaming the preset renames it wherever it was used.
Text naming no preset is stored as typed, because the field is editable.
A part of speech a language pack has not thought of is still the one the user meant.
An empty field writes no row at all.
**Synonyms are not written**, and no card offers one to type.
An `LSynonym` targets an Entry or a Meaning by id.
A meaning-card synonym is an `LRelation` with the same requirement.
Free text is neither, and no picker exists to resolve it.
So the field was removed from the form rather than left to discard what was typed.
`LCardDraftSynonym` therefore arrives empty from both card kinds.
Writing a link is its own seam, `LEngineRelationCreate` and `LEngineSynonymCreate`.
Each takes a target the caller resolved through `LEngineEntryFind` or `LEngineMeaningFind` first.

## `private void LEngineCardAttach(string ownerId, LCardDraft card, string language, bool collocation)`

Writes the Example and Situation a card typed and points the stored card at them.
Writes its Tag and Translation lines.
Each non-empty Example and Situation field creates a row of its own.
That row is independent data the card references rather than owns.
An empty field writes nothing at all.

A Meaning card and a Collocation card carry the same three fields on the same terms.
So this is the one write path for both.
`collocation` says which of the two owner sides `ownerId` names.
Nothing else about the two differs.

Each field is an ordered set.
Every value the card lists is kept at the position it holds in that list.
So three Tags are three rows at 0, 1 and 2, read back in the order they were typed.
The list arrives as the shell built it — the engine never splits text into rows.
A card that lists nothing for a field attaches nothing.
That detaches the field rather than deleting anything.
The rows a card references are independent data it does not own.
The card's own order is already carried by the row it produced.

## Inline notes

### `throw new LRefusal(LRefusal.LRefusalHeadword);`

The refusal names its reason with a key.
The shell turns that into localized text, so no user-facing wording lives in the engine.

### `LEntry entry = new LEntryArchive(_lEngineDatabase).LEntryCreate(`

The entry's language is the language-pack name, which is the key part_of_speech_value and morphology_value are already written against.

### `speeches: LEngineSpeechResolve(`

The part of speech is written with the entry rather than after it.
It is an owned child row of the entry.
The id it needs is the one LEntryCreate is about to assign.

### `LEngineCardValidate(draft.LEntryDraftMeanings, collocation: false);`

A Collocation carrying a card inside it is refused before anything is written.
Only a Meaning nests, and the format cannot write a nested Collocation either.
Saving it with the children dropped would lose data the caller believes it handed over.

### `private void LEngineMeaningCreate(`

Writes one Meaning and then the Meanings nested under it, parent before child.
A child needs its parent's id, which only exists once the parent row is written.
Each is appended within its own sibling group, so card order becomes stored position.
An empty child is skipped on the same terms an empty card is.

### `if (!string.IsNullOrWhiteSpace(draft.LEntryDraftPronunciation) ||`

The pronunciation row is what a recording hangs from.
So a downloaded recording creates one even when no IPA was typed.
Without it the audio would have nothing to reference.

### `private static IEnumerable<LCardDraft> LEngineCardRead(IReadOnlyList<LCardDraft> cards)`

The cards worth a row.
A card with every field blank is neither a Meaning nor a Collocation.
So it is skipped rather than written.
A card carrying nothing but a Translation is a card, because a link is something the user chose.
It is stored so the link it holds has a row to hang from.
The form always hands over at least one card of each kind.
It seeds one of each and refuses to remove a list's last card.
An editor must keep an empty card to type into.
So this is where an entry with nothing typed stops becoming rows.
Those would be a meaning row with no definition and a collocation row with no expression.
Positions come from the stored sibling count.
So skipping a card in the middle still leaves 0, 1, 2 over the cards that remain.

### `private static bool LEngineSentenceCheck(IReadOnlyList<LSentenceDraft> drafts)`

Whether a card holds any row worth writing.
A row naming no Example and stating no frame records nothing, and the store would refuse it.

### `private static IEnumerable<LSentenceDraft> LEngineSentenceRead(IReadOnlyList<LSentenceDraft> drafts)`

The rows worth writing, in the order the card holds them.
A row stating a frame and no sentence is kept, because the frame is the card's own.
The positions stay a gapless 0, 1, 2 over what is actually stored.

### `private static LExample? LEngineExampleResolve(`

The Example a row names, or `null` when the row names none.
A row whose Example carries neither an id nor a sentence names none.
An Example carrying an id names a stored row, however little its sentence says.

### `private static LExample LEngineExampleResolve(`

The stored Example a row's id names, updated to what the row now says.
A row carrying no id, or one nothing is stored under, gets a fresh Example instead.
Two rows naming one id therefore write one row, which is what sharing a sentence means.
The language falls back to the entry's when the Example states none of its own.

### `private static bool LEngineFieldCheck(IReadOnlyList<string> texts)`

Whether one card field carries any value worth a row.
It is on the same terms LEngineFieldRead writes them.
A field holding only blanks counts as typed-in nothing.

### `private static IEnumerable<string> LEngineFieldRead(IReadOnlyList<string> texts)`

The values of one card field that are worth a row.
Blank entries are dropped here rather than written.
So a list the shell built out of a control cannot leave an empty row behind.
The positions stay a gapless 0, 1, 2 over what is actually stored.
