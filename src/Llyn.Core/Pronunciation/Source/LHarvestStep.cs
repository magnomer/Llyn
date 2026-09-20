namespace Llyn.Core;

public sealed record LHarvestStep(
    LHarvestKind LHarvestStepKind,
    string LHarvestStepSource,
    int LHarvestStepOrder,
    LRecording? LHarvestStepRecording)
{
    public bool LHarvestStepEnded => LHarvestStepKind == LHarvestKind.LHarvestKindEnd;
}
