namespace Llyn.Conduct;

public sealed record CCatalogAuthor(
    long CCatalogAuthorId,
    string CCatalogAuthorName,
    string CCatalogAuthorWork,
    int CCatalogAuthorUsage,
    bool CCatalogAuthorStored,
    bool CCatalogAuthorChosen);
