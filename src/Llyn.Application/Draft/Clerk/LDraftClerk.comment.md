# LDraftClerk.cs
Hash: `edc00d06da99f22c`

## `public sealed class LDraftClerk`

The interactor that applies one `LRequest` to one held draft over the ports of a rig.
A form sends one edit at a time, and the clerk answers with a new draft, never a mutated one.
Each request is applied by a function that returns a new draft or new content.
Cards and the rows inside them are addressed by id wherever they nest, never by place.
So a form and the clerk cannot disagree about which item is meant.
The clerk holds no gate, no observer and no file.
The engine keeps those and calls the clerk under its own gate.
Each request family is applied by the concern class that owns it, beside this one.
Each such class answers null for a request not its own, so the clerk asks them in turn.
The clerk keeps only their construction, the default card and sentence targets, and the order they are asked in.
A clerk is built over one rig and is rebuilt when the engine takes a new rig.
It reads no port after construction that the rig did not hand it.

## `public LDraftClerk(LRig rig, LIdentity identity, LLanguageCache languages)`

Seats the concern classes over the ports of `rig` and the two shared services.
Situation and Register chips each get their own class, `LSituationChip` and `LRegisterChip`, over their own shelf.
`identity` mints the negative id every new row carries.
`languages` answers the pack of a language, cached, for respelling and anatomy.
Neither is a port, so the engine hands them in beside the rig.

## `public LEntryDraft LDraftClerkNormalize(LEntryDraft content)`

The content with every unidentified row given an id, through the mint.
The mint also drops the blank rows of the kinds it treats as nothing.

## `public LDraft LDraftClerkApply(LDraft draft, LRequest request)`

Asks the owners of the kinds that reach past the entry content, in turn.
Situation media go first, then the sentence, source and credit panels, Mentions, Glosses and situation fields.
Everything else is a change to the entry content and falls through to the content chain.

## `public LEntryDraft LDraftClerkApply(LEntryDraft content, LRequest request)`

Asks the entry fields, the cards, the reading, reflex and etymology rows, and then the card lists, in turn.
A kind no owner knows is a programming error, not a refusal, since no form can send one.
A null text or value is read as empty.
So a request can never leave a null where the draft holds text.

## `private LEntryDraft LDraftListApply(LEntryDraft content, LRequest request)`

Settles a request naming card or sentence zero onto a default target first.
Every other kind goes to the sentence, situation, Register, tag, translation, picture and video owners in turn.
A kind none of them knows is a programming error, not a refusal, since no form can send one.

## `private LEntryDraft LCardResolve(LEntryDraft content, Func<long, LRequest> retarget)`

Applies a request that named card zero to the first Meaning card, made when the draft has none.
A browsing panel planting a Tag, Register, Situation, Example or Source into a fresh entry asks this way.
The panel need not read the draft back to learn which card the reset made.

## `private LEntryDraft LSentenceResolve(LEntryDraft content, long cardId, Func<long, LRequest> retarget)`

Applies a request that named sentence zero to the card's first sentence row, made when the card has none.
A card the draft does not hold is refused, as any card request is.
