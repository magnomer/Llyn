# LPhonologyPort.cs

## `public interface LPhonologyPort`

The slice of the engine a deportment sees when it asks about sounds and scripts.
It covers the rime-book rows, the script images, the reflex readings and the inflection paradigm of an entry.
Each of those has a check and a read, since the rows are fetched in the background.
One start opens every fetch of an entry, and the reflexes keep a start of their own for the editor.
It also answers the language facts a panel words its fields by.
Those are tonal, silent, phonemic, respelled, the tone list, the schemes and the parts of speech.
`LEngine` implements it today, and a phonology clerk takes it over when the parts are dismantled.

## `bool LEngineFanqieCheck(long entryId);`

Whether the entry's rime-book rows are still being fetched.

## `void LEngineSoundStart(long entryId);`

Starts every background fetch a reading view of the entry shows, in the engine's order.

## `LDiweiPage LEngineDiweiResolve(long? id, Func<string, string?> localize);`

The page of one rime cell with its labels localized through the given lookup.

## `(bool, string, string) LEngineMarkRead(string language);`

Whether readings of `language` show their respelling, and the two brackets around a reading.

## `LAccentSheet LEngineAccentRead(LEntryDraft draft);`

The pronunciation block of `draft`, ready for the reading view.

## `Task<LAccentSheet> LEngineAccentLoad(`

Loads the variety flags the block of a draft draws, then answers the block.
