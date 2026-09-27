namespace Llyn.Conduct;

public sealed record CCatalogSituation(
    long CCatalogSituationId,
    string CCatalogSituationTitle,
    int CCatalogSituationUsage,
    CStateValue CCatalogSituationKind,
    bool CCatalogSituationChosen);
