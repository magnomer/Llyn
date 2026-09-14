using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<string> LEngineSchemeRead(string language)
    {
        return LEngineLanguageLoad(language).LLanguageSchemes.Select(static scheme => scheme.LSchemeName).ToList();
    }

    public Task LEngineTranscriptionFind(
        long session,
        string word,
        string language,
        string scheme,
        LReceiver receiver,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(receiver);
        ArgumentException.ThrowIfNullOrWhiteSpace(scheme);

        IReadOnlyList<LCandidate>? held;
        IReadOnlyList<LSource> sources;
        lock (_lEngineGate)
        {
            held = _lEngineTrove.LTroveTranscriptionRead(session, word, language, scheme);
            sources = held is null ? LEngineSchemeLoad(language, scheme) : [];
        }

        return held is null
            ? LEngineTranscriptionScan(session, word, language, scheme, sources, receiver, cancellation)
            : LEngineCandidatePublish(held, receiver);
    }

    private async Task LEngineTranscriptionScan(
        long session,
        string word,
        string language,
        string scheme,
        IReadOnlyList<LSource> sources,
        LReceiver receiver,
        CancellationToken cancellation)
    {
        IReadOnlyList<LCandidate> found =
            await new LLookup(sources, [], [], true)
                .LSeekerStart(word, receiver, cancellation)
                .ConfigureAwait(false);

        lock (_lEngineGate)
        {
            _lEngineTrove.LTroveTranscriptionSave(session, word, language, scheme, found);
        }
    }

    private IReadOnlyList<LSource> LEngineSchemeLoad(string language, string scheme)
    {
        if (_lEngineSchemeSources.TryGetValue((language, scheme), out IReadOnlyList<LSource>? sources))
        {
            return sources;
        }

        LLanguage pack = LEngineLanguageLoad(language);
        LScheme? declared = pack.LLanguageSchemes
            .FirstOrDefault(known => string.Equals(known.LSchemeName, scheme, StringComparison.Ordinal));
        IReadOnlyList<LSourceSpec> specs = declared?.LSchemeSources
            ?? pack.LLanguageGlyph?.LGlyphSourceRead(scheme)
            ?? [];
        sources = LSourceFactory.LSourceFactoryCreate(specs, _lEngineClient);
        _lEngineSchemeSources[(language, scheme)] = sources;
        return sources;
    }

    public IReadOnlyList<LTranscription> LEngineTranscriptionRead(long entryId)
    {
        lock (_lEngineGate)
        {
            return new LTranscriptionArchive(_lEngineDatabase).LTranscriptionRead(entryId);
        }
    }

    public IReadOnlyList<LTranscription> LEngineTranscriptionSet(
        long entryId, IReadOnlyList<LTranscription> transcriptions)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(transcriptions);
            IReadOnlyList<LTranscription> saved =
                new LTranscriptionArchive(_lEngineDatabase).LTranscriptionSet(entryId, transcriptions);
            LEngineUpdatedSet(entryId);
            return saved;
        }
    }

    private void LEngineTranscriptionSync(
        long entryId,
        IReadOnlyList<LTranscriptionDraft> drafts,
        List<LRevisionChange>? changes,
        Dictionary<long, long> identity)
    {
        LTranscriptionArchive transcriptions = new(_lEngineDatabase);
        IReadOnlyList<LTranscription> stored = transcriptions.LTranscriptionRead(entryId);
        IReadOnlyList<LTranscriptionDraft> written = LEngineTranscriptionScan(drafts);
        IReadOnlyList<LTranscription> current = LEngineTranscriptionRead(entryId, written);

        if (LEngineTranscriptionMatch(stored, current))
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
            LEngineIdentityRecord(identity, written[index].LTranscriptionDraftId, saved[index].LTranscriptionId);
        }

        changes?.Add(new LRevisionChange(
            0,
            entryId,
            "transcription",
            current.Count == 0 ? "delete" : stored.Count == 0 ? "create" : "update",
            LEngineTranscriptionFormat(current)));
    }

    private static IReadOnlyList<LTranscriptionDraft> LEngineTranscriptionScan(IReadOnlyList<LTranscriptionDraft> drafts)
    {
        List<LTranscriptionDraft> filled = [];
        foreach (LTranscriptionDraft draft in drafts)
        {
            if (!draft.LTranscriptionDraftSeeded || !draft.LTranscriptionDraftEmpty)
            {
                filled.Add(draft);
            }
        }

        return filled;
    }

    private static IReadOnlyList<LTranscription> LEngineTranscriptionRead(
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

    private static bool LEngineTranscriptionMatch(
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

    private static string LEngineTranscriptionFormat(IReadOnlyList<LTranscription> transcriptions)
    {
        List<string> lines = new(transcriptions.Count);
        foreach (LTranscription transcription in transcriptions)
        {
            lines.Add(transcription.LTranscriptionScheme + " " + transcription.LTranscriptionText);
        }

        return string.Join(", ", lines);
    }
}
