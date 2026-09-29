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
}
