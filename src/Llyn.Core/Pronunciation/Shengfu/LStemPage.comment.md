# LStemPage.cs
Hash: `25b63dce5b0b522e`

## `public sealed record LStemPage(string LStemPageLanguage, string LStemPageKey, IReadOnlyList<string> LStemPageCharacters, IReadOnlyList<LStemMember>? LStemPageMembers = null)`

The page of one Stem series as the engine composes it.
It carries the language, the series key and the characters that belong to it.
The blank page stands for no series, so the veneer never branches on null.

**Parameters**

- `LStemPageLanguage` — The language the series belongs to.
- `LStemPageKey` — The series as the source printed it.
- `LStemPageCharacters` — The member characters, already ordered for printing.
- `LStemPageMembers` — One `LStemMember` per character, in the same order, or null for bare members.

## `public IReadOnlyList<LStemMember> LStemPageMembers`

The members of the series, never null.
A page built without members gets one bare member per character through `LStemMember.LStemBareScan`.
So every character has its member, and the blank page carries none.

## `public bool LStemPageEmpty`

True while the series holds no character to print.
