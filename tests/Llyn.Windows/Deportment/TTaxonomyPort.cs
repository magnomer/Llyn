using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TTaxonomyPort
{
    [Fact]
    public void TaxonomyRowsRead_NoVistaRestored_AnswersNothingWithoutAsking()
    {
        LTaxonomy taxonomy = TInterfaceDeportment.TTaxonomyCreate(
            TEngineFake.TEngineStubCreate<LEntryPort>(), TEngineFake.TEngineStubCreate<LSettingsPort>());

        Assert.Empty(taxonomy.TTaxonomyRowsRead());
        Assert.Null(taxonomy.LTaxonomyChosen);
        Assert.False(taxonomy.LTaxonomyFiltered);
    }

    [Fact]
    public void TaxonomyTagCreate_Name_ForwardsToPortAndAnswersTheId()
    {
        List<string> created = [];
        LEntryPort entries = TEngineFake.TEngineCreate<LEntryPort>(
            new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineTagCreate"] = args =>
            {
                created.Add((string)args![0]!);
                return TInterface.TTagCreate(9, "motion");
            },
        });
        LTaxonomy taxonomy = TInterfaceDeportment.TTaxonomyCreate(
            entries, TEngineFake.TEngineStubCreate<LSettingsPort>());

        long tag = taxonomy.TTaxonomyTagCreate("motion");

        Assert.Equal(9, tag);
        Assert.Equal(["motion"], created);
    }

    [Fact]
    public void TaxonomyCoinageCheck_NothingChosenNorShown_NamesATag()
    {
        LTaxonomy taxonomy = TInterfaceDeportment.TTaxonomyCreate(
            TEngineFake.TEngineStubCreate<LEntryPort>(), TEngineFake.TEngineStubCreate<LSettingsPort>());

        Assert.True(taxonomy.TTaxonomyCoinageCheck());
    }

    [Fact]
    public void TaxonomyEmptyRead_BlankOrWrittenSearch_PicksVacantOrUnmatched()
    {
        LTaxonomy taxonomy = TInterfaceDeportment.TTaxonomyCreate(
            TEngineFake.TEngineStubCreate<LEntryPort>(), TEngineFake.TEngineStubCreate<LSettingsPort>());

        Assert.Equal("Tag.Vacant", taxonomy.TTaxonomyEmptyRead(" "));
        Assert.Equal("Tag.Unmatched", taxonomy.TTaxonomyEmptyRead("aqua"));
    }

    [Fact]
    public void TaxonomyTagRead_ChosenTag_CopiesIdTextAndMark()
    {
        IReadOnlyList<CCatalogTag> rows = TInterfaceDeportment.TTaxonomyTagRead(
            [TInterfaceDeportment.TCatalogTagCreate(TInterface.TTagCreate(9, "motion"), true)]);

        CCatalogTag row = Assert.Single(rows);
        Assert.Equal(new CTag(9, "motion"), row.CCatalogTagStored);
        Assert.True(row.CCatalogTagChosen);
    }
}
