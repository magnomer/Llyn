namespace Llyn.Core;

public sealed record LCue(
    long LCueId,
    long LCueDeckId,
    int LCuePosition,
    LGrain LCueGrain,
    long? LCueEntryId,
    long? LCueMeaningId,
    long? LCueCollocationId,
    long? LCueExampleId,
    long? LCueTargetId,
    int LCueFlag,
    bool LCueExcluded,
    string LCueText1,
    string LCueText2,
    string LCueAddedUtc)
{
    public bool LCueOrphaned => LCueGrain switch
    {
        LGrain.LGrainFree => false,
        LGrain.LGrainEntry => LCueEntryId is null,
        LGrain.LGrainMeaning => LCueMeaningId is null,
        LGrain.LGrainCollocation => LCueCollocationId is null,
        LGrain.LGrainExample => LCueExampleId is null,
        LGrain.LGrainPair => LCueTargetId is null || (LCueMeaningId is null && LCueCollocationId is null),
        _ => true,
    };

    public bool LCueResting => LCueExcluded || LCueOrphaned;
}
