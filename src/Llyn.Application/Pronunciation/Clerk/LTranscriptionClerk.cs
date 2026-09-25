using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LTranscriptionClerk
{
    private readonly LTranscriptionVault _lTranscriptionClerkTranscriptions;
    private readonly LSourceFactory _lTranscriptionClerkFactory;
    private readonly LLanguageCache _lTranscriptionClerkLanguages;
    private readonly Dictionary<string, IReadOnlyList<LSource>> _lTranscriptionClerkLookups =
        new(StringComparer.Ordinal);
    private readonly Dictionary<(string LTranscriptionLanguage, string LTranscriptionScheme), IReadOnlyList<LSource>>
        _lTranscriptionClerkSchemes = [];

    public LTranscriptionClerk(LRig rig, LLanguageCache languages)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(languages);
        _lTranscriptionClerkTranscriptions = rig.LRigTranscriptions;
        _lTranscriptionClerkFactory = rig.LRigSources;
        _lTranscriptionClerkLanguages = languages;
    }

    public IReadOnlyList<string> LSchemeRead(string language)
    {
        return string.IsNullOrWhiteSpace(language)
            ? []
            : _lTranscriptionClerkLanguages.LLanguageCacheRead(language).LLanguageSchemes
                .Select(static scheme => scheme.LSchemeName)
                .ToList();
    }

    public Task<IReadOnlyList<LCandidate>> LTranscriptionClerkFind(
        string word, string language, Action<LLookupStep> sink, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(sink);

        LLanguage pack = _lTranscriptionClerkLanguages.LLanguageCacheRead(language);
        return new LLookup(LLookupSourceRead(language, pack), pack.LLanguageVarieties, pack.LLanguageCleanups)
            .LSeekerStart(word, LReceiverCreate(pack, sink), cancellation);
    }

    public Task<IReadOnlyList<LCandidate>> LTranscriptionClerkFind(
        string word, string language, string scheme, Action<LLookupStep> sink, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(scheme);
        ArgumentNullException.ThrowIfNull(sink);

        return new LLookup(LSchemeSourceRead(language, scheme), [], [], true)
            .LSeekerStart(word, new LReceiverRelay(sink), cancellation);
    }

    public Task LTranscriptionClerkPublish(IReadOnlyList<LCandidate> held, string language, Action<LLookupStep> sink)
    {
        ArgumentNullException.ThrowIfNull(sink);

        LLanguage pack = _lTranscriptionClerkLanguages.LLanguageCacheRead(language);
        return LTranscriptionClerkPublish(held, LReceiverCreate(pack, sink));
    }

    public static Task LTranscriptionClerkPublish(IReadOnlyList<LCandidate> held, Action<LLookupStep> sink)
    {
        ArgumentNullException.ThrowIfNull(sink);
        return LTranscriptionClerkPublish(held, new LReceiverRelay(sink));
    }

    private static Task LTranscriptionClerkPublish(IReadOnlyList<LCandidate> held, LReceiver receiver)
    {
        ArgumentNullException.ThrowIfNull(held);

        foreach (LCandidate candidate in held)
        {
            receiver.LReceiverCandidateAdd(candidate);
        }

        receiver.LReceiverLookupFinish();
        return Task.CompletedTask;
    }

    private static LReceiver LReceiverCreate(LLanguage pack, Action<LLookupStep> sink)
    {
        LReceiver receiver = new LReceiverRelay(sink);
        return pack.LLanguageRespellings.Count > 0
            ? new LReceiverRespelling(receiver, pack.LLanguageRespellings)
            : receiver;
    }

    public void LTranscriptionClerkSync(
        long entryId,
        IReadOnlyList<LTranscriptionDraft> drafts,
        List<LRevisionChange>? changes,
        Dictionary<long, long> identity)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(identity);

        LTranscriptionVault transcriptions = _lTranscriptionClerkTranscriptions;
        IReadOnlyList<LTranscription> stored = transcriptions.LTranscriptionRead(entryId);
        IReadOnlyList<LTranscriptionDraft> written = LDraftClerkEquality.LTranscriptionScan(drafts);
        IReadOnlyList<LTranscription> current = LTranscriptionRowRead(entryId, written);

        if (LTranscriptionClerkMatch(stored, current))
        {
            return;
        }

        foreach (LTranscription row in current)
        {
            if (row.LTranscriptionId > 0 && !stored.Any(kept => kept.LTranscriptionId == row.LTranscriptionId))
            {
                throw new LRefusal(LRefusal.LRefusalLink);
            }
        }

        IReadOnlyList<LTranscription> saved = transcriptions.LTranscriptionSet(entryId, current);
        for (int index = 0; index < saved.Count; index++)
        {
            LIdentity.LIdentityRecord(identity, written[index].LTranscriptionDraftId, saved[index].LTranscriptionId);
        }

        changes?.Add(new LRevisionChange(
            entryId,
            "transcription",
            current.Count == 0 ? "delete" : stored.Count == 0 ? "create" : "update",
            LTranscriptionClerkFormat(current)));
    }

    public static IReadOnlyList<LTranscriptionDraft> LTranscriptionClerkReset(IReadOnlyList<LTranscriptionDraft> drafts)
    {
        ArgumentNullException.ThrowIfNull(drafts);

        List<LTranscriptionDraft> renewed = new(drafts.Count);
        foreach (LTranscriptionDraft draft in drafts)
        {
            renewed.Add(draft.LTranscriptionDraftId > 0 ? draft with { LTranscriptionDraftId = 0 } : draft);
        }

        return renewed;
    }

    private IReadOnlyList<LSource> LLookupSourceRead(string language, LLanguage pack)
    {
        if (!_lTranscriptionClerkLookups.TryGetValue(language, out IReadOnlyList<LSource>? sources))
        {
            sources = _lTranscriptionClerkFactory.LSourceFactoryCreate(pack.LLanguageLookupSources);
            _lTranscriptionClerkLookups[language] = sources;
        }

        return sources;
    }

    private IReadOnlyList<LSource> LSchemeSourceRead(string language, string scheme)
    {
        if (_lTranscriptionClerkSchemes.TryGetValue((language, scheme), out IReadOnlyList<LSource>? sources))
        {
            return sources;
        }

        LLanguage pack = _lTranscriptionClerkLanguages.LLanguageCacheRead(language);
        LScheme? declared = pack.LLanguageSchemes
            .FirstOrDefault(known => string.Equals(known.LSchemeName, scheme, StringComparison.Ordinal));
        IReadOnlyList<LSourceSpec> specs = declared?.LSchemeSources
            ?? pack.LLanguageGlyph?.LGlyphSourceRead(scheme)
            ?? [];
        sources = _lTranscriptionClerkFactory.LSourceFactoryCreate(specs);
        _lTranscriptionClerkSchemes[(language, scheme)] = sources;
        return sources;
    }

    private static IReadOnlyList<LTranscription> LTranscriptionRowRead(
        long entryId, IReadOnlyList<LTranscriptionDraft> drafts)
    {
        List<LTranscription> rows = [];
        HashSet<string> schemes = new(StringComparer.Ordinal);
        foreach (LTranscriptionDraft draft in drafts)
        {
            string scheme = draft.LTranscriptionDraftScheme.Trim();
            if (scheme.Length == 0 || !schemes.Add(scheme))
            {
                throw new LRefusal(LRefusal.LRefusalScheme);
            }

            rows.Add(new LTranscription(
                Math.Max(draft.LTranscriptionDraftId, 0),
                entryId,
                rows.Count,
                scheme,
                draft.LTranscriptionDraftText.Trim()));
        }

        return rows;
    }

    private static bool LTranscriptionClerkMatch(
        IReadOnlyList<LTranscription> stored, IReadOnlyList<LTranscription> current)
    {
        if (stored.Count != current.Count)
        {
            return false;
        }

        for (int index = 0; index < stored.Count; index++)
        {
            if (stored[index].LTranscriptionId != current[index].LTranscriptionId
                || !string.Equals(
                    stored[index].LTranscriptionScheme, current[index].LTranscriptionScheme, StringComparison.Ordinal)
                || !string.Equals(
                    stored[index].LTranscriptionText, current[index].LTranscriptionText, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private static string LTranscriptionClerkFormat(IReadOnlyList<LTranscription> transcriptions)
    {
        List<string> lines = new(transcriptions.Count);
        foreach (LTranscription transcription in transcriptions)
        {
            lines.Add(transcription.LTranscriptionScheme + " " + transcription.LTranscriptionText);
        }

        return string.Join(", ", lines);
    }
}
