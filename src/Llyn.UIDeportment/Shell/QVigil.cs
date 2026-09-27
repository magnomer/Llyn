using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class QVigil
{
    private readonly LDesk _qVigilDesk;

    private readonly List<(LSubject QVigilSubject, Action<LBulletin> QVigilObserver)> _qVigilTenureObservers = [];

    private readonly List<(LSubject QVigilSubject, Action<LBulletin> QVigilObserver)> _qVigilDraftObservers = [];

    private readonly List<(LSubject QVigilSubject, Action<LBulletin> QVigilObserver)> _qVigilEntryObservers = [];

    internal QVigil(LDesk desk)
    {
        ArgumentNullException.ThrowIfNull(desk);

        _qVigilDesk = desk;
    }

    public void QVigilObserverAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        LSubject held = LPanel.LPanelSubjectRead(subject);
        _qVigilTenureObservers.Add((held, QVigilBulletinSend));
        _qVigilDesk.LDeskTenure?.LTenureObserverAttach(held, QVigilBulletinSend);

        void QVigilBulletinSend(LBulletin bulletin)
        {
            observer(new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored));
        }
    }

    public void QVigilDraftAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        LSubject held = LPanel.LPanelSubjectRead(subject);
        _qVigilDraftObservers.Add((held, QVigilBulletinSend));
        _qVigilDesk.LDeskTenure?.LTenureDraftAttach(held, QVigilBulletinSend);

        void QVigilBulletinSend(LBulletin bulletin)
        {
            observer(new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored));
        }
    }

    public void QVigilEntryAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        LSubject held = LPanel.LPanelSubjectRead(subject);
        _qVigilEntryObservers.Add((held, QVigilBulletinSend));
        _qVigilDesk.LDeskTenure?.LTenureEntryAttach(held, QVigilBulletinSend);

        void QVigilBulletinSend(LBulletin bulletin)
        {
            observer(new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored));
        }
    }

    internal void QVigilApply(LTenure started)
    {
        foreach ((LSubject subject, Action<LBulletin> observer) in _qVigilTenureObservers)
        {
            started.LTenureObserverAttach(subject, observer);
        }

        foreach ((LSubject subject, Action<LBulletin> observer) in _qVigilDraftObservers)
        {
            started.LTenureDraftAttach(subject, observer);
        }

        foreach ((LSubject subject, Action<LBulletin> observer) in _qVigilEntryObservers)
        {
            started.LTenureEntryAttach(subject, observer);
        }
    }
}
