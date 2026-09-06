# TSentence.cs

## `public sealed class TSentence`

Covers the association a Meaning or a Collocation holds over an Example.
That association carries data, so what is tested is the frame surviving a write and a read.
It covers the order the owner gives its rows and the renumbering a detach forces.
It covers what deleting an owner and deleting a cited Example each reach.

## `public void SentenceSave_Frame_ReadsItBack()`

The whole row round-trips, because a frame the store drops is a frame the editor offered for nothing.
The stored id is not empty, since a row handed in without one is only identity once it is written.

## `public void SentenceSave_FrameWithNoExample_ReadsItBack()`

A frame belongs to the owner, so it is storable before any sentence is.
Requiring an Example first would lose what the user wrote for no reason the data model gives.

## `public void SentenceFrameRead_SavedRows_OffersWhatTheLanguageHolds()`

Nothing ships a marker or a role, so what a store has already been given is the only list the editor can offer.
An untouched store offers nothing, which is what an empty dropdown on a fresh workspace means.
Each language is asked separately, because a Japanese particle is no offer to make on an English row.
The rows here cite no Example, so a frame saved before any sentence was written still reaches the field that offers it.

## `public void SentenceSave_SharedExample_KeepsEachMeaningsFrame()`

Two Meanings citing one Example must read it under their own frame.
A frame living on the Example instead would make the second write overwrite the first.

## `public void SentenceSave_CollocationFrame_ReadsItBack()`

A Collocation holds an Example on the same terms a Meaning does.
The editor already draws the frame fields on a Collocation card, so the store has to keep what they say.

## `public void SentenceSave_SharedExample_KeepsBothOwnersFrames()`

One Example cited by a Meaning and a Collocation carries a different frame on each.
The two owners keep separate tables, so neither write can reach the other.

## `public void CollocationDelete_HeldSentences_LeavesNoRows()`

Deleting the owner takes its holds and leaves the Example, which is owned by nothing.

## Inline notes

### `private static string TSentenceCollocationCreate(TWorkspace workspace, string expression = "in a word")`

One entry with one Collocation on it, holding nothing yet.
