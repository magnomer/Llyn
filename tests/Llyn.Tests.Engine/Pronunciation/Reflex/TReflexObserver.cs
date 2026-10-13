using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TReflexObserver
{
    private readonly long _tReflexObserverDraft;

    private readonly TaskCompletionSource<LBulletin> _tReflexObserverRaised =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    internal TReflexObserver(long draft = 0)
    {
        _tReflexObserverDraft = draft;
    }

    internal Task<LBulletin> TReflexObserverRaised => _tReflexObserverRaised.Task;

    internal void TReflexObserverHandle(LBulletin bulletin)
    {
        bool wanted = _tReflexObserverDraft == 0
            ? bulletin.LBulletinSubject == LSubject.LSubjectReflex
            : bulletin.LBulletinSubject == LSubject.LSubjectDraft && bulletin.LBulletinId == _tReflexObserverDraft;
        if (wanted)
        {
            _tReflexObserverRaised.TrySetResult(bulletin);
        }
    }
}
