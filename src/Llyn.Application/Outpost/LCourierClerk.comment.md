# LCourierClerk.cs
Hash: `0bc6c226f4e1a932`

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

The title of the top notebook that holds every Llyn notebook.
Each language gets a notebook under it, titled with the language's name.
An entry without a language stays in the top notebook itself.

## `private const string LCourierSystem = "System";`

The title of the notebook under the top one that holds the style note.
So the CSS never sits among the entries of any language.

## `private const string LCourierStyle = "Llyn style";`

The title of the note whose CSS every entry note imports.

## `private const string LCourierTagPrefix = "llyn/";`

The prefix on every tag Llyn attaches, the language tag included.
Llyn's tags can then never coincide with the user's own tags.

## `internal const string LCourierPhonology = "phonology";`

The kind in the id of a language's sound note, whose key is always empty.
`LCourierLanguage` reads it too, so the link map and the sent note never name it differently.

## `internal const int LCourierStall = 3;`

How many notes in a row may fail on an unreachable Joplin before the push stops.
Three rules out one unlucky timeout, yet spares the user waiting through every remaining note.
It is internal so `LCourierLanguage` stops its reconstruction notes on the same count.

## `private const int LCourierPatience = 120;`

How many times a connect polls for the user's decision, one interval apart.
Two minutes leaves time to find Joplin's prompt without leaving a forgotten request polling forever.

## `private static readonly TimeSpan LCourierInterval = TimeSpan.FromSeconds(1);`

The wait before each poll, so a connect asks Joplin about once a second.

## `private static int _lCourierClerkBusy;`

One while a push or attach runs anywhere in the process, set by compare-and-swap so two callers never both start.

## `public LCourierClerk(LRig rig, LEntryQueryClerk entry, object gate, Func<LSettings> settings, Action<Exception> fault)`

Reads the Joplin, livery, manifest, warrant, workspace and language ports out of `rig`.
`entry` lists every stored entry a push sends, under `gate`.
`settings` is read at each push, so a new port or token applies without a restart.
`fault` receives every caught failure before it is turned into a refusal or skipped, which the engine records.
So a failure leaves a trace beyond the receipt.

## `public async Task<LReceipt> LCourierClerkSend(Func<long, LLiveryPage?> page, Func<string, LLiveryLanguage> language, Func<string, string> lookup, CancellationToken cancellation)`

`page` reads one entry's stored page, null when the entry is gone, which fails that entry.
`language` reads one language's series, rime-table categories and sound rows for its reconstruction notes.
`lookup` maps a localization key to text for the note bodies.
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

## `public bool LCourierClerkCheck()`

Whether a token is stored, so the shell shows Llyn as connected.
It only reads the settings and never asks Joplin, so a status bar may ask on every bulletin.
A stored token Joplin no longer honours still reads true until a push meets the refusal.

## `public static bool LCourierWarrantCheck(Exception exception)`

Whether `exception` is the refusal of a token Joplin rejected or that cannot be restored.
The engine drops the stored token on it, so the shell offers Connect again.
Only the exception itself is read, since every courier refusal is thrown bare.

## `public static Func<long, string> LCourierNoteBuild(LLivery livery, string stamp, IReadOnlyList<LEntry> entries)`

The `note` map `LCourierBatchSend` hands to every entry send and to the livery.
An entry id in `entries` maps to `LLivery.LLiveryIdFormat` over the realm `stamp` and the id.
Any other id maps to empty, so the livery keeps that reference as plain text.
So a link never points at a note this push does not write.

## `public static Func<string, string, string, string> LCourierLinkBuild(LLivery livery, string stamp, IReadOnlyList<LLiveryLanguage> languages)`

The `link` map the livery uses to turn rime-card chips into links to reconstruction notes.
It maps a language, a kind and a key to `LLivery.LLiveryIdFormat` over the kind, realm `stamp`, language and key.
It holds every series and category of `languages` with a non-empty key.
A language with sound rows also holds its sound note under `LCourierPhonology` and an empty key.
Anything else maps to empty.
`LCourierLanguage.LCourierLanguageSend` takes its note ids from this map alone, so the two never drift.
`LCourierBatchSend` filters it afterwards by the notes Joplin answered, so a chip never links to a missing note.

## `private async Task<LReceipt> LCourierBatchSend(Func<long, LLiveryPage?> page, Func<string, LLiveryLanguage> language, Func<string, string> lookup, CancellationToken cancellation)`

Pushes the notebooks, the style note, the reconstruction notes and every entry, then trashes gone notes.
It saves the top notebook, the system notebook and one notebook per language among the entries.
Each notebook id comes from `LLivery.LLiveryIdFormat`, a language's over its trimmed name.
So a notebook keeps its id across pushes, and a renamed one gets its title back.
A language with no entry gets no notebook, so packs never leave empty notebooks.
Each entry lands in its language's notebook, or in the top notebook without a language.
A note left in the top notebook by an older push moves when it is next sent.
An empty stored token refuses with `LRefusalMissing`, since Llyn was never connected.
A token that is present but cannot be restored refuses with `LRefusalWarrant`.
Both refuse at once, so the user connects before anything is sent.
A Joplin that answers on no port refuses too, since every call would fail the same way.
A refusal or a cancellation stops the push, because every later note would meet the same cause.
A failure while saving the notebook or the style note stops the whole push.
A `TimeoutException` there goes to `fault`, then refuses with `LRefusalOutpost`.
So every cause of an unreachable Joplin reads the same to the user.
Every language holding entries is read through `language` before the first note is sent.
Each language is read inside its own guard, so one failed read costs only that language.
Its failure goes to `fault`, and it gets no reconstruction notes and no chip links.
Its entries still go, as plain text where a chip would link.
Any failed read skips the trash sweep for this push, as a foreign manifest does.
A push that could not read everything never drives trashing.
So a transient database fault never wipes that language's notes from Joplin.
The manifest keeps their digests, so the next whole push sweeps what is truly gone.
`LCourierLanguage.LCourierLanguageSend` then sends each language's reconstruction notes before the entries.
Their ids come from the `LCourierLinkBuild` map, and each note Joplin answers, sent or kept, joins a set.
The `link` map handed to the entries holds only ids in that set.
So a failed reconstruction note leaves its chips plain rather than linking to nothing.
Its notes join the current set, so a series or category gone from the read is trashed.
A failed reconstruction note joins the failed list by title, and its timeouts share the entries' row.
A reconstruction note counts as saved or kept exactly as an entry does.
Each entry's note id comes from one `note` map that `LCourierNoteBuild` builds over the realm stamp and the entries.
The same map goes to the livery, so a body can name another entry's note.
Only a failure inside one entry costs just that entry.
That entry's fault goes to `fault`, and its headword joins the receipt's failed list.
Three entries in a row failing with `TimeoutException` refuse with `LRefusalOutpost`, as Joplin stopped answering.
Any other outcome breaks the row, so scattered timeouts never stop a healthy push.
A manifest whose realm is set and differs from this workspace's realm is treated as empty.
Nothing from it is trashed, and it is replaced on save.
A rebuilt database or a foreign file must never drive trashing.
A manifest with an empty realm was written before realms existed, so it is adopted and stamped.
Every manifest id outside the current set is trashed afterwards, unless a language read failed.
The current set holds the style note, every entry note and every reconstruction note tried.
A note that failed this time stays in the set, so it is never trashed for one failure.
Each trash passes every Llyn notebook id and the livery's mark, so only proven Llyn notes go.
Those ids cover every language pack too, so a note whose language lost all its entries still goes.
Each scanned language adds its notebook ids from `LCourierLanguage.LCourierLanguageFolder` as well.
So a reconstruction note is proven Llyn's even when its notebook was never saved this time.
A note in the notebook of a language neither scanned nor still used is left alone.
A trash that answers leaves the manifest whether or not it trashed.
It counts as removed only when the outpost answers true.
A failed trash goes to `fault` and keeps its id, so the next push tries again.
Three trashes in a row failing with `TimeoutException` refuse with `LRefusalOutpost`.
Any other outcome breaks that row, as in the entry loop.
The manifest is saved even on a stop, so the next push skips what already arrived.
The style note counts in no tally, since it is Llyn's own plumbing rather than content the user wrote.

## `private async Task<bool> LCourierEntrySend(int port, string token, string id, string folder, string style, LEntry entry, Func<long, LLiveryPage?> page, Func<long, string> note, Func<string, string, string, string> link, Func<string, string> lookup, Dictionary<string, string> digests, CancellationToken cancellation)`

Reads the entry's page through `page`, then renders and sends its note.
The page reader takes the engine's gate itself, so this clerk takes no lock here.
The page is read without fetching, so a push over every entry starts no network fetch.
An entry gone since the listing throws, so it counts as failed rather than silently kept.
The tags come from the page's draft.
`link` goes to the livery unchanged, so the rime card's chips point at this push's reconstruction notes.

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

