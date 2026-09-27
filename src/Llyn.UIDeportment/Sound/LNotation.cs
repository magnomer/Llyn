using System;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class LNotation
{
    private readonly CDesk _lNotationDesk;

    internal LNotation(CDesk desk)
    {
        ArgumentNullException.ThrowIfNull(desk);

        _lNotationDesk = desk;
    }

    public event Action<string, int>? LNotationSourceStarted;

    public event Action<CCandidate>? LNotationCandidateAdded;

    public event Action? LNotationFinished;

    public bool LNotationHeld => _lNotationDesk.CDeskErrand.CErrandTranscription is not null;

    public string LNotationLanguage => _lNotationDesk.CDeskErrand.CErrandTranscription?.LForayLanguage ?? string.Empty;

    public bool LNotationFlagged => _lNotationDesk.CDeskErrand.CErrandTranscription?.LForayFlagged ?? false;

    public bool LNotationPrimary => _lNotationDesk.CDeskErrand.CErrandTranscription?.LForayPrimary ?? false;

    public long LNotationTarget => _lNotationDesk.CDeskErrand.CErrandTranscription?.LForayTarget ?? 0;

    public string LNotationScheme => _lNotationDesk.CDeskErrand.CErrandTranscription?.LForayScheme ?? string.Empty;

    public bool LNotationSchemed => _lNotationDesk.CDeskErrand.CErrandTranscription?.LForaySchemed ?? false;

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
        _lNotationDesk.CDeskErrand.CErrandCancel();
    }
}
