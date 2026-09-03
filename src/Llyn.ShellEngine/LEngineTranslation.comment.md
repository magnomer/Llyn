# LEngineTranslation.cs

## `public sealed partial class LEngine`

The Translation half of the engine.
A Translation is a link from one card to another Entry, held as that Entry's id.
It crosses languages, so `break` in English links to `부수다` in Korean.
The link is stored one way, from the card that wrote it.
A target finds what points at it by looking back rather than by holding a list.

A card writes its whole link line at once, the way it writes its Tag line.
So no caller works out which links were added and which were taken away.
The shell holds what the card should say, not a diff against what it said before.

Which side an id names arrives as an `LOwner` rather than in the method's name.
A link hangs from a Meaning or a Collocation and from nothing else.
So any other side is refused rather than guessed.
The branch between the two sides is the engine's alone.
The archive is handed a table and never learns which card kind asked.

## `public IReadOnlyList<LTranslation> LEngineTranslationRead(string ownerId, LOwner owner)`

Reads the links the Meaning or Collocation identified by `ownerId` carries.
They arrive in the order that card holds them.

## `public void LEngineTranslationSave(string ownerId, IReadOnlyList<string> ids, LOwner owner)`

Writes that card's whole link line from the Entry ids given.
Blank ids and repeats are dropped, and the surviving order is the order given.

## `public IReadOnlyList<LEntry> LEngineTranslationFind(string query, string? entryId)`

Every Entry whose headword contains `query`, across every language the workspace holds.
A link crosses languages, so the search cannot be narrowed to the card's own one.
An empty query names the whole workspace, which is what a just-opened dropdown shows.
The Entry named by `entryId` is left out, because a card may not translate its own Entry.
A null `entryId` leaves nothing out, which is what an unsaved Entry needs.

## `public LEntry? LEngineTranslationResolve(string word, string? entryId)`

The one Entry whose whole headword is `word`, or nothing when none or several are.
The search behind it matches on containment, so a hit is not proof the user named it.
Resolving asks for the headword itself, so `brea` never silently becomes `breakfast`.
Case is ignored, because a headword is the same word however it was typed.
Several Entries sharing a headword are a question, so this answers nothing and the caller asks.

## `public LEntry LEngineTranslationCreate(string headword, string language)`

Creates the bare Entry a typed word no Entry answers needs, and returns it.
The row carries a headword and a language and nothing else.
The language is the one the caller chose, because a typed word does not name its own.
A stub is a real Entry and is opened and filled in like any other later.
So its making is recorded as a revision, exactly as any other new Entry is.

## `public void LEngineTranslationDelete(string id)`

Drops a stub the editor made and then discarded, so an abandoned edit leaves nothing behind.
An Entry anything still links to is left standing, because it is no longer only a stub.

## `public IReadOnlyList<LTranslationTarget> LEngineTargetRead(IReadOnlyList<string> ids)`

The headword and language behind each link id, in the order asked for.
A card holds ids and shows words, so the two are joined once for the whole card.
An id no Entry answers is passed over rather than raised.

## `public IReadOnlyList<LUsage> LEngineIncomingRead(string entryId)`

Every card of either kind that links to the Entry identified by `entryId`.
Links are stored one way, so an Entry's incoming list is read rather than held.
An Entry nothing points at reads back empty, which is not an error.
