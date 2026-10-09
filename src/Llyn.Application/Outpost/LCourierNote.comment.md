# LCourierNote.cs
Hash: `d978ffb8d947f5b3`

## `public sealed class LCourierNote`

Renders and sends one note for `LCourierClerk.LCourierBatchSend`, keeping it when Joplin already holds it.
It stands apart so the push's order and tallies stay in the clerk, and one note's sending lives here.
It holds no state between pushes, since the manifest digests travel in with each call.

## `private const string LCourierTagPrefix = "llyn/";`

The prefix on every tag Llyn attaches, the language tag included.
Llyn's tags can then never coincide with the user's own tags.

## `public LCourierNote(LOutpost outpost, LLivery livery)`

Takes the Joplin port and the livery from the courier clerk.

## `public static Func<long, string> LCourierNoteBuild(LLivery livery, string stamp, IReadOnlyList<LEntry> entries)`

The `note` map `LCourierClerk.LCourierBatchSend` hands to every entry send and to the livery.
An entry id in `entries` maps to `LLivery.LLiveryIdFormat` over the realm `stamp` and the id.
Any other id maps to empty, so the livery keeps that reference as plain text.
So a link never points at a note this push does not write.

## `public async Task<bool> LCourierEntrySend(int port, string token, string id, string folder, string style, LEntry entry, Func<long, LLiveryPage?> page, Func<long, string> note, Func<string, string, string, string> link, Func<string, string> lookup, Dictionary<string, string> digests, CancellationToken cancellation)`

Reads the entry's page through `page`, then renders and sends its note.
The page reader takes the engine's gate itself, so this clerk takes no lock here.
The page is read without fetching, so a push over every entry starts no network fetch.
An entry gone since the listing throws, so it counts as failed rather than silently kept.
The tags come from the page's draft.
`link` goes to the livery unchanged, so the rime card's chips point at this push's reconstruction notes.

## `public async Task<bool> LCourierNoteSend(int port, string token, LOutpostNote note, IReadOnlyList<string> tags, IReadOnlyList<LParcel> parcels, Dictionary<string, string> digests, CancellationToken cancellation)`

Sends one note unless it is kept, answering whether it was sent.
A note is kept only when the manifest holds its digest and Joplin returns exactly its body.
Joplin returns that body only while the note keeps its title.
That check repairs a note edited, renamed, moved or trashed in Joplin, or missing from a fresh profile.
A kept note still gets its tags saved, so tags edited inside Joplin are repaired.
It stays counted as kept.
The digest and every note id come from the livery, which owns the hashing.
Parcels go first, so the note never links to a resource Joplin lacks.
The digest is recorded only after the tags land, so a failed note is retried next time.

## `private static IReadOnlyList<string> LCourierTagRead(LEntryDraft draft)`

Every card tag of the entry, through meanings, collocations and their children, plus its language.
Each one carries `LCourierTagPrefix`, so it is plainly Llyn's.
The set is sorted and blank-free, so the same entry always yields the same tags.
The tag save makes a Llyn note's tag set exact.
So old unprefixed tags on Llyn notes are detached on the next push.
The user's tags themselves stay untouched.
