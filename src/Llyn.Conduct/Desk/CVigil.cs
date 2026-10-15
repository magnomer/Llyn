using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

internal sealed class CVigil
{
    private readonly CDesk _cVigilDesk;

    private readonly List<(LSubject CVigilSubject, Action<LBulletin> CVigilObserver)> _cVigilTenureObservers = [];

    private readonly List<(LSubject CVigilSubject, Action<LBulletin> CVigilObserver)> _cVigilDraftObservers = [];

    private readonly List<(LSubject CVigilSubject, Action<LBulletin> CVigilObserver)> _cVigilEntryObservers = [];

    internal CVigil(CDesk desk)
    {
        ArgumentNullException.ThrowIfNull(desk);

        _cVigilDesk = desk;
    }

    internal void CVigilObserverAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        LSubject held = CCatalog.LCatalogSubjectRead(subject);
        _cVigilTenureObservers.Add((held, CVigilBulletinSend));
        _cVigilDesk.CDeskDraft.CDeskDraftTenure?.LTenureObserverAttach(held, CVigilBulletinSend);

        void CVigilBulletinSend(LBulletin bulletin)
        {
            observer(new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored, bulletin.LBulletinRefusal));
        }
    }

    internal void CVigilDraftAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        LSubject held = CCatalog.LCatalogSubjectRead(subject);
        _cVigilDraftObservers.Add((held, CVigilBulletinSend));
        _cVigilDesk.CDeskDraft.CDeskDraftTenure?.LTenureDraftAttach(held, CVigilBulletinSend);

        void CVigilBulletinSend(LBulletin bulletin)
        {
            observer(new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored, bulletin.LBulletinRefusal));
        }
    }

    internal void CVigilEntryAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        LSubject held = CCatalog.LCatalogSubjectRead(subject);
        _cVigilEntryObservers.Add((held, CVigilBulletinSend));
        _cVigilDesk.CDeskDraft.CDeskDraftTenure?.LTenureEntryAttach(held, CVigilBulletinSend);

        void CVigilBulletinSend(LBulletin bulletin)
        {
            observer(new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored, bulletin.LBulletinRefusal));
        }
    }

    internal void CVigilApply(LTenure started)
    {
        foreach ((LSubject subject, Action<LBulletin> observer) in _cVigilTenureObservers)
        {
            started.LTenureObserverAttach(subject, observer);
        }

        foreach ((LSubject subject, Action<LBulletin> observer) in _cVigilDraftObservers)
        {
            started.LTenureDraftAttach(subject, observer);
        }

        foreach ((LSubject subject, Action<LBulletin> observer) in _cVigilEntryObservers)
        {
            started.LTenureEntryAttach(subject, observer);
        }
    }
}
