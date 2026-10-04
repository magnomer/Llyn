using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceCitation
{
    internal static CDesk TDeskFailCreate(LEngine engine, CEnvoy envoy) =>
        TDeskFailCreate(engine, envoy, "Input", "Input", CSubject.CSubjectEntry);

    internal static CDesk TDeskFailCreate(LEngine engine, CEnvoy envoy, string scope, string origin, CSubject subject)
    {
        LDraftPort real = new LDraftOutlet(engine);
        LDraftPort drafts = TEngineFake.TEngineCreate<LDraftPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineTenureStart"] = args => real.LEngineTenureStart(
                (string)args![0]!, (LSubject)args[1]!, (long?)args[2]),
            ["LEngineObserverAttach"] = _ => null,
            ["LEngineObserverDetach"] = _ => null,
            ["LEngineCitationResolve"] = _ => throw new InvalidOperationException("no sources"),
        });
        return new CDesk(drafts, new LSettingsOutlet(engine), scope, envoy, origin, subject);
    }

    internal static CImprint TImprintCreate(CAtelier atelier, CEnvoy envoy) => new(
        atelier.CAtelierDraftPort,
        atelier.CAtelierEntryPort,
        atelier.CAtelierSettingsPort,
        envoy,
        atelier.CAtelierLedger.LLedgerRepaint,
        static run => run());

    internal static CImprint TImprintFailCreate(LEngine engine, CEnvoy envoy)
    {
        LDraftPort real = new LDraftOutlet(engine);
        LDraftPort drafts = TEngineFake.TEngineCreate<LDraftPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineTenureStart"] = args => real.LEngineTenureStart((LVista)args![0]!, (long?)args[1]),
            ["LEngineObserverAttach"] = _ => null,
            ["LEngineObserverDetach"] = _ => null,
            ["LEngineBylineRead"] = args => real.LEngineBylineRead((string?)args![0]),
            ["LEngineBylineFind"] = _ => throw new InvalidOperationException("no authors"),
        });
        return new CImprint(
            drafts,
            new LEntryOutlet(engine),
            new LSettingsOutlet(engine),
            envoy,
            new CLedgerNoticed(),
            static run => run());
    }

    internal static CExample? TAnthologyDraftRead(this CAnthology anthology, LDraft? draft) =>
        anthology.LAnthologyDraftRead(draft);

    internal static CCard TCardFailCreate(LEngine engine, CDesk desk, CEnvoy envoy)
    {
        LEntryPort entries = TEngineFake.TEngineCreate<LEntryPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineReferenceFind"] = _ => throw new InvalidOperationException("no sources"),
        });
        return new CCard(desk, new LDraftOutlet(engine), entries, new LSettingsOutlet(engine), envoy);
    }
}
