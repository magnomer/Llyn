using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTimbre
{
    [Fact]
    public void TimbrePhonemic_RespellingPhonemicPack_ReadsTrue()
    {
        CTimbre timbre = TTimbrePrepare(new()
        {
            ["LEngineRespellingCheck"] = _ => true,
            ["LEnginePhonemicCheck"] = _ => true,
        });

        Assert.True(timbre.CTimbrePhonemic);
    }

    [Fact]
    public void TimbrePhonemic_PhonemicPackWithoutRespelling_ReadsFalse()
    {
        CTimbre timbre = TTimbrePrepare(new()
        {
            ["LEngineRespellingCheck"] = _ => false,
            ["LEnginePhonemicCheck"] = _ => true,
        });

        Assert.False(timbre.CTimbrePhonemic);
    }

    [Fact]
    public void TimbreSpoken_SilentPack_ReadsFalse()
    {
        CTimbre timbre = TTimbrePrepare(new() { ["LEngineSilentCheck"] = _ => true });

        Assert.False(timbre.CTimbreSpoken);
    }

    [Fact]
    public void TimbreTonal_EmptyDesk_AsksThePackForNoLanguage()
    {
        List<string> asked = [];
        CTimbre timbre = TTimbrePrepare(new()
        {
            ["LEngineTonalCheck"] = args =>
            {
                asked.Add((string)args![0]!);
                return true;
            },
        });

        Assert.True(timbre.CTimbreTonal);
        Assert.Equal([string.Empty], asked);
    }

    private static CTimbre TTimbrePrepare(Dictionary<string, Func<object?[]?, object?>> answers)
    {
        return TInterfaceConduct.TEditorCreate(
                TEngineFake.TEngineStubCreate<LDraftPort>(),
                TEngineFake.TEngineStubCreate<LEntryPort>(),
                TEngineFake.TEngineCreate<LPhonologyPort>(answers),
                TEngineFake.TEngineStubCreate<LSettingsPort>(),
                TEngineFake.TEngineStubCreate<LMediaPort>())
            .CEditorTimbre;
    }
}
