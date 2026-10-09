namespace Llyn.Core;

public sealed record LRigContext(
    LRegisterVault LRigContextRegisters,
    LSituationVault LRigContextSituations,
    LTagVault LRigContextTags);
