using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceTrove
{
    internal static LTrove TTroveCreate() =>
        new LTrove();

    internal static void TTroveCandidateSave(
        this LTrove trove,
        long session,
        string word,
        string language,
        IReadOnlyList<LCandidate> found)
    {
        trove.LTroveCandidateSave(session, word, language, found);
    }

    internal static IReadOnlyList<LCandidate>? TTroveCandidateRead(
        this LTrove trove,
        long session,
        string word,
        string language) =>
        trove.LTroveCandidateRead(session, word, language);

    internal static void TTroveRecordingSave(
        this LTrove trove,
        long session,
        string word,
        string language,
        IReadOnlyList<LRecording> found)
    {
        trove.LTroveRecordingSave(session, word, language, found);
    }

    internal static IReadOnlyList<LRecording>? TTroveRecordingRead(
        this LTrove trove,
        long session,
        string word,
        string language) =>
        trove.LTroveRecordingRead(session, word, language);

    internal static void TTroveTranscriptionSave(
        this LTrove trove,
        long session,
        string word,
        string language,
        string scheme,
        IReadOnlyList<LCandidate> found)
    {
        trove.LTroveTranscriptionSave(session, word, language, scheme, found);
    }

    internal static IReadOnlyList<LCandidate>? TTroveTranscriptionRead(
        this LTrove trove,
        long session,
        string word,
        string language,
        string scheme) =>
        trove.LTroveTranscriptionRead(session, word, language, scheme);

    internal static void TTroveClear(this LTrove trove, long session)
    {
        trove.LTroveClear(session);
    }
}
