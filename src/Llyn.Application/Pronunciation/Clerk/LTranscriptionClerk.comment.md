# LTranscriptionClerk.cs

## `public sealed class LTranscriptionClerk`

Transcriptions of an entry and the lookups that feed them.
The pronunciation lookup and the per-scheme lookup both run here, over sources built once per language.
The engine hands in a step sink and never sees the receiver the lookup is typed by.

## `public LTranscriptionClerk(LRig rig, LLanguageCache languages)`

Reads the transcription port and the source factory out of `rig`.

## `public IReadOnlyList<string> LSchemeRead(string language)`

The scheme names the pack of `language` declares, or none for a blank language.

## `public Task<IReadOnlyList<LCandidate>> LTranscriptionClerkFind(string word, string language, Action<LLookupStep> sink, CancellationToken cancellation)`

The pronunciation lookup of `word`, with the pack's varieties and cleanups.
A pack with respelling groups gets a receiver that respells each candidate before `sink` sees it.

## `public Task<IReadOnlyList<LCandidate>> LTranscriptionClerkFind(string word, string language, string scheme, Action<LLookupStep> sink, CancellationToken cancellation)`

The lookup of `word` over the sources of one scheme, raw.

## `public Task LTranscriptionClerkPublish(IReadOnlyList<LCandidate> held, string language, Action<LLookupStep> sink)`

Replays remembered pronunciation candidates to `sink`, respelled the way a fresh lookup would be.

## `public static Task LTranscriptionClerkPublish(IReadOnlyList<LCandidate> held, Action<LLookupStep> sink)`

Replays remembered scheme candidates to `sink` and finishes.

## `private static Task LTranscriptionClerkPublish(IReadOnlyList<LCandidate> held, LReceiver receiver)`

Hands every held candidate to `receiver` and finishes the lookup.

## `private static LReceiver LReceiverCreate(LLanguage pack, Action<LLookupStep> sink)`

A relay to `sink`, wrapped in a respelling receiver when `pack` declares respelling groups.

## `public void LTranscriptionClerkSync(long entryId, IReadOnlyList<LTranscriptionDraft> drafts, List<LRevisionChange>? changes, Dictionary<long, long> identity)`

The transcriptions of an entry reconciled to its draft.
An unchanged list writes nothing.
A positive id naming no stored row refuses the commit.
A blank or repeated scheme refuses the commit.
The minted ids are recorded against the draft ids and one change summarises the list.

## `public static IReadOnlyList<LTranscriptionDraft> LTranscriptionClerkReset(IReadOnlyList<LTranscriptionDraft> drafts)`

The drafts with every positive id cleared, for a fresh entry that has no stored rows to match.

## `private IReadOnlyList<LSource> LLookupSourceRead(string language, LLanguage pack)`

The lookup sources of `language`, built from `pack` on first use.

## `private IReadOnlyList<LSource> LSchemeSourceRead(string language, string scheme)`

The sources of one scheme, from the scheme's declaration or the glyph's source for it.

## `private static IReadOnlyList<LTranscription> LTranscriptionRowRead(long entryId, IReadOnlyList<LTranscriptionDraft> drafts)`

The rows a draft list means, trimmed and placed in order.

## `private static bool LTranscriptionClerkMatch(IReadOnlyList<LTranscription> stored, IReadOnlyList<LTranscription> current)`

Whether two row lists say the same thing, id for id.

## `private static string LTranscriptionClerkFormat(IReadOnlyList<LTranscription> transcriptions)`

The rows as one summary line for the revision.
