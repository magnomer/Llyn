# LDatabaseLink.cs
Hash: `6213269c5836f43a`

## `public sealed class LDatabaseLink`

One ordered association table, seen from the store that owns its member rows.
The table links an owner row to a member row and keeps the member's position within that owner.
Every such table in the schema differs only in its name and its two key columns.
So one link type serves each of them, named by the store that builds it.
The table and column names are store-owned literals, never caller input.
So composing them into the statement text opens no injection seam.
Every value still travels as a parameter.

An owner's order is a unique index.
So attaching and detaching renumber that owner's whole set through `LDatabaseOrder`.
A caller names the index it wants.
It never has to find a free position or leave a gap behind.

Every method starts its own session.
Inside a caller's open session it nests, so a delete and its guard stay one transaction.

## `public LDatabaseLink(LDatabase database, string table, string owner, string member)`

Binds the link to the workspace `database`, the association `table`, and its `owner` and `member` columns.

## `public void LDatabaseLinkAttach(long ownerId, long memberId, int position)`

References the member from the owner at `position` in that owner's order.
The row goes in beyond the end of the set and the whole set is then renumbered around it.
So the requested index is honoured.
An occupied position is no longer a unique-index failure.
A member the owner already references keeps its single link and moves to `position`.

## `public void LDatabaseLinkDetach(long ownerId, long memberId)`

Removes the owner's reference to the member and closes the gap it leaves.
The member row and its other references survive.

## `public void LDatabaseLinkDelete(long memberId)`

Drops every reference to the member from this table and renumbers what each owner has left.
The owners are read before the delete because afterwards there is nothing left to name them.
A gap in an owner's positions is a unique-index failure waiting for its next attach.
