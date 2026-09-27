# LAnthology.cs

## `public sealed class LAnthology`

The deportment of the corpus panel's example list: the panel state over the example vista and its rows.
The list finds rows and their usage, and takes the query, order and kind filter.
Its panel loads, edits and deletes the chosen Example.
The delete seam asks the removal seam with how many entries cite the chosen Example.
It is sealed, so rows, Mention results, legends and tickets cross as Conduct shapes.
Its constructor and vista restore take engine types, so they stay internal.

## `public bool LAnthologyNarrowed`

Whether the query or the filter hides any row, as the vista answers it.

## `private int LAnthologyUsageRead(long? id)`

How many entries cite the given Example, read fresh from the engine.
Zero when no Example is given.

## `private bool LAnthologyDeleteConfirm()`

Asks the removal seam whether to delete the chosen Example, given its usage.

## `public void LAnthologyCitationSet(string title)`

Points the Example's citation at the Reference the engine resolves the typed title to.
The engine reads the Reference cited now off the held draft, so an unchanged title keeps it.

## `public bool LAnthologyTextCheck(string text, CStateValue value)`

Whether a field showing `text` already shows `value`.
A blank field matches an empty or unknown value, as the engine would store it.
The transcript keeps a matching field untouched, so a bulletin never moves the caret.
The hint is set apart from this check, so an unknown value still reads as unknown.

## `public CMentionResult? LAnthologyMentionFind(long? id, int offset)`

What a click at `offset` in the given Example's text found, read fresh from the engine.
Null when no Example is given, so the excerpt hands the window nothing.
The excerpt passes the chosen Example inline, so no driver local carries it.

## `private IReadOnlyList<CCatalogReference> LAnthologyReferenceFind(string word, LCatalogOrder order)`

The references matching a word in the given order, shared by the whole shelf and the citation search.
The shelf lists by author, and the search lists the most cited first.

## `public IReadOnlyList<CCitationRow> LAnthologyCitationFind(string word, long? source)`

The Sources the citation field offers for the typed word, trimmed first.
An empty word offers none, and `CCitationRow.CCitationRowFind` splits the rest.

## `internal static CCatalogExample LAnthologyRowRead(LCatalogExample row, string unknown, string unwritten)`

Maps one found example to its row shape.
An unknown text reads the unknown wording, and an empty text reads the unwritten wording.

## `internal static CExample? LAnthologyExampleRead(LExample? example)`

Maps a stored Example to its shape, and null to null.
The excerpt's links are kept only while the text reads soundly, as the excerpt showed them before.

## `internal static CMentionResult LAnthologyMentionRead(LMentionResult result)`

Maps what a click on a text found to its shape.
The excerpt and the lectern both hand the window this shape, so the lectern calls it too.
It sits here since `LCard` would become a Large type.

## `private static CMentionMark? LAnthologyMentionRead(LMention? mention)`

Maps the stored Mention under the click, and none to none.
