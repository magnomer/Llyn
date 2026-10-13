namespace Llyn.Conduct;

public sealed record CHarvestStep(
    string CHarvestStepSource,
    int CHarvestStepOrder,
    CRecording? CHarvestStepRecording,
    bool CHarvestStepEnded);
