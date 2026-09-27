using System;
using System.Collections.Generic;
using Llyn.Conduct;
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
    public void TenorRegisterCreate_Name_ForwardsToPortAndAnswersTheId()
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

        long register = tenor.TTenorRegisterCreate("formal");

        Assert.Equal(7, register);
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

    [Fact]
    public void TenorCoinageCheck_NothingChosenNorShown_NamesARegister()
    {
        LTenor tenor = TInterfaceDeportment.TTenorCreate(
            TEngineFake.TEngineStubCreate<LEntryPort>(), TEngineFake.TEngineStubCreate<LSettingsPort>());

        Assert.True(tenor.TTenorCoinageCheck());
    }

    [Fact]
    public void TenorRegisterRead_ChosenRegister_CopiesNameUsageAndMark()
    {
        IReadOnlyList<CCatalogRegister> rows = TInterfaceDeportment.TTenorRegisterRead(
            [TInterfaceDeportment.TCatalogRegisterCreate(TInterface.TRegisterCreate(7, "formal"), 3, true)]);

        CCatalogRegister row = Assert.Single(rows);
        Assert.Equal(new CRegister(7, "formal"), row.CCatalogRegisterStored);
        Assert.Equal(3, row.CCatalogRegisterUsage);
        Assert.True(row.CCatalogRegisterChosen);
    }
}
