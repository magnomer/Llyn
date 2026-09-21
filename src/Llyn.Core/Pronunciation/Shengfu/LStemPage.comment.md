# LStemPage.cs

## `public sealed record LStemPage(`

The page of one Stem series as the engine composes it.
It carries the language, the series key and the characters that belong to it.
The blank page stands for no series, so the veneer never branches on null.

**Parameters**

- `LStemPageLanguage` — The language the series belongs to.
- `LStemPageKey` — The series as the source printed it.
- `LStemPageCharacters` — The member characters, already ordered for printing.

## `public bool LStemPageEmpty`

True while the series holds no character to print.
