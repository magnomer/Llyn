using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LTenure
{
    private readonly List<(LSubject, Action<LBulletin>, long?)> _lTenureObservers = [];

    private int _lTenurePreparing;

    private LDraft? _lTenureKept;

    private int _lTenureRound;

    private LTenureState _lTenureLast;

    private LTenureState? _lTenureState;

    private long _lTenureStateRevision;

    public void LTenureObserverAttach(LSubject subject, Action<LBulletin> observer)
    {
        LTenureObserverInsert(subject, observer, null);
    }

    public void LTenureDraftAttach(LSubject subject, Action<LBulletin> observer)
    {
        LTenureObserverInsert(subject, observer, LTenureId);
    }

    public void LTenureEntryAttach(LSubject subject, Action<LBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);
        if (LTenureRead()?.LDraftStored is long id)
        {
            LTenureObserverInsert(subject, observer, id);
        }
    }

    private void LTenureObserverInsert(LSubject subject, Action<LBulletin> observer, long? id)
    {
        ArgumentNullException.ThrowIfNull(observer);
        lock (_lTenureGate)
        {
            if (!_lTenureQueue.LTenureQueueEnded)
            {
                _lTenureObservers.Add((subject, observer, id));
            }
        }
    }

    private void LTenureBulletinHandle(LBulletin bulletin)
    {
        (LSubject, Action<LBulletin>, long?)[] observers;
        lock (_lTenureGate)
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectDraft)
            {
                LTenureKeptClear();
            }

            if (_lTenureQueue.LTenureQueueEnded)
            {
                return;
            }

            if (_lTenurePreparing > 0 && bulletin.LBulletinSubject == LSubject.LSubjectDraft
                && bulletin.LBulletinId == LTenureId)
            {
                return;
            }

            observers = [.. _lTenureObservers];
        }

        foreach ((LSubject subject, Action<LBulletin> observer, long? id) in observers)
        {
            if (subject == bulletin.LBulletinSubject && (id is null || id == bulletin.LBulletinId))
            {
                observer(bulletin);
            }
        }
    }

    private const int LTenurePrepareRounds = 3;

    public LDraft? LTenurePrepare()
    {
        return LTenurePrepare(LTenureDraftPrepare);
    }

    private void LTenureDraftPrepare()
    {
        if (_lTenureSubject != LSubject.LSubjectEntry)
        {
            return;
        }

        int rounds = 0;
        while (rounds++ < LTenurePrepareRounds && LTenureDraftApply())
        {
        }
    }

    private bool LTenureDraftApply()
    {
        IReadOnlyList<LRequest> requests = _lEngine.LEngineDraft.LEngineDraftPrepare(LTenureId);
        foreach (LRequest request in requests)
        {
            LTenureRequestApply(request);
        }

        return requests.Count > 0;
    }

    public LDraft? LTenurePrepare(Action prepare)
    {
        ArgumentNullException.ThrowIfNull(prepare);
        lock (_lTenureTurn)
        {
            lock (_lTenureGate)
            {
                if (_lTenureQueue.LTenureQueueEnded)
                {
                    return null;
                }

                _lTenurePreparing++;
            }

            try
            {
                prepare();
                return LTenureRead();
            }
            finally
            {
                lock (_lTenureGate)
                {
                    _lTenurePreparing--;
                    LTenureKeptClear();
                }
            }
        }
    }

    public LMentionDraft? LTenureEtymologyFind(LMentionDraft span)
    {
        ArgumentNullException.ThrowIfNull(span);

        return LMentionClerk.LMentionEtymologyFind(LTenureRead(), span);
    }

    public LMentionDraft? LTenureMentionFind(long cardId, long sentenceId, LMentionDraft span)
    {
        ArgumentNullException.ThrowIfNull(span);

        return LMentionClerk.LMentionSpanFind(LTenureKeptRead(), cardId, sentenceId, span);
    }

    public bool LTenureMentionCheck(long cardId, long sentenceId, string text, int start, int length)
    {
        return LTenureMentionFind(cardId, sentenceId, LMentionClerk.LMentionSpanRead(text, start, length)) is not null;
    }

    public bool LTenureSenseCheck(long cardId, long sentenceId, string text, int start, int length)
    {
        return LTenureMentionFind(cardId, sentenceId, LMentionClerk.LMentionSpanRead(text, start, length))
            is { LMentionDraftLinked: true };
    }

    public long? LTenureSenseRead(long cardId, long sentenceId, string text, int start, int length)
    {
        LTenurePersist();
        return LTenureMentionFind(cardId, sentenceId, LMentionClerk.LMentionSpanRead(text, start, length))
            is { LMentionDraftLinked: true } found
            ? found.LMentionDraftEntry
            : null;
    }

    public long? LTenureStoredRead()
    {
        try
        {
            return LTenureRead()?.LDraftStored;
        }
        catch (Exception exception) when (LWorkspaceClerk.LWorkspaceRefusedCheck(exception))
        {
            return null;
        }
    }

    private LDraft? LTenureKeptRead()
    {
        int round;
        lock (_lTenureGate)
        {
            if (_lTenureQueue.LTenureQueueEnded)
            {
                return null;
            }

            if (_lTenureKept is LDraft kept)
            {
                return kept;
            }

            round = _lTenureRound;
        }

        LDraft? read;
        try
        {
            read = _lEngine.LEngineDraft.LEngineDraftRead(LTenureId);
        }
        catch (Exception exception) when (LWorkspaceClerk.LWorkspaceRefusedCheck(exception))
        {
            return null;
        }

        lock (_lTenureGate)
        {
            if (round == _lTenureRound)
            {
                _lTenureKept = read;
            }
        }

        return read;
    }

    private void LTenureKeptClear()
    {
        _lTenureKept = null;
        _lTenureRound++;
    }
    private void LTenureObserverClear()
    {
        _lEngine.LEngineObserverDetach(LTenureBulletinHandle);
        lock (_lTenureGate)
        {
            _lTenureObservers.Clear();
        }
    }

    private void LTenureStateRaise()
    {
        LTenureState state = LTenureStateRead();
        lock (_lTenureGate)
        {
            if (state == _lTenureLast)
            {
                return;
            }

            _lTenureLast = state;
        }

        _lEngine.LEngineBulletinRaise(LSubject.LSubjectTenure, LTenureId);
    }
}
