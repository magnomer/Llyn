# LReferenceKind.cs

## `public enum LReferenceKind`

What kind of material a Reference stands for.
It is a label the user reads, sorts and filters by, and nothing else.
No code branches on it to decide which fields a Reference may carry.
Every Reference holds the same seven fields whatever kind it names.
A kind that decided the shape would rebuild one record per kind, which is what this record exists to avoid.

It carries `LReferenceKindUnspecified` and `LReferenceKindUnknown` itself rather than sitting behind an `LStateValue`.
The set of kinds is closed, so the three states fit inside it as two more members.
That keeps one column in the database where a state and a value would need two.

`LReferenceKindOther` is the home for material none of the named kinds fits.
The note field carries what that material actually was.

## `LReferenceKindUnspecified,`

No kind has been chosen.

## `LReferenceKindUnknown,`

The kind was deliberately marked as not known.

## `LReferenceKindBook,`

A book, whole or in part.

## `LReferenceKindJournal,`

An academic or professional periodical.

## `LReferenceKindArticle,`

A single piece published inside a larger work.

## `LReferenceKindWeb,`

A page or site read on the web.

## `LReferenceKindVideo,`

Moving pictures, whatever carried them.

## `LReferenceKindAudio,`

A recording heard rather than watched.

## `LReferenceKindPicture,`

A still image.

## `LReferenceKindOther,`

Material none of the named kinds fits.
