# LDatabaseOrder.cs

## `public static class LDatabaseOrder`

Rewrites the `position` column of an ordered row set so it reads `0, 1, 2, …` again.
This happens after an insert, a move, or a removal.
Every ordered table in the schema carries a unique index over `(owner, position)`.
That is what makes a one-row-at-a-time reorder impossible.
Swapping two neighbours collides on the first statement.
So the whole set is rewritten in two passes.
Every position is first shifted clear of the range by a constant.
A constant shift keeps the set unique against itself and clear of the final values.
Each position is then written back as its index in the intended order.

A set is named by a *scope*, a store-owned SQL predicate over `$owner`.
`entry_id = $owner` and `entry_id = $owner AND parent_id IS NULL` are examples.
Scopes and column names are literals the calling store chooses, never caller input.
Every value still travels as a parameter.

## `private const long LDatabaseOrderShift = 1_000_000_000L;`

The shift that shelves the current positions while the final ones are written.

## `public static IReadOnlyList<string> LDatabaseOrderRead(`

Reads the members of an ordered set in their stored order.
That is the `id` column of a stable-id table, or the member column of an association table.
The list an insert or a move rearranges before handing it back to a normalize call.

## `public static void LDatabaseOrderNormalize(`

Rewrites the positions of an ordered set.
The members listed in `identifiers` take positions `0 … n-1` in that order.
Every row in the scope must appear in the list.
`memberColumn` is the column that names a member.
It is `id` for a stable-id table, and the member column for an association table.

## `public static IReadOnlyList<string> LDatabaseOrderMove(`

Moves the member at `from` in `identifiers` to `target`, clamping the target into the list.
Returns the rearranged order for a normalize call.
The order is returned unchanged when the source index is outside the list.

## `public static IReadOnlyList<string> LDatabaseOrderInsert(`

Places `member` at `target` in `identifiers`, clamping the target into the list.
Returns the resulting order for a normalize call.
Used when a row has just been inserted and the whole set must be renumbered around it.
