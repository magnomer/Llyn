namespace Llyn.Conduct;

public sealed record CCatalogRegister(
    CRegister CCatalogRegisterStored,
    int CCatalogRegisterUsage,
    bool CCatalogRegisterChosen);
