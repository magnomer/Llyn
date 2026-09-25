using System;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    internal void PDisplayObserverAttach()
    {
        _lLectern.LLecternChosenAttach(
            LSubject.LSubjectFavorite, PObserver.PObserverCreate(this, PDisplayFavoriteUpdate));
        _lLectern.LLecternChosenAttach(LSubject.LSubjectGrasp, PObserver.PObserverCreate(this, PDisplayGraspUpdate));
        _lLectern.LLecternChosenAttach(
            LSubject.LSubjectFrequency, PObserver.PObserverCreate(this, PDisplayFrequencyUpdate));
        _lLectern.LLecternChosenAttach(
            LSubject.LSubjectInflection, PObserver.PObserverCreate(this, PDisplayParadigmUpdate));
        _lLectern.LLecternChosenAttach(LSubject.LSubjectReflex, PObserver.PObserverCreate(this, PDisplayReflexUpdate));
        _lLectern.LLecternChosenAttach(LSubject.LSubjectEntry, PObserver.PObserverCreate(this, PDisplayEntryUpdate));
        _lLectern.LLecternObserverAttach(
            LSubject.LSubjectScript, PObserver.PObserverCreate(this, PDisplayScriptUpdate));
        _lLectern.LLecternObserverAttach(
            LSubject.LSubjectFanqie, PObserver.PObserverCreate(this, PDisplayFanqieUpdate));
        _lLectern.LLecternObserverAttach(LSubject.LSubjectWorkspace, PObserver.PObserverCreate(this, PDisplayClear));
        _lLectern.LLecternObserverAttach(
            LSubject.LSubjectExample, PObserver.PObserverCreate(this, PDisplayEntryUpdate));
        _lLectern.LLecternObserverAttach(
            LSubject.LSubjectSituation, PObserver.PObserverCreate(this, PDisplayEntryUpdate));
        _lLectern.LLecternObserverAttach(
            LSubject.LSubjectReference, PObserver.PObserverCreate(this, PDisplayEntryUpdate));
        _lLectern.LLecternObserverAttach(LSubject.LSubjectAuthor, PObserver.PObserverCreate(this, PDisplayEntryUpdate));
        _lLectern.LLecternObserverAttach(LSubject.LSubjectTag, PObserver.PObserverCreate(this, PDisplayEntryUpdate));
        _lLectern.LLecternObserverAttach(
            LSubject.LSubjectRegister, PObserver.PObserverCreate(this, PDisplayEntryUpdate));
        _lLectern.LLecternObserverAttach(
            LSubject.LSubjectSettings, PObserver.PObserverCreate(this, PDisplayEntryUpdate));
    }

    private void PDisplayFavoriteUpdate()
    {
        if (_lLectern.LLecternChosen is long shown)
        {
            PDisplayFavoriteShow(shown);
        }
    }

    private void PDisplayGraspUpdate()
    {
        if (_lLectern.LLecternChosen is long shown)
        {
            PDisplayGraspShow(shown);
        }
    }

    private void PDisplayFrequencyUpdate()
    {
        if (_lLectern.LLecternChosen is long shown)
        {
            PDisplayFrequencyShow(shown);
        }
    }

    private void PDisplayParadigmUpdate()
    {
        if (_lLectern.LLecternChosen is long shown)
        {
            PDisplayParadigmShow(shown);
        }
    }

    private void PDisplayReflexUpdate()
    {
        if (_lLectern.LLecternChosen is long shown)
        {
            PDisplayReflexLoad(shown);
        }
    }

    private void PDisplayScriptUpdate()
    {
        if (_lLectern.LLecternChosen is long shown)
        {
            PDisplayScriptShow(shown, PDisplayLanguage.Text);
        }
    }

    private void PDisplayFanqieUpdate()
    {
        if (_lLectern.LLecternChosen is long shown)
        {
            PDisplayFanqieShow(shown, PDisplayLanguage.Text);
        }
    }

    private void PDisplayEntryUpdate()
    {
        if (_lLectern.LLecternChosen is not long shown)
        {
            return;
        }

        LEntryDraft? draft;
        try
        {
            draft = _lLectern.LLecternDraftLoad();
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
