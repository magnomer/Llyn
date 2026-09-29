namespace Llyn.Conduct;

public sealed record CCatalogSituation(
    long CCatalogSituationId,
    string CCatalogSituationTitle,
    string CCatalogSituationCount,
    string CCatalogSituationKind,
    bool CCatalogSituationChosen);
