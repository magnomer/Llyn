# TTranslation.cs

## `public sealed class TTranslation`

Covers the Translation link store, which mirrors the Tag one on an id instead of a text.
A card writes its whole line at once, so every case here is a write followed by a read.

## `public void TranslationMeaningSave_CardWithLinks_ReadsBackInOrder()`

A Translation line is ordered by the card, not by the target headword.
So the order given going in is the order coming back, numbered from zero.

## `public void TranslationMeaningSave_SameEntryTwice_KeepsOneLink()`

A card links an Entry or it does not, so a repeat is one link rather than two.
A blank id names no Entry and is dropped the same way.
The positions left behind are contiguous, because the row that folded away never took one.

## `public void TranslationCollocationSave_ShorterList_Renumbers()`

Editing a card is the same whole write as making it.
So no caller works out which links were added and which were taken away.
The link that was third and is now first says so in its position.

## `public void EntryDelete_LinkTarget_RemovesLinksKeepsCard()`

A link is the target Entry, so a deleted target leaves no link behind.
The card that pointed at it is not a part of that Entry and stays, now carrying one link fewer.

## `public void TranslationTargetRead_FortyThousandIds_ReadsPastTheParameterCap()`

SQLite binds at most 32 766 parameters to one statement.
A card asking for more ids than that must still read in one statement.
So the ids travel as one JSON value.

## `public void TranslationTargetRead_UnknownId_PassesOverIt()`

A card shows headwords, and the rows hold only ids, so the two are joined once for the whole card.
The order asked for is the order returned.
An id no Entry answers is passed over rather than raised, because a caller wants what it can show.

## `public void EntrySave_CardWithLinks_ReadsBackWithLinks()`

A link is part of the card draft, so a save writes it and a load brings it back.
The ids come back in the order the card gave them, not the order the targets were made.
The target sees the same link from its own side, named by the entry that holds the card.

## `public void EntryUpdate_OneLinkRemoved_DropsRowKeepsOther()`

An update writes the card's whole link line rather than a diff.
So a line one shorter leaves one row, and the row that went is the one dropped.
The link that stayed reads back on its own, which says nothing else was rewritten.

## `public void EntrySave_CardWithOnlyALink_StoresCard()`

A blank card is skipped, and a link is not blankness.
So a card whose only content is a link is written and comes back holding it.
Without a row the link would have nothing to hang from and would be lost on save.

## `public void TranslationIncomingRead_EntryPointedAt_FindsBothCards()`

Links are stored one way, so the target finds them by looking back rather than by holding them.
Both card kinds are found, each naming the Entry it belongs to.
An Entry nothing points at finds nothing, which is not an error.
