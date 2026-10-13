using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TParadigmCell
{
    [Fact]
    public void SpeechPackLoad_ParadigmCells_ReadsEachCellInOrder()
    {
        using TLanguageFixture fixture = TLanguageFixture.TLanguageFixtureCreate("""{ "listed": false }""");
        fixture.TLanguageFixtureSave("vocabulary.json", """
            {
              "paradigms": [
                { "part": 6, "values": [4], "cells": [[9, 12, 17], [10, 14, 24]] },
                { "part": 7, "cells": [[20, 3]] },
                { "part": 1, "values": [2, 3] }
              ]
            }
            """);

        LSpeechPack pack = TInterface.TSpeechPackLoad(fixture.TLanguageFixtureName);

        Assert.Equal(3, pack.LSpeechPackParadigms.Count);
        LParadigm verb = pack.LSpeechPackParadigms[0];
        Assert.Equal([4L], verb.LParadigmCells.Where(cell => cell.Count == 1).Select(cell => cell[0]));
        Assert.Equal(3, verb.LParadigmCells.Count);
        Assert.Equal([4L], verb.LParadigmCells[0]);
        Assert.Equal([9L, 12L, 17L], verb.LParadigmCells[1]);
        Assert.Equal([10L, 14L, 24L], verb.LParadigmCells[2]);
        LParadigm bare = pack.LSpeechPackParadigms[1];
        Assert.DoesNotContain(bare.LParadigmCells, cell => cell.Count == 1);
        Assert.Equal([20L, 3L], Assert.Single(bare.LParadigmCells));
        LParadigm noun = pack.LSpeechPackParadigms[2];
        Assert.Equal(2, noun.LParadigmCells.Count);
        Assert.Equal([2L], noun.LParadigmCells[0]);
        Assert.Equal([3L], noun.LParadigmCells[1]);
    }

    [Fact]
    public void SpeechPackLoad_CellsMalformed_SkipsRow()
    {
        using TLanguageFixture fixture = TLanguageFixture.TLanguageFixtureCreate("""{ "listed": false }""");
        fixture.TLanguageFixtureSave("vocabulary.json", """
            {
              "paradigms": [
                { "part": 1, "cells": [[2, "plural"]] },
                { "part": 2, "cells": [[2, -1]] },
                { "part": 3, "cells": [2, 3] },
                { "part": 4, "cells": [[]] },
                { "part": 5, "values": [2], "cells": "x" },
                { "part": 12, "cells": [[11, 12]] }
              ]
            }
            """);

        LSpeechPack pack = TInterface.TSpeechPackLoad(fixture.TLanguageFixtureName);

        LParadigm kept = Assert.Single(pack.LSpeechPackParadigms);
        Assert.Equal(12, kept.LParadigmSpeechCode);
        Assert.Equal([11L, 12L], Assert.Single(kept.LParadigmCells));
    }

    [Fact]
    public void ParadigmSlotKey_ManyValues_JoinsCodesAscending()
    {
        LParadigmSlot slot = TParadigmCellCreate();

        Assert.Equal([10L, 14L, 17L, 20L, 24L], slot.LParadigmSlotCodes);
        Assert.Equal("10+14+17+20+24", slot.LParadigmSlotKey);
    }

    [Fact]
    public void ParadigmSlotName_ManyValues_JoinsNamesInCellOrder()
    {
        LParadigmSlot slot = TParadigmCellCreate();

        Assert.Equal("subjunctive imperfect -ra first singular", slot.LParadigmSlotName);
        Assert.Equal(
            "subjunctive imperfect -ra first singular",
            Assert.Single(TInterface.TParadigmRowScan([slot])).LParadigmRowName);
    }

    [Fact]
    public void ParadigmRowName_OneValue_KeepsTodaysName()
    {
        LSpeechValue noun = TInterface.TSpeechValueCreate("English", 1, "noun", 0) with { LSpeechValueId = 1 };
        LMorphology plural = TInterfaceInflection.TMorphologyCreate(1, 4, "plural", 0);
        LMorphology past = TInterfaceInflection.TMorphologyCreate(1, 5, "past", 1);
        LInflection wolves = TInterfaceInflection.TInflectionCreate(1, 0, "wolves", null, 1, [1]);

        LParadigmSlot first = TInterface.TParadigmSlotCreate(noun, plural, wolves, LState.LStateSpecified);
        LParadigmSlot second = TInterface.TParadigmSlotCreate(noun, past, wolves, LState.LStateSpecified);

        Assert.Equal([plural], first.LParadigmSlotMorphologies);
        Assert.Equal("4", first.LParadigmSlotKey);
        Assert.Equal("plural", first.LParadigmSlotName);
        Assert.Equal("plural, past", Assert.Single(TInterface.TParadigmRowScan([first, second])).LParadigmRowName);
    }

    private static LParadigmSlot TParadigmCellCreate()
    {
        LSpeechValue verb = TInterface.TSpeechValueCreate("Spanish", 6, "verb", 0) with { LSpeechValueId = 6 };
        LMorphology subjunctive = TInterfaceInflection.TMorphologyCreate(1, 10, "subjunctive", 0);
        LMorphology imperfect = TInterfaceInflection.TMorphologyCreate(2, 14, "imperfect", 0);
        LMorphology form = TInterfaceInflection.TMorphologyCreate(10, 24, "-ra", 0);
        LMorphology first = TInterfaceInflection.TMorphologyCreate(3, 17, "first", 0);
        LMorphology singular = TInterfaceInflection.TMorphologyCreate(4, 20, "singular", 0);
        return TInterface.TParadigmSlotCreate(verb, subjunctive, null, LState.LStateUnspecified) with
        {
            LParadigmSlotMorphologies = [subjunctive, imperfect, form, first, singular],
        };
    }
}
