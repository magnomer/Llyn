using System;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PDisplay
{
    private void PDisplayBulletinHandle(LBulletin bulletin)
    {
        if (_pDisplayEntry is not long shown)
        {
            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectFavorite)
        {
            if (shown == bulletin.LBulletinId)
            {
                PDisplayFavoriteShow(shown);
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectGrasp)
        {
            if (shown == bulletin.LBulletinId)
            {
                PDisplayGraspShow(shown);
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectFrequency)
        {
            if (shown == bulletin.LBulletinId)
            {
                PDisplayFrequencyShow(shown);
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectInflection)
        {
            if (shown == bulletin.LBulletinId)
            {
                PDisplayParadigmShow(shown);
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectScript)
        {
            PDisplayScriptShow(shown, PDisplayLanguage.Text);
            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectFanqie)
        {
            PDisplayFanqieShow(shown, PDisplayLanguage.Text);
            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectReflex)
        {
            if (shown == bulletin.LBulletinId)
            {
                PDisplayReflexLoad(shown);
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectWorkspace)
        {
            PDisplayClear();
            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectDraft)
        {
            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectEntry
            && bulletin.LBulletinId > 0
            && shown != bulletin.LBulletinId)
        {
            return;
        }

        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(shown);
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

        PDisplayShow(shown, draft);
    }
}
