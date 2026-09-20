namespace Llyn.Core;

public sealed record LLookupStep(
    LLookupKind LLookupStepKind,
    string LLookupStepSource,
    int LLookupStepOrder,
    LCandidate? LLookupStepCandidate)
{
    public bool LLookupStepEnded => LLookupStepKind == LLookupKind.LLookupKindEnd;
}
