# Entry

The edit requests for the entry-level parts of a held draft: fields, cards, chips, sources and authors.
They stand apart from the row requests, which edit the rows of the lists inside an entry.
Each kind of edit is its own record.
So the engine applies it with a type switch and no field flags.
The field requests key `LRequestKey` by type and row id, so a deferred edit replaces only its own row.

## `LRequest`

It is one edit the UI asks the engine to make to a held draft.
The UI never assembles a draft.
It sends the draft id, the item, the field and the value, and re-reads what the engine holds.
The engine refuses a request for a draft it does not hold.
The families live beside this file, one file for each.

### `LRequestKey`

It names the field a request writes, so a tenure waiting to flush keeps only the latest request per field.
Every record spells its own key with `nameof`, so no request reads its type at run time.
The plain fields key by type name alone, which suits the entry-level fields, of which there is one each.
A request on one row of a list adds the row's id, so two rows never replace each other.
The structural requests keep the type key and are applied at once rather than deferred.
Two deferred additions would collapse into one, which is why nothing defers them.

## `LRequestEntry.cs`

These are the requests for the fields of the form that are not a card.
Each carries the draft id and the new value, and nothing else.
A text field sends the whole text, not a delta, so the last request to arrive wins outright.

### `LRequestLanguage`

The property is named for its text rather than its meaning, because a member may not share its type's name.

### `LRequestNote`

The engine normalizes the Markdown at commit, so the draft keeps what was typed.

### `LRequestIpa`

It replaces the typed reading of the primary pronunciation, the first of the list.
Its recording and its stored detail are left alone.
When the list is empty the request creates that first row.
A pronunciation with neither reading nor recording is dropped by the engine, not by the request.
A form that edits every row sends the pronunciation request of the row family instead (see `Row.comment.md`).
The engine derives the row's respelling from the new reading again, so a hand-written respelling is replaced.

### `LRequestRespelling`

It replaces the respelling of the primary pronunciation alone, leaving its original reading as it stands.
The form sends it in place of `LRequestIpa` while the respelling switch is on.
When the list is empty the request creates that first row.

### `LRequestAudio`

It replaces the recording of the primary pronunciation and the source it came from.
An empty file clears it.

### `LRequestSpeech`

It replaces the parts of speech as a whole list.
A chip is added or removed whole, and the list is short, so a delta would save nothing.

### `LRequestUnit` and `LRequestFreedom`

`LUnitEmpty` and `LFreedomEmpty` clear the choice.

## `LRequestCard.cs`

Every card request names its card by `LRequestCardId`, real or minted, except the addition, which has no card yet.
Addition, Removal and Shift are the three structural nouns every list uses.

### `LRequestCardAddition`

The `LRequestParentId` is zero for the top of that list, or the meaning card the new card nests under.
The position is zero-based and clamped to the list.
The engine mints the card's id and answers with it in the returned draft and the bulletin.

### `LRequestCardRemoval`

It drops one card, wherever it nests.
A removed meaning also drops every collocation link to it and its descendants.

### `LRequestCardShift`

A card keeps its kind, so a collocation never lands under a meaning.
A zero parent means the top of its own list.

### `LRequestCardExpression`

A meaning card refuses it, since a meaning carries no expression.

### `LRequestCardMeaning`

The meaning template shows it as its Definition.

### `LRequestSenseAddition`

The collocation card is linked to one meaning of the same draft, at the end of its links.
The first link a collocation holds is its home.

### `LRequestSenseRemoval`

It keeps the order of the remaining links.

## `LRequestAuthor.cs`

These are the author requests of the sources panel.
The credits of a source are an ordered list held on the draft, so they take the four structural nouns.
An author is a shared row, so a credit is added by name or picked by id, never edited here.

### `LRequestAuthorAddition`

The engine mints the id, and commit creates the row before attaching it.
The `LRequestFormerId` names the credit the typed field stood for, which the new one replaces.
It is zero for a blank field.
A name that resolves to the former author itself changes nothing.
A name matching a known author reuses that author instead of minting one.

### `LRequestAuthorPick`

An author already credited is left as it is.
The former id works as in the addition.

### `LRequestAuthorState`

It sets whether the authorship is known, unknown, or unrecorded.
Crediting an author marks it known on its own, so this is sent for the other two.

### `LRequestAuthorName`

It renames the author an author draft holds.
This is the one request of the authors panel, which edits an author alone rather than as a credit.

## `LRequestReference.cs`

These are the requests of the sources panel, one per stated field of the source a draft holds.
They name only the draft, because a draft holds one source.
The credited authors have their own family.

### `LRequestReferenceKind`

The kind travels as its own enum rather than as a state value.
So nothing is parsed on the way in.

## `LRequestSituation.cs`

The four structural records name the card and the chip, and the three field records name only the situation.
A situation is one row however many cards link it, so a field edit reaches every card holding that id.
The situation panel sends each field record with the id of the draft's own situation.

### `LRequestSituationAddition`

The engine mints its id, and commit creates the row.

### `LRequestSituationPick`

`Pick` reads as a noun here, the item the user picked, since it is not a registered verb.
The engine copies the stored row into the card under its positive id.
A card already holding that id is left as it is.

### `LRequestSituationRemoval`

It unlinks the situation from the card, and the row itself stays in the store.

### `LRequestSituationTitle`

A title left empty drops the chip from every card at the next normalize.

## `LRequestTag.cs`

The tag requests are shaped like the situation requests.
A tag carries plain text rather than a state value, because a tag is never unknown.
Pick copies the stored row under its positive id, and Text replaces the tag wherever the draft holds it.

## `LRequestImage.cs`

The image requests are shaped like the situation requests.
An image row is added blank and its location typed or chosen afterwards.
So the engine keeps a blank image row in the draft and commit skips it.
Pick links an existing image by copying the stored row under its positive id.
Location replaces the image's location wherever the draft holds it.

## `LRequestVideo.cs`

The video requests are shaped like the image requests with a second field for the span.
Span and Location both replace their field wherever the draft holds the video.

## Stamp

Commit: `c2f335c331fe`
- `LRequest.cs` `b8f283cebcac168b`
- `LRequestAuthor.cs` `7ac4d468b465a9f4`
- `LRequestCard.cs` `7fc40afb2afaa6e8`
- `LRequestEntry.cs` `1f35805b14c35770`
- `LRequestImage.cs` `2e60398607e76ed3`
- `LRequestReference.cs` `c3e01736e791b25c`
- `LRequestSituation.cs` `7a9f784ced047010`
- `LRequestTag.cs` `9faa8a436f5964ce`
- `LRequestVideo.cs` `b406e17e4ce90dfc`
