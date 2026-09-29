using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceMention
{
    internal static CSentence TSentenceFailCreate(LEngine engine, CDesk desk, CEnvoy envoy)
    {
        LDraftPort drafts = TEngineFake.TEngineCreate<LDraftPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineMentionResolve"] = _ => throw new InvalidOperationException("no headwords"),
        });
        return new CSentence(desk, new LPhonologyOutlet(engine), drafts, new LSettingsOutlet(engine), envoy);
    }

    internal static IReadOnlyList<CMentionLabel> TMentionFailRead(LEngine engine, CDesk desk, CEnvoy envoy)
    {
        LDraftPort drafts = TEngineFake.TEngineCreate<LDraftPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineMentionResolve"] = _ => throw new InvalidOperationException("no headwords"),
        });
        return CMention.LMentionChipRead(drafts, desk, 0, 0, envoy, new LSettingsOutlet(engine));
    }

    internal static IReadOnlyList<CVistaRow> TQuotationFailRead(LEngine engine, CEnvoy envoy)
    {
        LEntryPort entries = TEngineFake.TEngineCreate<LEntryPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineEntryFind"] = _ => throw new InvalidOperationException("no entries"),
        });
        return new CQuotation(
            entries,
            new LPortraitOutlet(engine),
            new LSettingsOutlet(engine),
            envoy,
            static () => true,
            static _ => true,
            static () => true).CQuotationRowsRead();
    }

    internal static IReadOnlyList<CMeaning>? TMeaningRead(
        LEngine engine, CDesk desk, CEnvoy envoy, Func<object?[]?, object?> sense)
    {
        LDraftPort drafts = TEngineFake.TEngineCreate<LDraftPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineSenseRead"] = sense,
        });
        return CMention.LMentionMeaningRead(
            drafts, desk, 0, 0, "she knelt", 0, 3, envoy, new LSettingsOutlet(engine));
    }
}
