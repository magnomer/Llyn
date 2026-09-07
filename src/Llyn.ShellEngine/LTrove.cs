using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LTrove
{
    private readonly Dictionary<string, (string Word, string Language, IReadOnlyList<LCandidate> Found)>
        _lTroveCandidate = new(StringComparer.Ordinal);

    private readonly Dictionary<string, (string Word, string Language, IReadOnlyList<LRecording> Found)>
        _lTroveRecording = new(StringComparer.Ordinal);

    internal IReadOnlyList<LCandidate>? LTroveCandidateRead(string session, string word, string language)
    {
        if (session.Length == 0 ||
            !_lTroveCandidate.TryGetValue(session, out (string Word, string Language, IReadOnlyList<LCandidate> Found) held))
        {
            return null;
        }

        return LTroveHoldMatch(held.Word, held.Language, word, language) ? held.Found : null;
    }

    internal void LTroveCandidateSave(
        string session, string word, string language, IReadOnlyList<LCandidate> found)
    {
        if (session.Length == 0 || !LTroveCandidateCheck(found))
        {
            return;
        }

        _lTroveCandidate[session] = (word, language, found);
    }

    internal IReadOnlyList<LRecording>? LTroveRecordingRead(string session, string word, string language)
    {
        if (session.Length == 0 ||
            !_lTroveRecording.TryGetValue(session, out (string Word, string Language, IReadOnlyList<LRecording> Found) held))
        {
            return null;
        }

        return LTroveHoldMatch(held.Word, held.Language, word, language) ? held.Found : null;
    }

    internal void LTroveRecordingSave(
        string session, string word, string language, IReadOnlyList<LRecording> found)
    {
        if (session.Length == 0 || !LTroveRecordingCheck(found))
        {
            return;
        }

        _lTroveRecording[session] = (word, language, found);
    }

    internal void LTroveClear(string session)
    {
        _lTroveCandidate.Remove(session);
        _lTroveRecording.Remove(session);
    }

    internal void LTroveClear()
    {
        _lTroveCandidate.Clear();
        _lTroveRecording.Clear();
    }

    private static bool LTroveCandidateCheck(IReadOnlyList<LCandidate> found)
    {
        foreach (LCandidate candidate in found)
        {
            if (!string.IsNullOrEmpty(candidate.LCandidatePhonetic))
            {
                return true;
            }
        }

        return false;
    }

    private static bool LTroveRecordingCheck(IReadOnlyList<LRecording> found)
    {
        foreach (LRecording recording in found)
        {
            if (!string.IsNullOrEmpty(recording.LRecordingAddress))
            {
                return true;
            }
        }

        return false;
    }

    private static bool LTroveHoldMatch(string word, string language, string asked, string spoken)
    {
        return string.Equals(word, asked, StringComparison.Ordinal) &&
            string.Equals(language, spoken, StringComparison.Ordinal);
    }
}
