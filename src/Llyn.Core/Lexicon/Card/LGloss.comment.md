# LGloss.cs

## `public sealed record LGloss(`

One Gloss: an Example's sentence rendered as text in one language.
An Example carries any number of them, in the order the user keeps them.
The list lives on the Example and never on the card that quotes it.
So every card citing one sentence reads the same renderings.
A Gloss links nothing, unlike the Translation a Meaning carries, which is a link to another Entry.
The text carries `LStateValue`, so a row may say the rendering is not known.
The language may stand empty when the user has not chosen one yet.

**Parameters**

- `LGlossId` — Opaque, program-generated stable id, zero for a row the store has not written.
- `LGlossLanguage` — The language the rendering is written in, empty when none is chosen.
- `LGlossText` — The rendering and what is known about it.

## `public LGloss LGlossNormalize()`

The same Gloss with an unreadable text dropped to unspecified.

## `public static IReadOnlyList<LGloss> LGlossNormalize(IReadOnlyList<LGloss> glosses)`

Every Gloss of a list normalized the same way, order kept.
