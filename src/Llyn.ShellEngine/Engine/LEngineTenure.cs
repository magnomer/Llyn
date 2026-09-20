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

    public LTenure LEngineTenureStart(LVista vista, long? id)
    {
        ArgumentNullException.ThrowIfNull(vista);

        LSubject subject = vista.LVistaSubject
            ?? throw new InvalidOperationException("The vista names no subject to hold.");
        return LEngineTenureStart(vista.LVistaTab, subject, id);
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

        return new LTenure(this, subject, started.LDraftId);
    }
}
