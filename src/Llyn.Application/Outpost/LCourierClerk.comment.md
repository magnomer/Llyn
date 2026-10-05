# LCourierClerk.cs
Hash: `2d97db1afd6dc715`

## `public sealed class LCourierClerk`

Pushes every entry one way into Joplin, so Llyn's copy always wins over edits made there.
The engine's gate guards vault calls only, so a slow Joplin never blocks the rest of the engine.
Only one push or attach runs at a time in the whole process, even across a workspace switch.
Two would race over one manifest.
Overwriting a note Llyn created is accepted policy, since the push is one way.
Attachments a user pastes into a Llyn note lose their reference when the note is rewritten.
Joplin's own cleanup may later delete such attachments.
Llyn never acts on a Joplin item it did not create.

## `private const string LCourierNotebook = "Llyn";`

The title of the notebook that holds every pushed note.

## `private const string LCourierStyle = "Llyn style";`

The title of the note whose CSS every entry note imports.

## `private const string LCourierTagPrefix = "llyn/";`

The prefix on every tag Llyn attaches, the language tag included.
Llyn's tags can then never coincide with the user's own tags.

## `private const int LCourierStall = 3;`

How many entries in a row may fail on an unreachable Joplin before the push stops.
Three rules out one unlucky timeout, yet spares the user waiting through every remaining entry.

## `private const int LCourierPatience = 120;`

How many times a connect polls for the user's decision, one interval apart.
Two minutes leaves time to find Joplin's prompt without leaving a forgotten request polling forever.

## `private static readonly TimeSpan LCourierInterval = TimeSpan.FromSeconds(1);`

The wait before each poll, so a connect asks Joplin about once a second.

## `public LCourierClerk(LRig rig, LPortraitClerk portrait, LEntryClerk entry, object gate, Func<LSettings> settings, Action<Exception> fault)`

Reads the Joplin, livery, manifest, warrant and workspace ports out of `rig`.
`settings` is read at each push, so a new port or token applies without a restart.
`fault` receives every caught failure before it is turned into a refusal or skipped, which the engine records.
So a failure leaves a trace beyond the receipt.

## `private static int _lCourierClerkBusy;`

One while a push or attach runs anywhere in the process, set by compare-and-swap so two callers never both start.

## `public async Task<LReceipt> LCourierClerkSend(LPortraitLabel label, CancellationToken cancellation)`

Runs one push, refusing with `LRefusalCourier` while another push or connect still runs.
The busy flag clears in a `finally`, so a failed push never blocks the next one.

## `public async Task<string> LCourierClerkAttach(CancellationToken cancellation)`

Asks Joplin for a token and answers it hidden, the form the engine stores in settings.
It shares the push's busy flag, so a token never changes under a running push.
A Joplin that answers on no port refuses with `LRefusalOutpost`, since no request can reach it.
Any other failure to start the request goes to `fault`, then refuses with `LRefusalOutpost`.
Each poll waits first, because the user needs time to see Joplin's prompt.
A poll that throws `TimeoutException` goes to `fault` and counts as still waiting.
Three such polls in a row refuse with `LRefusalOutpost`, since Joplin stopped answering.
Any poll that answers breaks the row.
A rejection refuses with `LRefusalWarrant`, and the user may simply try again.
Running out of polls refuses with `LRefusalPending`, so the request does not hang the shell.

## `private async Task<LReceipt> LCourierBatchSend(LPortraitLabel label, CancellationToken cancellation)`

Pushes the notebook, the style note and every entry, then trashes notes whose entries are gone.
An empty stored token refuses with `LRefusalMissing`, since Llyn was never connected.
A token that is present but cannot be restored refuses with `LRefusalWarrant`.
Both refuse at once, so the user connects before anything is sent.
A Joplin that answers on no port refuses too, since every call would fail the same way.
A refusal or a cancellation stops the push, because every later note would meet the same cause.
A failure while saving the notebook or the style note stops the whole push.
A `TimeoutException` there goes to `fault`, then refuses with `LRefusalOutpost`.
So every cause of an unreachable Joplin reads the same to the user.
Only a failure inside one entry costs just that entry.
That entry's fault goes to `fault`, and its headword joins the receipt's failed list.
Three entries in a row failing with `TimeoutException` refuse with `LRefusalOutpost`, as Joplin stopped answering.
Any other outcome breaks the row, so scattered timeouts never stop a healthy push.
A manifest whose realm is set and differs from this workspace's realm is treated as empty.
Nothing from it is trashed, and it is replaced on save.
A rebuilt database or a foreign file must never drive trashing.
A manifest with an empty realm was written before realms existed, so it is adopted and stamped.
Every manifest id that is neither the style note nor a current entry is trashed afterwards.
Each trash passes the notebook id and the livery's mark, so only proven Llyn notes go.
A trash that answers leaves the manifest whether or not it trashed.
It counts as removed only when the outpost answers true.
A failed trash goes to `fault` and keeps its id, so the next push tries again.
Three trashes in a row failing with `TimeoutException` refuse with `LRefusalOutpost`.
Any other outcome breaks that row, as in the entry loop.
The manifest is saved even on a stop, so the next push skips what already arrived.
The style note counts in no tally, since the receipt speaks of entries.

## `private async Task<bool> LCourierEntrySend(int port, string token, string id, string notebook, string style, LEntry entry, LPortraitLabel label, Dictionary<string, string> digests, CancellationToken cancellation)`

Reads the entry's page and draft under the gate, then renders and sends its note outside it.
The page is read without fetching, so a push over every entry starts no network fetch.
An entry gone since the listing throws, so it counts as failed rather than silently kept.

## `private async Task<bool> LCourierNoteSend(int port, string token, LOutpostNote note, IReadOnlyList<string> tags, IReadOnlyList<LParcel> parcels, Dictionary<string, string> digests, CancellationToken cancellation)`

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

