# LStateColumn.cs

## `public static class LStateColumn`

Reads and writes a value that knows what is known about it.
It uses the two columns the schema keeps for it.
The state is always written.
The text is written only when the state says the value states one.
Every table storing such a value uses the same pair and the same wording for the state.
So the pair is read and written in one place rather than spelled out per store.

The schema guards the pair with a `CHECK` that no column holds text unless its state is `specified`.
So a row can never claim a value is unknown while carrying its text.

## `public static string LStateColumnFormat(LState state)`

The stored wording for `state`.

## `public static LState LStateColumnParse(string state)`

The state a stored wording names.
Callers check the wording with `LStateColumnCheck` first, since anything else has no state.

## `public static bool LStateColumnCheck(string state)`

Whether `state` is one of the three wordings the store writes.
Any other wording means the row was damaged, and the reading side reports it as unreadable.

## `public static void LStateColumnApply(SqliteCommand command, string field, LStateValue value)`

Carries a whole value into `command`.
Refuses an unreadable value, since a diagnosis is never a value to store.
The state goes into `$<field>State` and the text into `$<field>`.
The text is `NULL` for every value that states none.

## `public static LStateValue LStateColumnRead(SqliteDataReader reader, int state)`

Reads the value whose state stands at column `state` and whose text stands directly after it.
That is the order every `SELECT` over such a pair lists them in.
A state wording the store does not know yields an unreadable value carrying whatever text or wording stood there.
