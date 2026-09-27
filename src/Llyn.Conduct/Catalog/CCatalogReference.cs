namespace Llyn.Conduct;

public sealed record CCatalogReference(
    long CCatalogReferenceId,
    string CCatalogReferenceName,
    string CCatalogReferenceByline,
    CStateValue CCatalogReferenceCredit,
    CStateValue CCatalogReferenceYear,
    int CCatalogReferenceUsage,
    bool CCatalogReferenceChosen);
