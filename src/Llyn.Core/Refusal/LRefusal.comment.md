# LRefusal.cs

## `public sealed class LRefusal : Exception`

A deliberate refusal the shell is meant to present.
It is a request the logic declines for a reason the user can act on.
A save without a headword is one such reason.

The refusal names its reason with a key, never with a sentence.
Display text belongs to the localization catalog the shell owns.
So the logic layers stay free of user-facing wording.
A refusal reads in whichever interface language is selected.
Only deliberate refusals carry a key.
An unexpected failure stays an ordinary exception and travels with its own message.

## `public const string LRefusalHeadword = "Refusal.HeadwordMissing";`

Reason key for a save whose entry carries no headword.

## `public const string LRefusalEntry = "Refusal.EntryMissing";`

Reason key for an update whose entry is not in the workspace.

## `public const string LRefusalTarget = "Refusal.TargetMissing";`

Reason key for a translation pick naming no stored Entry.
The text may have resolved to nothing.

## `public const string LRefusalExample = "Refusal.ExampleMissing";`

Reason key for a sentence draft opened on an example that is not in the workspace.

## `public const string LRefusalSituation = "Refusal.SituationMissing";`

Reason key for a situation draft opened on a Situation that is not in the workspace.

## `public const string LRefusalReference = "Refusal.ReferenceMissing";`

Reason key for a source draft opened on a Reference that is not in the workspace.

## `public const string LRefusalDraft = "Refusal.DraftMissing";`

Reason key for tentative work whose held file is no longer in the drafts folder.
Another window may have stored or discarded it already.

## `public const string LRefusalCollocation = "Refusal.CollocationNested";`

A Collocation was handed a card to hold inside it.
Only a Meaning nests, because only a sense names a parent in the store.
The format cannot write such a document either, so the save refuses rather than dropping the card.

## `public const string LRefusalCard = "Refusal.CardMissing";`

Reason key for a request naming a card, or a parent card, the held draft does not carry.
An id of zero is one such card, because nothing in a draft is ever addressed by zero.

## `public const string LRefusalStale = "Refusal.DraftStale";`

Reason key for tentative work whose id was raised in a workspace no longer open.
The id names nothing here, and a write under it would land in the wrong place.

## `public const string LRefusalItem = "Refusal.ItemMissing";`

Reason key for a request naming a list item the draft does not hold, or naming none.
A sentence, chip, media row or credit is an item.
A card has its own key, because the form treats a card and a row inside it differently.

## `public const string LRefusalLink = "Refusal.LinkMissing";`

Reason key for a commit that names a stored row no longer in the workspace.
A chip, sentence, media row, credit or inflection value holds a positive id, and that id answers nothing.
The commit refuses rather than making a fresh row from the text the draft still shows.
A silent rebind would bind the card to a row the user never chose.

## `public const string LRefusalScheme = "Refusal.SchemeDoubled";`

Reason key for a transcription request naming a scheme another row of the same entry already carries.
One scheme spells one reading one way, so a second row under that name would say nothing new.

## `public const string LRefusalUnreadable = "Refusal.ValueUnreadable";`

A draft still carries a value the store could not read, and saving would write a diagnosis as data.
The user clears the field first, and the unreadable data is lost on purpose.

## `public LRefusal(string reason)`

Refuses a request for `reason`, a localization key.

## `public string LRefusalReason { get; }`

The localization key naming why the request was refused.
