# Refusal

This folder holds the one exception for a deliberate refusal the shell is meant to present.
A refusal is a request the logic declines for a reason the user can act on.
A save without a headword is one such reason.
It stands apart from ordinary exceptions, which travel with their own message.

## `LRefusal`

The refusal names its reason with a key, never with a sentence.
Display text belongs to the localization catalog the shell owns.
So the logic layers stay free of user-facing wording.
A refusal reads in whichever interface language is selected.
Only deliberate refusals carry a key.
An unexpected failure stays an ordinary exception.

## `LRefusalReason`

The localization key naming why the request was refused.

## `LRefusalIllegible`

Whether the refusal is for a value the store could not read.
Only this refusal lets a commit retry, once the user agrees to drop such values.

## `LRefusalHeadword`

A save whose entry carries no headword.

## `LRefusalEntry`

An update whose entry is not in the workspace.

## `LRefusalTarget`

A translation pick naming no stored Entry.
The text may have resolved to nothing.

## `LRefusalExample`

A sentence draft opened on an example that is not in the workspace.

## `LRefusalSituation`

A situation draft opened on a Situation that is not in the workspace.

## `LRefusalReference`

A source draft opened on a Reference that is not in the workspace.

## `LRefusalDraft`

Tentative work whose held file is no longer in the drafts folder.
Another window may have stored or discarded it already.

## `LRefusalStale`

Tentative work whose id was raised in a workspace no longer open.
The id names nothing here, and a write under it would land in the wrong place.

## `LRefusalCollocation`

A Collocation was handed a card to hold inside it.
Only a Meaning nests, because only a sense names a parent in the store.
The format cannot write such a document either, so the save refuses rather than dropping the card.

## `LRefusalCard`

A request naming a card, or a parent card, the held draft does not carry.
An id of zero is one such card, because nothing in a draft is ever addressed by zero.

## `LRefusalItem`

A request naming a list item the draft does not hold, or naming none.
A sentence, chip, media row or credit is an item.
A card has its own key, because the form treats a card and a row inside it differently.

## `LRefusalLink`

A commit that names a stored row no longer in the workspace.
A chip, sentence, media row, credit or inflection value holds a positive id, and that id answers nothing.
The commit refuses rather than making a fresh row from the text the draft still shows.
A silent rebind would bind the card to a row the user never chose.

## `LRefusalScheme`

A transcription request naming a scheme another row of the same entry already carries.
One scheme spells one reading one way, so a second row under that name would say nothing new.

## `LRefusalUnreadable`

A draft still carries a value the store could not read, and saving would write a diagnosis as data.
The user clears the field first, and the unreadable data is lost on purpose.

## `LRefusalMarkup`

Markup text that is not well-formed XML or whose root is not `llyn`.
Nothing is read from such a file, so the reader refuses rather than guessing at entries.

## `LRefusalName`

An author draft whose name is blank.

## `LRefusalWarrant`

A Joplin call whose token Joplin refused.
The caller asks for a new token instead of failing each note.

## `LRefusalOutpost`

A push or connect that found no Joplin on any port tried, or lost it mid-run.
Joplin may be closed or its Web Clipper service off, and only the user can change either.

## `LRefusalMissing`

A push whose stored token is empty.
Llyn was never connected to Joplin, so the user is told to click Connect first.

## `LRefusalCourier`

A push or connect asked while another push or connect still runs.
Two runs would race over the manifest and the stored token, so the second waits for the user instead.

## `LRefusalPending`

A connect whose access request Joplin left unanswered past the polling limit.
The user may simply not have seen the prompt, so they accept it and try again.

## Stamp

Commit: `c2f335c331fe`
- `LRefusal.cs` `97ac9729718b70d9`
