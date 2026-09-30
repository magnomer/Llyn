namespace Llyn.Conduct;

public sealed record CCatalogReference(
    long CCatalogReferenceId,
    string CCatalogReferenceName,
    string CCatalogReferenceByline,
    CStateWording CCatalogReferenceCredit,
    CStateWording CCatalogReferenceYear,
    int CCatalogReferenceUsage,
    bool CCatalogReferenceChosen);
