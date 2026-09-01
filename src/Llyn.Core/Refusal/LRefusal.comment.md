# LRefusal.cs

## `public sealed class LRefusal : Exception`

A deliberate refusal the shell is meant to present: a request the logic declines for a reason the user can act on, such as a save without a headword.

The refusal names its reason with a key, never with a sentence. Display text belongs to the localization catalog the shell owns, so the logic layers stay free of user-facing wording and a refusal reads in whichever interface language is selected. Only deliberate refusals carry a key; an unexpected failure stays an ordinary exception and travels with its own message.

## `public const string LRefusalHeadword = "Refusal.HeadwordMissing";`

Reason key for a save whose entry carries no headword.

## `public const string LRefusalEntry = "Refusal.EntryMissing";`

Reason key for an update whose entry is not in the workspace.

## `public const string LRefusalTarget = "Refusal.TargetMissing";`

Reason key for a relation or synonym whose target is not one lexical row of this workspace — text that resolved to nothing, or a request naming both an Entry and a Meaning, or neither.

## `public LRefusal(string reason)`

Refuses a request for `reason`, a localization key.

## `public string LRefusalReason { get; }`

The localization key naming why the request was refused.
