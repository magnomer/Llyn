using System.Collections.Generic;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TStencilDefault
{
    [Theory]
    [InlineData(LGrain.LGrainEntry, "{headword}", "{definition:all}")]
    [InlineData(LGrain.LGrainMeaning, "{headword}", "{definition}")]
    [InlineData(LGrain.LGrainCollocation, "{expression}", "{paraphrase}")]
    [InlineData(LGrain.LGrainExample, "{sentence}", "{gloss}")]
    [InlineData(LGrain.LGrainPair, "{headword}", "{target}")]
    public void StencilDefaultRead_EachGrain_FillsBothSides(LGrain grain, string prompt, string answer)
    {
        IReadOnlyList<LStencilSlot> slots = TInterface.TStencilDefaultRead(grain);

        Assert.Equal(
            [
                TInterface.TStencilSlotCreate(LCueColumn.LCueColumnText, 1, prompt),
                TInterface.TStencilSlotCreate(LCueColumn.LCueColumnText, 2, answer),
                TInterface.TStencilSlotCreate(LCueColumn.LCueColumnPicture, 2, "{image}"),
                TInterface.TStencilSlotCreate(LCueColumn.LCueColumnSound, 2, "{recording}"),
            ],
            slots);
    }

    [Fact]
    public void StencilDefaultRead_FreeGrain_ReturnsNoSlots()
    {
        Assert.Empty(TInterface.TStencilDefaultRead(LGrain.LGrainFree));
    }
}
