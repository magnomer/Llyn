# LPhonologyPort.cs

## `public interface LPhonologyPort`

The slice of the engine a deportment sees when it asks about sounds and scripts.
It covers the rime-book rows, the script images, the reflex readings and the inflection paradigm of an entry.
Each of those has a check, a start and a read, since the rows are fetched in the background.
It also answers the language facts a panel words its fields by.
Those are tonal, silent, phonemic, respelled, the tone list, the schemes and the parts of speech.
`LEngine` implements it today, and a phonology clerk takes it over when the parts are dismantled.

## `bool LEngineFanqieCheck(long entryId);`

Whether the entry's rime-book rows are still being fetched.

## `LDiweiPage LEngineDiweiResolve(long? id, Func<string, string?> localize);`

The page of one rime cell with its labels localized through the given lookup.
