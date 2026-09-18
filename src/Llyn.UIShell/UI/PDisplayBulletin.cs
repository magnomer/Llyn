using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PDisplay
{
    private void PDisplayObserverAttach(LVista vista)
    {
        vista.LVistaChosenAttach(LSubject.LSubjectFavorite, new PObserver(this, PDisplayFavoriteUpdate));
        vista.LVistaChosenAttach(LSubject.LSubjectGrasp, new PObserver(this, PDisplayGraspUpdate));
        vista.LVistaChosenAttach(LSubject.LSubjectFrequency, new PObserver(this, PDisplayFrequencyUpdate));
        vista.LVistaChosenAttach(LSubject.LSubjectInflection, new PObserver(this, PDisplayParadigmUpdate));
        vista.LVistaChosenAttach(LSubject.LSubjectReflex, new PObserver(this, PDisplayReflexUpdate));
        vista.LVistaChosenAttach(LSubject.LSubjectEntry, new PObserver(this, PDisplayEntryUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectScript, new PObserver(this, PDisplayScriptUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectFanqie, new PObserver(this, PDisplayFanqieUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectWorkspace, new PObserver(this, PDisplayClear));
        vista.LVistaObserverAttach(LSubject.LSubjectExample, new PObserver(this, PDisplayEntryUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectSituation, new PObserver(this, PDisplayEntryUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectReference, new PObserver(this, PDisplayEntryUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectAuthor, new PObserver(this, PDisplayEntryUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectTag, new PObserver(this, PDisplayEntryUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectRegister, new PObserver(this, PDisplayEntryUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectSettings, new PObserver(this, PDisplayEntryUpdate));
    }

    private void PDisplayFavoriteUpdate()
    {
        if (_pDisplayVista?.LVistaChosen is long shown)
        {
            PDisplayFavoriteShow(shown);
        }
    }

    private void PDisplayGraspUpdate()
    {
        if (_pDisplayVista?.LVistaChosen is long shown)
        {
            PDisplayGraspShow(shown);
        }
    }

    private void PDisplayFrequencyUpdate()
    {
        if (_pDisplayVista?.LVistaChosen is long shown)
        {
            PDisplayFrequencyShow(shown);
        }
    }

    private void PDisplayParadigmUpdate()
    {
        if (_pDisplayVista?.LVistaChosen is long shown)
        {
            PDisplayParadigmShow(shown);
        }
    }

    private void PDisplayReflexUpdate()
    {
        if (_pDisplayVista?.LVistaChosen is long shown)
        {
            PDisplayReflexLoad(shown);
        }
    }

    private void PDisplayScriptUpdate()
    {
        if (_pDisplayVista?.LVistaChosen is long shown)
        {
            PDisplayScriptShow(shown, PDisplayLanguage.Text);
        }
    }

    private void PDisplayFanqieUpdate()
    {
        if (_pDisplayVista?.LVistaChosen is long shown)
        {
            PDisplayFanqieShow(shown, PDisplayLanguage.Text);
        }
    }

    private void PDisplayEntryUpdate()
    {
        if (_pDisplayVista?.LVistaChosen is not long shown)
        {
            return;
        }

        LEntryDraft? draft;
        try
        {
            draft = _pDisplayVista?.LVistaLoad()?.LDraftContent;
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
