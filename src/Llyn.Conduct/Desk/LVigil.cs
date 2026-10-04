using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

internal sealed class LVigil
{
    private readonly CDesk _lVigilDesk;

    private readonly List<(LSubject LVigilSubject, Action<LBulletin> LVigilObserver)> _lVigilTenureObservers = [];

    private readonly List<(LSubject LVigilSubject, Action<LBulletin> LVigilObserver)> _lVigilDraftObservers = [];

    private readonly List<(LSubject LVigilSubject, Action<LBulletin> LVigilObserver)> _lVigilEntryObservers = [];

    internal LVigil(CDesk desk)
    {
        ArgumentNullException.ThrowIfNull(desk);

        _lVigilDesk = desk;
    }

    internal void LVigilObserverAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        LSubject held = CCatalog.LCatalogSubjectRead(subject);
        _lVigilTenureObservers.Add((held, LVigilBulletinSend));
        _lVigilDesk.CDeskTenure?.LTenureObserverAttach(held, LVigilBulletinSend);

        void LVigilBulletinSend(LBulletin bulletin)
        {
            observer(new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored));
        }
    }

    internal void LVigilDraftAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        LSubject held = CCatalog.LCatalogSubjectRead(subject);
        _lVigilDraftObservers.Add((held, LVigilBulletinSend));
        _lVigilDesk.CDeskTenure?.LTenureDraftAttach(held, LVigilBulletinSend);

        void LVigilBulletinSend(LBulletin bulletin)
        {
            observer(new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored));
        }
    }

    internal void LVigilEntryAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        LSubject held = CCatalog.LCatalogSubjectRead(subject);
        _lVigilEntryObservers.Add((held, LVigilBulletinSend));
        _lVigilDesk.CDeskTenure?.LTenureEntryAttach(held, LVigilBulletinSend);

        void LVigilBulletinSend(LBulletin bulletin)
        {
            observer(new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored));
        }
    }

    internal void LVigilApply(LTenure started)
    {
        foreach ((LSubject subject, Action<LBulletin> observer) in _lVigilTenureObservers)
        {
            started.LTenureObserverAttach(subject, observer);
        }

        foreach ((LSubject subject, Action<LBulletin> observer) in _lVigilDraftObservers)
        {
            started.LTenureDraftAttach(subject, observer);
        }

        foreach ((LSubject subject, Action<LBulletin> observer) in _lVigilEntryObservers)
        {
            started.LTenureEntryAttach(subject, observer);
        }
    }
}
