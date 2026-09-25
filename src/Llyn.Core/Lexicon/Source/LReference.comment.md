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
`LReferenceAuthorState` is that same distinction for the authorship as a whole, carried as an `LStateMark`.
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
  It also says whether the stored word could be read.

## `public string LReferenceKindKey`

The localization key of the kind held, for a page that shows the kind it was given.

## `public string LReferenceKindTag`

The stored word of the kind held, which a menu row carries as its tag.

## `public string LReferenceTitleHint`

The placeholder key the title field shows while empty.
It is the unknown mark while the title is unknown, else the field's own key.
The year, url and note hints beside it read the same way.

## `private static string LReferenceHintRead(LStateValue value, string key)`

The unknown mark for an unknown value, else the key the field owns.

## `public string LReferenceNameRead()`

The name the Reference is shown under, which is the first field it actually states.
That is title, then url.
The note is left out, because a memo is not a name.
Every field may stand unspecified or unknown, so none of them is guaranteed.
A Reference that names itself nowhere is shown under its id.
It is never shown as a blank the reader could not tell from the next one.
The rule lives on the record because the catalog, the row and the citation list all need the same answer.

## `public string LReferenceBylineRead(IReadOnlyList<LAuthor> credits)`

The line a citation is written as beside a quoted sentence: `Author (Year)`.
Every credited name is listed in order, joined by a comma.
A Reference crediting nobody is written under its name instead, so the line is never blank.
The year follows in parentheses only when it is stated.
The rule lives here because the example line, the citation field and its dropdown all write the same line.

## `public LColophon LReferenceColophonRead(IReadOnlyList<LAuthor> credits, string tally, Func<string, string> localize)`

The read sheet of this Source, composed on the record so the page passes the Source nowhere.
A Source handed as an argument would be an engine answer the page carried into a request.

## `public static string LReferenceKindResolve(LReferenceKind kind)`

The localization key a kind is labelled by, so every place that names a kind reads the same word.
The read sheet, the print legend and the kind filter all resolve it here.

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

## `public static string LReferenceUsageFormat(int count, Func<string, string> localize)`

The sentence for how many places cite a Source, one of three forms by count.
It serves the sources panel, the authors panel and the vita alike, so the wording lives once.
