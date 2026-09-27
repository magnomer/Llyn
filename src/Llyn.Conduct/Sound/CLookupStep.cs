namespace Llyn.Conduct;

public sealed record CLookupStep(
    string CLookupStepSource,
    int CLookupStepOrder,
    CCandidate? CLookupStepCandidate,
    bool CLookupStepEnded);
