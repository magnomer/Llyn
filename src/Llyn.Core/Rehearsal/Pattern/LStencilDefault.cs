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

    public static string LStencilDefaultRead(LCueColumn column) =>
        column switch
        {
            LCueColumn.LCueColumnPicture => "{image}",
            LCueColumn.LCueColumnSound => "{recording}",
            _ => string.Empty,
        };

    private static IReadOnlyList<LStencilSlot> LStencilDefaultCreate(string prompt, string answer) =>
    [
        new LStencilSlot(LCueColumn.LCueColumnText, 1, prompt),
        new LStencilSlot(LCueColumn.LCueColumnText, 2, answer),
        new LStencilSlot(LCueColumn.LCueColumnPicture, 2, LStencilDefaultRead(LCueColumn.LCueColumnPicture)),
        new LStencilSlot(LCueColumn.LCueColumnSound, 2, LStencilDefaultRead(LCueColumn.LCueColumnSound)),
    ];
}
