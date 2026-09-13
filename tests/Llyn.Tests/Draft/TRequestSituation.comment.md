# TRequestSituation.cs

## `public sealed class TRequestSituation`

Covers the media requests a Situation draft takes, the same ten a card takes, with the card id ignored.
The panel sends a card id of 0, because a Situation draft carries no card.

## `public void RequestApply_ImageRemovalOnSituationDraft_DropsTheRow()`

A removal leaves the other picture in place and touches no card, because the draft holds none.

## `public void RequestApply_ImageLocationOnSituationDraft_RewritesTheLocation()`

A location change keeps the row and its id and swaps the path.

## `public void RequestApply_VideoRemovalOnSituationDraft_RefusesUnknownRow()`

A removal naming a row the Situation does not hold is refused as an item, exactly as on a card.

## `public void RequestApply_ImageAdditionOnEntryDraft_StillRoutesToTheCard()`

An Entry draft is untouched by the Situation branch: its picture lands on the card named.
A card id of 0 on an Entry draft is still refused as a card, so the drafts stay apart.
