using System.Collections.Generic;

namespace Llyn.Core;

public static class LStencilDefault
{
    public static IReadOnlyList<LStencilSlot> LStencilDefaultRead(LGrain grain) =>
        grain switch
        {
            LGrain.LGrainEntry => LStencilDefaultCreate("{headword}", "{definition:all}"),
            LGrain.LGrainMeaning => LStencilDefaultCreate("{headword}", "{definition}"),
            LGrain.LGrainCollocation => LStencilDefaultCreate("{expression}", "{paraphrase}"),
            LGrain.LGrainExample => LStencilDefaultCreate("{sentence}", "{gloss}"),
            LGrain.LGrainPair => LStencilDefaultCreate("{headword}", "{target}"),
            _ => [],
        };

    private static IReadOnlyList<LStencilSlot> LStencilDefaultCreate(string prompt, string answer) =>
    [
        new LStencilSlot(LCueColumn.LCueColumnText, 1, prompt),
        new LStencilSlot(LCueColumn.LCueColumnText, 2, answer),
        new LStencilSlot(LCueColumn.LCueColumnPicture, 2, "{image}"),
        new LStencilSlot(LCueColumn.LCueColumnSound, 2, "{recording}"),
    ];
}
