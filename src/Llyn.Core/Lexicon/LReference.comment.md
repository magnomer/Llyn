# LReference.cs

## `public sealed record LReference(`

One bibliographic Reference — the material an Entry or an Example cites.
A Reference is independent: no Entry, Example, or Author owns it, and it owns none of them.
Entries reach it through ordered reference rows and an Example through its single source column.
Deleting a citing row never touches the Reference.
Deleting the Reference never touches its citers.

One shape describes an article, a television episode, a video, or a picture alike.
The fields that mean something for the material at hand are specified.
The rest stay `LState.LStateUnspecified`.
`LReferenceKind` names what the material is, and decides nothing about the shape.
No code branches on it to choose which fields a Reference may carry.
A kind that decided the shape would rebuild one record per kind, which is what this record exists to avoid.
Whatever a kind alone cannot say goes in `LReferenceNote`, in the user's own words.
Each field is an `LStateValue` so "never entered", "recorded as unknown", and "this value" stay distinct.
`LReferenceAuthorState` is that same distinction for the authorship as a whole.
The Authors themselves are separate rows attached in order.
So a specified authorship may name one Author, several, or the specified Author `Anonymous`.

**Parameters**

- `LReferenceId` — Opaque, program-generated stable id.
- `LReferenceTitle` — Title of the material, and for a television episode the episode name.
- `LReferenceYear` — Publication or release year.
- `LReferenceKind` — What kind of material it is, as a label and never as a rule.
- `LReferenceNote` — Anything further about the material, written by the user as free text.
- `LReferenceUrl` — Address the material was found at.
- `LReferenceAuthorState` — Whether the authorship is unspecified, unknown, or specified by attached Authors.

## `public string LReferenceNameRead()`

The name the Reference is shown under, which is the first field it actually states.
That is title, then url.
The note is left out, because a memo is not a name.
Every field may stand unspecified or unknown, so none of them is guaranteed.
A Reference that names itself nowhere is shown under its id.
It is never shown as a blank the reader could not tell from the next one.
The rule lives on the record because the catalog, the row and the citation list all need the same answer.

## `public static string LReferenceKindFormat(LReferenceKind kind)`

The word a kind is written as outside the program.
The database column and the markup leaf both store this word rather than a number.
A number would tie the file on disk to the order the members happen to be declared in.

## `public static LStateValue LReferenceKindShow(LReferenceKind kind)`

The kind as a three-state value, so markup can write it the way it writes every other field.
An unspecified kind writes no leaf at all and an unknown one writes an empty leaf.

## `public static LReferenceKind LReferenceKindParse(string? text)`

The kind a stored word names.
A word this build does not know reads as unspecified rather than as a failure.
A workspace written by a later build must still open.

## `public LReference LReferenceNormalize()`

The same Reference with every unreadable value dropped to unspecified.
Called only after the user agreed to lose what the store could not read.
