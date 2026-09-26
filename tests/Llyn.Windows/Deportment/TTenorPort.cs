using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TTenorPort
{
    [Fact]
    public void TenorRowsRead_NoVistaRestored_AnswersNothingWithoutAsking()
    {
        LTenor tenor = TInterfaceDeportment.TTenorCreate(
            TEngineFake.TEngineStubCreate<LEntryPort>(), TEngineFake.TEngineStubCreate<LSettingsPort>());

        Assert.Empty(tenor.TTenorRowsRead());
        Assert.Null(tenor.LTenorChosen);
        Assert.False(tenor.LTenorFiltered);
    }

    [Fact]
    public void TenorRegisterCreate_Name_ForwardsToPort()
    {
        List<string> created = [];
        LEntryPort entries = TEngineFake.TEngineCreate<LEntryPort>(
            new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineRegisterCreate"] = args =>
            {
                created.Add((string)args![0]!);
                return TInterface.TRegisterCreate(7, "formal");
            },
        });
        LTenor tenor = TInterfaceDeportment.TTenorCreate(entries, TEngineFake.TEngineStubCreate<LSettingsPort>());

        LRegister register = tenor.TTenorRegisterCreate("formal");

        Assert.Equal(7, register.LRegisterId);
        Assert.Equal(["formal"], created);
    }

    [Fact]
    public void TenorLanguageRead_SettingsPort_ForwardsWithoutEntries()
    {
        LSettingsPort settings = TEngineFake.TEngineCreate<LSettingsPort>(
            new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineLanguageRead"] = _ => new[] { "Korean", "Latin" },
        });
        LTenor tenor = TInterfaceDeportment.TTenorCreate(TEngineFake.TEngineStubCreate<LEntryPort>(), settings);

        Assert.Equal(["Korean", "Latin"], tenor.TTenorLanguageRead());
    }
}
