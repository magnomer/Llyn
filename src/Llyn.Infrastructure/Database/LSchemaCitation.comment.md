# LSchemaCitation.cs

## `public static class LSchemaCitation`

The one Reference every workspace starts with: the Source titled Unknown.
An Example whose source is not known cites this row like any other Source.
So the example table needs no unknown wording of its own, and the citation exchanges like any Reference.
The row is ordinary data.
The user may delete it, and typing its title in a citation field makes it again.

## `public static long LSchemaCitationCreate(SqliteConnection connection, string schema)`

Inserts the Unknown Source into `schema` and returns its id.
Only the title is stated, since nothing else about it is known.
Called once when a workspace is first created, and by migration when older rows need a row that is gone.

## `public static void LSchemaCitationSettle(SqliteConnection connection, string schema)`

Points every Example in `schema` still carrying the old `unknown` source wording at the Unknown Source.
Older workspaces wrote that wording before the row existed.
The reading side treats the wording as unreadable now, so migration rewrites it before any read sees it.
The Unknown Source is looked up by title first, so a workspace already holding one gains no second.
Nothing is touched when no Example carries the wording.
