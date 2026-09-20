using System;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    internal void PDisplayObserverAttach()
    {
        _lDisplay.LDisplayChosenAttach(LSubject.LSubjectFavorite, new PObserver(this, PDisplayFavoriteUpdate));
        _lDisplay.LDisplayChosenAttach(LSubject.LSubjectGrasp, new PObserver(this, PDisplayGraspUpdate));
        _lDisplay.LDisplayChosenAttach(LSubject.LSubjectFrequency, new PObserver(this, PDisplayFrequencyUpdate));
        _lDisplay.LDisplayChosenAttach(LSubject.LSubjectInflection, new PObserver(this, PDisplayParadigmUpdate));
        _lDisplay.LDisplayChosenAttach(LSubject.LSubjectReflex, new PObserver(this, PDisplayReflexUpdate));
        _lDisplay.LDisplayChosenAttach(LSubject.LSubjectEntry, new PObserver(this, PDisplayEntryUpdate));
        _lDisplay.LDisplayObserverAttach(LSubject.LSubjectScript, new PObserver(this, PDisplayScriptUpdate));
        _lDisplay.LDisplayObserverAttach(LSubject.LSubjectFanqie, new PObserver(this, PDisplayFanqieUpdate));
        _lDisplay.LDisplayObserverAttach(LSubject.LSubjectWorkspace, new PObserver(this, PDisplayClear));
        _lDisplay.LDisplayObserverAttach(LSubject.LSubjectExample, new PObserver(this, PDisplayEntryUpdate));
        _lDisplay.LDisplayObserverAttach(LSubject.LSubjectSituation, new PObserver(this, PDisplayEntryUpdate));
        _lDisplay.LDisplayObserverAttach(LSubject.LSubjectReference, new PObserver(this, PDisplayEntryUpdate));
        _lDisplay.LDisplayObserverAttach(LSubject.LSubjectAuthor, new PObserver(this, PDisplayEntryUpdate));
        _lDisplay.LDisplayObserverAttach(LSubject.LSubjectTag, new PObserver(this, PDisplayEntryUpdate));
        _lDisplay.LDisplayObserverAttach(LSubject.LSubjectRegister, new PObserver(this, PDisplayEntryUpdate));
        _lDisplay.LDisplayObserverAttach(LSubject.LSubjectSettings, new PObserver(this, PDisplayEntryUpdate));
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
