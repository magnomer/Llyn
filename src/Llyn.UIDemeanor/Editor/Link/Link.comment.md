# Link

Link holds the Translation field of a card in the console.
A Translation names a headword in another language, and that headword may not exist yet.

## `BLink`

This builds the one Translation field and hands out its keys.
A link shows its headword and language in angle brackets.
Its list is Conduct's `CProspect`, which holds the stored entries that match the typed word.
After the stored rows come the fresh rows, one for each language the word could be created in.
A fresh row begins with a plus sign, so it reads as a new entry and not a stored one.
Choosing a stored row links its id, and choosing a fresh row links the word and the language alone.
So the engine decides whether an entry is made, and the console only passes the choice on.
Enter resolves with the list offered, and a close resolves without offering it.

## Stamp

Commit: `f3a687d70b47`
- `BLink.cs` `ea92aefe7b2d1a57`
