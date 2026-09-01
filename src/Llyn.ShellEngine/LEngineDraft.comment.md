# LEngineDraft.cs

## `public sealed partial class LEngine`

The one write path for an input-form draft: the whole form goes in as a single value and comes out as the rows one entry is made of. It is the inverse of `LEntryLoader`, so it lives on its own rather than among the engine's lookup and workspace calls, and everything it writes shares one session — an entry is written whole or not at all.

## `public LEntry LEngineEntrySave(LEntryDraft draft)`

Saves the whole input form as one new entry: the headword row, its senses and collocations in card order, its note and pronunciation when they carry text, the revision recording the create, and the workspace row moved onto that revision and that entry. Returns the stored entry with its assigned id and timestamps.

A blank headword is refused before any connection opens, so a save that cannot be made costs nothing. Everything after that runs inside one session, which is what makes a half-written entry impossible: a failure at any write rolls back every write before it, leaving no entry row behind. That is also why this is a single engine call — the shell has no way to compose a partial write out of several of them.

A card with every field blank writes nothing. The form keeps an empty card on screen to type into and refuses to remove a list's last card, so it always hands over at least one Meaning card and one Collocation card; deciding that a blank one is neither belongs here, not in the form. A headword saved on its own therefore gains no sense row and no collocation row, and a blank card between two filled ones leaves the two stored at positions 0 and 1.

A recording the downloader saved is written as the pronunciation's audio row, in the same transaction, with its path made relative to the workspace so a moved workspace keeps its audio. Because the row hangs off the pronunciation, a recording with no typed IPA still creates the pronunciation to hang from.

The Example, Situation and Tag text a card carries is written as independent data: each value in those ordered sets becomes a new row of its own entity, which the card's sense or collocation then references at that value's position. The entry owns none of them, so clearing a card would only detach what it points at.

Text is never matched against an existing Example, Situation or Tag: every non-empty value creates a new row, even when the same words were saved before. Matching needs a picker that resolves typed text to a chosen row, and none exists yet.

The part of speech is written as one owned row of the entry, from the text the field holds. Text naming a preset the entry's language declares is stored as that preset's stable id, so renaming the preset renames it wherever it was used; text naming no preset is stored as typed, because the field is editable and a part of speech a language pack has not thought of is still the one the user meant. An empty field writes no row at all. **Synonyms are not written**, and no card offers one to type. An `LSynonym` targets an Entry or a Meaning by id, and a sense-card synonym is an `LRelation` with the same requirement; free text is neither, and no picker exists to resolve it, so the field was removed from the form rather than left to discard what was typed. `LCardDraftSynonym` therefore arrives empty from both card kinds. Writing a link is its own seam — `LEngineRelationCreate` and `LEngineSynonymCreate`, each taking a target the caller resolved through `LEngineEntryFind` or `LEngineSenseFind` first.

## `private void LEngineCardAttach(string ownerId, LCardDraft card, string language, bool collocation)`

Writes the Example, Situation and Tag a card typed and points the stored card at them. Each non-empty field creates a row of its own — independent data the card references rather than owns — and an empty field writes nothing at all.

A Meaning card and a Collocation card carry the same three fields on the same terms, so this is the one write path for both: `collocation` says which of the two owner sides `ownerId` names, and nothing else about the two differs.

Each field is an ordered set: every value the card lists becomes a row of its own, referenced at the position it holds in that list, so three tags are three rows at 0, 1 and 2 and read back in the order they were typed. The list arrives as the shell built it — the engine never splits text into rows. A card that lists nothing for a field attaches nothing, which detaches the field rather than deleting anything: the rows a card references are independent data it does not own. The card's own order is already carried by the row it produced.

## Inline notes

### `throw new LRefusal(LRefusal.LRefusalHeadword);`

The refusal names its reason with a key; the shell turns that into localized text, so no user-facing wording lives in the engine.

### `LEntry entry = new LEntryArchive(_lEngineDatabase).LEntryCreate(`

The entry's language is the language-pack name, which is the key part_of_speech_value and morphology_value are already written against.

### `speeches: LEngineSpeechResolve(`

The part of speech is written with the entry rather than after it: it is an owned child row of the entry, and the id it needs is the one LEntryCreate is about to assign.

### `LSense sense = senses.LSenseCreate(new LSense(`

Each sense is appended, so card order becomes stored position.

### `if (!string.IsNullOrWhiteSpace(draft.LEntryDraftPronunciation) ||`

The pronunciation row is what a recording hangs from, so a downloaded recording creates one even when no IPA was typed; without it the audio would have nothing to reference.

### `private static IEnumerable<LCardDraft> LEngineCardRead(IReadOnlyList<LCardDraft> cards)`

The cards worth a row: a card with every field blank is neither a Meaning nor a Collocation, so it is skipped rather than written. The form always hands over at least one card of each kind — it seeds one of each and refuses to remove a list's last card, because an editor must keep an empty card to type into — so this is where an entry with nothing typed stops becoming a sense row with no definition and a collocation row with no expression. Positions come from the stored sibling count, so skipping a card in the middle still leaves 0, 1, 2 over the cards that remain.

### `private static bool LEngineFieldCheck(IReadOnlyList<string> texts)`

Whether one card field carries any value worth a row, on the same terms LEngineFieldRead writes them: a field holding only blanks counts as typed-in nothing.

### `private static IEnumerable<string> LEngineFieldRead(IReadOnlyList<string> texts)`

The values of one card field that are worth a row: blank entries are dropped here rather than written, so a list the shell built out of a control cannot leave an empty row behind, and the positions stay a gapless 0, 1, 2 over what is actually stored.
