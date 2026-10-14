namespace Llyn.Core;

public sealed record LDeck(
    long LDeckId,
    long? LDeckFolderId,
    int LDeckPosition,
    string LDeckName,
    LGrain LDeckGrain,
    long LDeckStencilId,
    LBearingLead LDeckLead,
    LDeckSchedule LDeckSchedule,
    string LDeckCode,
    bool LDeckBasic,
    string? LDeckSyncedUtc,
    string LDeckAddedUtc,
    string LDeckUpdatedUtc);
