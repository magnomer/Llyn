using System;
using System.Threading;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LTenureFacade
{
    private readonly LEngine _lTenureFacadeEngine;
    private int _lTenureFacadeDelay = 250;

    public LTenureFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lTenureFacadeEngine = engine;
    }

    internal int LEngineTenureDelay
    {
        get => Volatile.Read(ref _lTenureFacadeDelay);
        set => Volatile.Write(ref _lTenureFacadeDelay, value);
    }

    internal LTenure LEngineTenureStart(LVista vista, long? id)
    {
        ArgumentNullException.ThrowIfNull(vista);

        LSubject subject = vista.LVistaSubject
            ?? throw new InvalidOperationException("The vista names no subject to hold.");
        return LEngineTenureStart(vista.LVistaTab, subject, id);
    }

    internal LTenure LEngineTenureStart(string origin, LSubject subject, long? id)
    {
        LDraft started = subject switch
        {
            LSubject.LSubjectEntry => _lTenureFacadeEngine.LEngineDraft.LEngineDraftStart(origin, id),
            LSubject.LSubjectExample => _lTenureFacadeEngine.LEngineExample.LEngineExampleStart(origin, id),
            LSubject.LSubjectSituation => _lTenureFacadeEngine.LEngineSituation.LEngineSituationStart(origin, id),
            LSubject.LSubjectReference => _lTenureFacadeEngine.LEngineReference.LEngineReferenceStart(origin, id),
            LSubject.LSubjectAuthor => _lTenureFacadeEngine.LEngineAuthor.LEngineAuthorStart(origin, id),
            _ => throw new ArgumentOutOfRangeException(
                nameof(subject), subject, "A tenure holds only an entry, example, situation, reference or author."),
        };

        return new LTenure(_lTenureFacadeEngine, subject, started.LDraftId);
    }
}
