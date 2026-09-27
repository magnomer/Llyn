using System;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class LNotation
{
    private readonly LDesk _lNotationDesk;

    internal LNotation(LDesk desk)
    {
        ArgumentNullException.ThrowIfNull(desk);

        _lNotationDesk = desk;
    }

    public event Action<string, int>? LNotationSourceStarted;

    public event Action<CCandidate>? LNotationCandidateAdded;

    public event Action? LNotationFinished;

    public bool LNotationHeld => _lNotationDesk.LDeskErrand.QErrandTranscription is not null;

    public string LNotationLanguage => _lNotationDesk.LDeskErrand.QErrandTranscription?.LForayLanguage ?? string.Empty;

    public bool LNotationFlagged => _lNotationDesk.LDeskErrand.QErrandTranscription?.LForayFlagged ?? false;

    public bool LNotationPrimary => _lNotationDesk.LDeskErrand.QErrandTranscription?.LForayPrimary ?? false;

    public long LNotationTarget => _lNotationDesk.LDeskErrand.QErrandTranscription?.LForayTarget ?? 0;

    public string LNotationScheme => _lNotationDesk.LDeskErrand.QErrandTranscription?.LForayScheme ?? string.Empty;

    public bool LNotationSchemed => _lNotationDesk.LDeskErrand.QErrandTranscription?.LForaySchemed ?? false;

    public void LNotationStepHandle(CLookupStep step)
    {
        ArgumentNullException.ThrowIfNull(step);

        if (step.CLookupStepCandidate is CCandidate candidate)
        {
            LNotationCandidateAdded?.Invoke(candidate);
            return;
        }

        if (step.CLookupStepEnded)
        {
            LNotationFinished?.Invoke();
            return;
        }

        LNotationSourceStarted?.Invoke(step.CLookupStepSource, step.CLookupStepOrder);
    }

    public void LNotationCancel()
    {
        _lNotationDesk.LDeskErrand.QErrandCancel();
    }
}
