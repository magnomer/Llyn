# LCourierLanguage.cs
Hash: `30d6bd5fcaad5e62`

## `public sealed class LCourierLanguage`

Sends one language's reconstruction notes for `LCourierClerk.LCourierBatchSend`.
It stands apart so the courier clerk stays within the file size limit.
It holds no state between pushes, so the clerk builds one per push.

## `internal static readonly IReadOnlyList<string> LCourierLanguageFolder = ["xiesheng", "yunjing", "shengmu", "yunmu"];`

The id seeds of the series, rime-table, initial and rime notebooks, in that order.
`LCourierLanguageSend` saves its notebooks from them, and `LCourierClerk.LCourierBatchSend` hands them to the trash sweep.
One list keeps the notebooks Llyn saves and the notebooks it proves as its own from drifting apart.

## `public LCourierLanguage(LOutpost outpost, LLivery livery, Action<Exception> fault)`

Takes the Joplin port, the livery and the clerk's fault sink from the courier clerk.
So its failures leave the same trace as an entry's.

## `public async Task<int> LCourierLanguageSend(int port, string token, string shelf, string style, LLiveryLanguage language, Func<long, string> note, Func<string, string, string, string> link, Func<string, string> lookup, Func<LOutpostNote, IReadOnlyList<LParcel>, Task<bool>> send, ISet<string> current, IList<string> failed, int stalled, CancellationToken cancellation)`

Writes one note per series, initial, rime and tone into the language's notebooks.
Series notes go under `Navigation.Xiesheng`, tone notes under `Navigation.Yunjing`.
Initial and rime notes go under `Yunjing.Shengmu` and `Yunjing.Yunmu` inside the rime-table notebook.
Every notebook title comes through `lookup`.
A tone note is titled through `Display.FanqieTone` over its key.
Every note title is looked up and formatted inside that note's own failure guard.
So a malformed localized string fails only that note.
Series, initial and rime notes are titled with their page key, as the panels name them.
Each notebook id comes from `LLivery.LLiveryIdFormat` over its `LCourierLanguageFolder` seed and the language.
A notebook is saved only when it will hold a note, so a language without books leaves none behind.
The rime-table notebook is saved when it or one of its two inner notebooks holds a note.
A `TimeoutException` while saving a notebook goes to `fault`, then refuses with `LRefusalOutpost`, as the clerk's notebooks do.
Any other failure while saving a notebook stops the whole push, as it does for the clerk's notebooks.
Every note id comes from `link` and nowhere else, so a chip and its note always agree.
The clerk passes the unfiltered id map here, and filters the entries' map by what answered.
A note whose id `link` answers empty is not sent, which drops a blank key.
Each id joins `current` before sending, so a failed note is never trashed.
Every note goes through `send`, the clerk's own note send, so keeping and digests work as for entries.
`send` also counts the note and records its id once Joplin answers.
A failed note goes to `fault`, and its title joins `failed`.
A note whose title itself failed joins `failed` under the language name instead.
`stalled` carries the row of timeouts in and out, so reconstruction notes and entries share one row.
Three in a row refuse with `LRefusalOutpost`, and any other outcome breaks the row.
It answers the row as it stands after the last note.
