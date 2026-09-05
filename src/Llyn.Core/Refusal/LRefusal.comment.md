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

Reason key for a relation or synonym whose target is not one lexical row of this workspace.
The text may have resolved to nothing.
Or the request may name both an Entry and a Meaning, or neither.

## `public const string LRefusalDraft = "Refusal.DraftMissing";`

Reason key for tentative work whose held file is no longer in the drafts folder.
Another window may have stored or discarded it already.

## `public LRefusal(string reason)`

Refuses a request for `reason`, a localization key.

## `public string LRefusalReason { get; }`

The localization key naming why the request was refused.
