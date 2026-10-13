using System;
using System.Threading;
using Llyn.Application;
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

    internal LTenure LEngineOccurrenceStart(LVista vista, long? situation)
    {
        LTenure started = LEngineTenureStart(vista, null);
        if (situation is long linked)
        {
            started.LTenureRequestApply(new LRequestSituationPick(started.LTenureId, 0, linked, 0));
        }

        return started;
    }

    internal LTenure LEngineQuotationStart(LVista vista, long? example)
    {
        LTenure started = LEngineTenureStart(vista, null);
        if (example is long cited)
        {
            started.LTenureRequestApply(new LRequestSentenceExample(started.LTenureId, 0, 0, cited));
        }

        return started;
    }

    internal LTenure LEngineFootnoteStart(LVista vista, long? reference)
    {
        LTenure started = LEngineTenureStart(vista, null);
        if (reference is long cited)
        {
            started.LTenureRequestApply(new LRequestSentenceReference(started.LTenureId, 0, 0, cited));
        }

        return started;
    }

    internal LTenure LEngineMembershipStart(LVista vista, long? tag)
    {
        LTenure started = LEngineTenureStart(vista, null);
        if (tag is long carried)
        {
            started.LTenureRequestApply(new LRequestTagPick(started.LTenureId, 0, carried, 0));
        }

        return started;
    }

    internal LTenure LEngineCohortStart(LVista vista, long? register)
    {
        LTenure started = LEngineTenureStart(vista, null);
        if (register is long carried)
        {
            started.LTenureRequestApply(new LRequestRegisterPick(started.LTenureId, 0, carried, 0));
        }

        return started;
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

    internal long LEngineTenureCommit(LSubject subject, long id)
    {
        return subject switch
        {
            LSubject.LSubjectEntry => _lTenureFacadeEngine.LEngineDraft.LEngineDraftCommit(id).LOutcomeEntry.LEntryId,
            LSubject.LSubjectExample => _lTenureFacadeEngine.LEngineExample.LEngineExampleCommit(id).LExampleId,
            LSubject.LSubjectSituation => _lTenureFacadeEngine.LEngineSituation.LEngineSituationCommit(id).LSituationId,
            LSubject.LSubjectReference => _lTenureFacadeEngine.LEngineReference.LEngineReferenceCommit(id).LReferenceId,
            LSubject.LSubjectAuthor => _lTenureFacadeEngine.LEngineAuthor.LEngineAuthorCommit(id).LAuthorId,
            _ => throw new InvalidOperationException(
                "A tenure holds only an entry, example, situation, reference or author."),
        };
    }
}
