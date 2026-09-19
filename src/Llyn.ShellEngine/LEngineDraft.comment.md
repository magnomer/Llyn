# LEngineDraft.cs

## `public sealed partial class LEngine`

The one write path for an input-form draft.
The whole form goes in as a single value.
It comes out as the rows one entry is made of.
It is the inverse of `LEntryLoader`.
So it lives on its own rather than among the engine's lookup and workspace calls.
Everything it writes shares one session, so an entry is written whole or not at all.

## `internal LEntry LEngineEntrySave(LEntryDraft draft)`

Saves the whole input form as one new entry.
That is the headword row and its meanings and collocations in card order.
It is also its note and pronunciation when they carry text.
The note is stored as Markdown normalized by `LMarkdown`.
It is also the revision recording the create.
The workspace row is moved onto that revision.
Returns the stored entry with its assigned id and timestamps.
A background frequency fill starts once the commit is through, which `LEngineFrequency.cs` owns.

The draft is normalized before anything is written, so an item still carrying id zero is named here.
Positive means a stored row and negative means an id the engine minted, so zero is never a saved state.
Only the engine mints, and normalizing here means no caller can slip a zero past it.

A blank headword is refused before any connection opens, so a save that cannot be made costs nothing.
The headword is stored trimmed, since padding typed around a word is never part of it.
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

The entry's forms and inflections are written with the entry, in the order the draft holds them.
A part of speech is written as the language-pack value the draft names, or as the text it carries.
The pronunciation is written whole: its reading, syllables and recording.
An entry recording no pronunciation writes no row, so an empty block is never stored.

A new card's rows, chips and links are written by `LEngineCardSync` in `LEngineCard.cs`.
A card just created references nothing yet, so the reconcile that updates a stored card attaches a new one whole.
One write path for both keeps the two from drifting apart.
The Example a sentence row names is resolved in `LEngineDraftSentence.cs`.

## Inline notes

### `throw new LRefusal(LRefusal.LRefusalHeadword);`

The refusal names its reason with a key.
The shell turns that into localized text, so no user-facing wording lives in the engine.

### `LEntry entry = _lEngineEntries.LEntryCreate(`

The entry's language is the language-pack name, which is the language speech_value rows are already written under.

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

### `LEnginePronunciationSync(`

The pronunciations, transcriptions and reflexes of a new entry are written by the same reconciliation an update runs.
On a fresh entry nothing is stored yet, so every row the draft carries is created.
A positive id the draft still holds names a row of an entry since deleted.
It is reset to zero first.
Otherwise the commit would refuse the row instead of writing it anew.

### `private static IReadOnlyList<LPronunciationDraft> LEnginePronunciationReset(`

Every pronunciation row of the draft as a row still to be created.
A negative id is kept, because the identity map records what it became.

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

### `private static long LEngineSituationResolve(`

The stored Situation a chip's positive id names, updated to what the chip now says.
A positive id nothing is stored under is refused rather than rebound.
A chip carrying a negative id is looked up by its title first.
A wording the workspace already holds is shared rather than doubled.
Only then is a fresh Situation made, recorded in the map under the negative id it replaces.

### `private static bool LEngineImageCheck(IReadOnlyList<LImageDraft> rows)`

Whether the card's Images carry any location worth a row.
It is on the same terms LEngineImageRead writes them.
A row naming nothing counts as typed-in nothing.

### `internal static IEnumerable<LImageDraft> LEngineImageRead(IReadOnlyList<LImageDraft> rows)`

The Images of one card that are worth a row.
Rows naming no picture are dropped here rather than written.
So a list the shell built out of a control cannot leave an empty row behind.
The positions stay a gapless 0, 1, 2 over what is actually stored.

### `private static long LEngineImageResolve(`

The stored Image a row's id names, updated to the location the row now says.
A positive id nothing is stored under is refused rather than rebound to a fresh row.
A row carrying a negative id gets a fresh Image instead.
The fresh row is recorded in the map under the negative id it replaces.
Two cards naming one id therefore reference one row, which is what sharing a picture means.
An imported card names the row the catalog created, so a file that declared one picture stores one.

### `private static long LEngineVideoResolve(`

The same rule for a Video, whose location and span are both updated when either changed.
