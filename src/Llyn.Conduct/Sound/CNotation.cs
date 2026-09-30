using System.Collections.Generic;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

internal sealed class CNotation
{
    private readonly List<CNotationItem> _cNotationRows = [];

    private bool _cNotationSearching;

    private bool _cNotationSchemed;

    internal void LNotationStart(bool schemed)
    {
        _cNotationRows.Clear();
        _cNotationSearching = true;
        _cNotationSchemed = schemed;
    }

    internal void LNotationFinish()
    {
        _cNotationSearching = false;
    }

    internal CNotationRoll LNotationRead()
    {
        return new CNotationRoll(
            [.. _cNotationRows],
            _cNotationRows.Count == 0,
            _cNotationSearching,
            _cNotationSearching ? "Phonetician.Searching"
            : _cNotationSchemed ? "Transcription.Empty"
            : "Phonetician.Empty");
    }

    internal int LNotationPlace(string source, int order)
    {
        int position = 0;
        while (position < _cNotationRows.Count && _cNotationRows[position].CNotationItemOrder < order)
        {
            position++;
        }

        if (position < _cNotationRows.Count && _cNotationRows[position].CNotationItemOrder == order)
        {
            return position;
        }

        _cNotationRows.Insert(position, new CNotationItem(source, order, [], "Phonetician.Searching", false));
        return position;
    }

    internal void LNotationCandidateAdd(CCandidate candidate, LForay foray)
    {
        int position = LNotationPlace(candidate.CCandidateSource, candidate.CCandidateOrder);
        CNotationItem row = _cNotationRows[position];
        if (candidate.CCandidateNotated)
        {
            _cNotationRows[position] = row with
            {
                CNotationItemReading = [.. row.CNotationItemReading, LNotationReadingRead(candidate, foray)],
                CNotationItemNotice = string.Empty,
                CNotationItemReady = true,
            };
            return;
        }

        if (row.CNotationItemReading.Count > 0)
        {
            return;
        }

        _cNotationRows[position] = row with
        {
            CNotationItemNotice = candidate.CCandidateReached ? "Phonetician.Missing" : "Phonetician.Broken",
            CNotationItemReady = false,
        };
    }

    private static CNotationReading LNotationReadingRead(CCandidate candidate, LForay foray)
    {
        (bool shown, string opener, string closer) = foray.LForayMarkRead();
        return new CNotationReading(
            candidate.CCandidatePhonetic ?? string.Empty,
            foray.LForayReadingRead(candidate.CCandidatePhonetic, candidate.CCandidateRespelling),
            CSounding.CSoundingVarietyRead(foray.LForayLanguage, candidate.CCandidateVariety),
            candidate.CCandidateRegional && foray.LForayFlagged,
            new CRespellingMark(shown, opener, closer));
    }
}
