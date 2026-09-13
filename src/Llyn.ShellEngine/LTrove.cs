using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LTrove
{
    private readonly Dictionary<long, (string LTroveWord, string LTroveLanguage, IReadOnlyList<LCandidate> LTroveFound)> _lTroveCandidate = [];

    private readonly Dictionary<long, (string LTroveWord, string LTroveLanguage, IReadOnlyList<LRecording> LTroveFound)> _lTroveRecording = [];

    private readonly Dictionary<(long LTroveSession, string LTroveScheme), (string LTroveWord, string LTroveLanguage, IReadOnlyList<LCandidate> LTroveFound)> _lTroveTranscription = [];

    internal IReadOnlyList<LCandidate>? LTroveCandidateRead(long session, string word, string language)
    {
        if (session == 0 ||
            !_lTroveCandidate.TryGetValue(session,
                out (string LTroveWord, string LTroveLanguage, IReadOnlyList<LCandidate> LTroveFound) held))
        {
            return null;
        }

        return LTroveHoldMatch(held.LTroveWord, held.LTroveLanguage, word, language) ? held.LTroveFound : null;
    }

    internal void LTroveCandidateSave(
        long session, string word, string language, IReadOnlyList<LCandidate> found)
    {
        if (session == 0 || !LTroveCandidateCheck(found))
        {
            return;
        }

        _lTroveCandidate[session] = (word, language, found);
    }

    internal IReadOnlyList<LRecording>? LTroveRecordingRead(long session, string word, string language)
    {
        if (session == 0 ||
            !_lTroveRecording.TryGetValue(session,
                out (string LTroveWord, string LTroveLanguage, IReadOnlyList<LRecording> LTroveFound) held))
        {
            return null;
        }

        return LTroveHoldMatch(held.LTroveWord, held.LTroveLanguage, word, language) ? held.LTroveFound : null;
    }

    internal void LTroveRecordingSave(
        long session, string word, string language, IReadOnlyList<LRecording> found)
    {
        if (session == 0 || !LTroveRecordingCheck(found))
        {
            return;
        }

        _lTroveRecording[session] = (word, language, found);
    }

    internal IReadOnlyList<LCandidate>? LTroveTranscriptionRead(long session, string word, string language, string scheme)
    {
        if (session == 0 ||
            !_lTroveTranscription.TryGetValue((session, scheme),
                out (string LTroveWord, string LTroveLanguage, IReadOnlyList<LCandidate> LTroveFound) held))
        {
            return null;
        }

        return LTroveHoldMatch(held.LTroveWord, held.LTroveLanguage, word, language) ? held.LTroveFound : null;
    }

    internal void LTroveTranscriptionSave(
        long session, string word, string language, string scheme, IReadOnlyList<LCandidate> found)
    {
        if (session == 0 || !LTroveCandidateCheck(found))
        {
            return;
        }

        _lTroveTranscription[(session, scheme)] = (word, language, found);
    }

    internal void LTroveClear(long session)
    {
        _lTroveCandidate.Remove(session);
        _lTroveRecording.Remove(session);
        foreach ((long LTroveSession, string LTroveScheme) key in _lTroveTranscription.Keys.Where(key => key.LTroveSession == session).ToList())
        {
            _lTroveTranscription.Remove(key);
        }
    }

    internal void LTroveClear()
    {
        _lTroveCandidate.Clear();
        _lTroveRecording.Clear();
        _lTroveTranscription.Clear();
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
