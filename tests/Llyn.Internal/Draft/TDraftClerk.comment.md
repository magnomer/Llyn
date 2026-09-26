# TDraftClerk.cs

## `public sealed class TDraftClerk`

Covers the draft clerk on its own, over a rig of fakes and no engine.
The engine tests drive the same code through the facade and remain the regression net.

## `public void DraftClerkApply_HeadwordRequest_ChangesHeadword()`

A headword request rewrites the headword of the content and nothing else.
The draft handed in is untouched, since a clerk answers with a new draft.

## `public void DraftClerkApply_TagAddition_MintsIdBelowZero()`

A tag added by text lands first in the card's list under a minted negative id.
The issuer over the fake workspace row is what mints it.

## `public void DraftClerkApply_DuplicateScheme_ThrowsRefusal()`

A second transcription row under a scheme the draft already carries is refused.
The first one stands, so the refusal is the clerk's and not the fake's.

## `public void DraftClerkApply_UnknownRequestKind_ThrowsArgument()`

A request kind no switch knows falls to the default arm, which is an argument error and not a refusal.
