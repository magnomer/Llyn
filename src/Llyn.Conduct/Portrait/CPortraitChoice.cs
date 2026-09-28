namespace Llyn.Conduct;

public sealed record CPortraitChoice(
    string CPortraitChoiceKey,
    string CPortraitChoiceSuffix,
    bool CPortraitChoiceChosen,
    CPortraitMedium CPortraitChoiceMedium);
