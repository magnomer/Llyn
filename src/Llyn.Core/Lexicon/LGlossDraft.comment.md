# LGlossDraft.cs

## `public sealed record LGlossDraft(`

One Gloss as a draft carries it.
It holds the same three facts as [LGloss](LGloss.comment.md), under the draft prefix.
The id is negative for a row the database has not stored yet.

**Parameters**

- `LGlossDraftId` — The id of the stored Gloss the row edits, negative for a new row.
- `LGlossDraftLanguage` — The language the rendering is written in, empty when none is chosen.
- `LGlossDraftText` — The rendering and what is known about it.

## `public static LGlossDraft LGlossDraftCreate(LGloss gloss)`

The stored Gloss as a draft, id kept.

## `public LGloss LGlossDraftResolve()`

The draft as a Gloss, id kept, negative for a row the store has not written.

## `public LGlossDraft LGlossDraftNormalize()`

The same draft with an unreadable text dropped to unspecified.

## `public static IReadOnlyList<LGlossDraft> LGlossDraftNormalize(IReadOnlyList<LGlossDraft> glosses)`

Every draft of a list normalized the same way, order kept.
