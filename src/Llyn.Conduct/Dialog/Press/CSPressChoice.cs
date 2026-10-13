namespace Llyn.Conduct;

public sealed record CSPressChoice<CSPressChoiceKind>(
    CSPressChoiceKind CSPressChoiceValue,
    string CSPressChoiceKey,
    bool CSPressChoiceChosen);
