using System;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    internal void PDisplayObserverAttach()
    {
        _lDisplay.LDisplayChosenAttach(
            LSubject.LSubjectFavorite, PObserver.PObserverCreate(this, PDisplayFavoriteUpdate));
        _lDisplay.LDisplayChosenAttach(LSubject.LSubjectGrasp, PObserver.PObserverCreate(this, PDisplayGraspUpdate));
        _lDisplay.LDisplayChosenAttach(
            LSubject.LSubjectFrequency, PObserver.PObserverCreate(this, PDisplayFrequencyUpdate));
        _lDisplay.LDisplayChosenAttach(
            LSubject.LSubjectInflection, PObserver.PObserverCreate(this, PDisplayParadigmUpdate));
        _lDisplay.LDisplayChosenAttach(LSubject.LSubjectReflex, PObserver.PObserverCreate(this, PDisplayReflexUpdate));
        _lDisplay.LDisplayChosenAttach(LSubject.LSubjectEntry, PObserver.PObserverCreate(this, PDisplayEntryUpdate));
        _lDisplay.LDisplayObserverAttach(
            LSubject.LSubjectScript, PObserver.PObserverCreate(this, PDisplayScriptUpdate));
        _lDisplay.LDisplayObserverAttach(
            LSubject.LSubjectFanqie, PObserver.PObserverCreate(this, PDisplayFanqieUpdate));
        _lDisplay.LDisplayObserverAttach(LSubject.LSubjectWorkspace, PObserver.PObserverCreate(this, PDisplayClear));
        _lDisplay.LDisplayObserverAttach(
            LSubject.LSubjectExample, PObserver.PObserverCreate(this, PDisplayEntryUpdate));
        _lDisplay.LDisplayObserverAttach(
            LSubject.LSubjectSituation, PObserver.PObserverCreate(this, PDisplayEntryUpdate));
        _lDisplay.LDisplayObserverAttach(
            LSubject.LSubjectReference, PObserver.PObserverCreate(this, PDisplayEntryUpdate));
        _lDisplay.LDisplayObserverAttach(LSubject.LSubjectAuthor, PObserver.PObserverCreate(this, PDisplayEntryUpdate));
        _lDisplay.LDisplayObserverAttach(LSubject.LSubjectTag, PObserver.PObserverCreate(this, PDisplayEntryUpdate));
        _lDisplay.LDisplayObserverAttach(
            LSubject.LSubjectRegister, PObserver.PObserverCreate(this, PDisplayEntryUpdate));
        _lDisplay.LDisplayObserverAttach(
            LSubject.LSubjectSettings, PObserver.PObserverCreate(this, PDisplayEntryUpdate));
    }

    private void PDisplayFavoriteUpdate()
    {
        if (_lDisplay.LDisplayChosen is long shown)
        {
            PDisplayFavoriteShow(shown);
        }
    }

    private void PDisplayGraspUpdate()
    {
        if (_lDisplay.LDisplayChosen is long shown)
        {
            PDisplayGraspShow(shown);
        }
    }

    private void PDisplayFrequencyUpdate()
    {
        if (_lDisplay.LDisplayChosen is long shown)
        {
            PDisplayFrequencyShow(shown);
        }
    }

    private void PDisplayParadigmUpdate()
    {
        if (_lDisplay.LDisplayChosen is long shown)
        {
            PDisplayParadigmShow(shown);
        }
    }

    private void PDisplayReflexUpdate()
    {
        if (_lDisplay.LDisplayChosen is long shown)
        {
            PDisplayReflexLoad(shown);
        }
    }

    private void PDisplayScriptUpdate()
    {
        if (_lDisplay.LDisplayChosen is long shown)
        {
            PDisplayScriptShow(shown, PDisplayLanguage.Text);
        }
    }

    private void PDisplayFanqieUpdate()
    {
        if (_lDisplay.LDisplayChosen is long shown)
        {
            PDisplayFanqieShow(shown, PDisplayLanguage.Text);
        }
    }

    private void PDisplayEntryUpdate()
    {
        if (_lDisplay.LDisplayChosen is not long shown)
        {
            return;
        }

        LEntryDraft? draft;
        try
        {
            draft = _lDisplay.LDisplayDraftLoad();
        }
        catch (Exception)
        {
            return;
        }

        if (draft is null)
        {
            PDisplayClear();
            return;
        }

        PDisplayShow(draft);
    }
}
