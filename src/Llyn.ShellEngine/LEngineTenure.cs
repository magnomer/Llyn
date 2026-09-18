using System;
using System.Threading;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private int _lEngineTenureDelay = 250;

    internal int LEngineTenureDelay
    {
        get => Volatile.Read(ref _lEngineTenureDelay);
        set => Volatile.Write(ref _lEngineTenureDelay, value);
    }

    public LTenure LEngineTenureStart(string origin, LSubject subject, long? id)
    {
        LDraft started = subject switch
        {
            LSubject.LSubjectEntry => LEngineDraftStart(origin, id),
            LSubject.LSubjectExample => LEngineExampleStart(origin, id),
            LSubject.LSubjectSituation => LEngineSituationStart(origin, id),
            LSubject.LSubjectReference => LEngineReferenceStart(origin, id),
            LSubject.LSubjectAuthor => LEngineAuthorStart(origin, id),
            _ => throw new ArgumentOutOfRangeException(
                nameof(subject), subject, "A tenure holds only an entry, example, situation, reference or author."),
        };

        LTenure tenure = new(this, subject, started.LDraftId);
        LEngineObserverAttach(tenure);
        return tenure;
    }
}
