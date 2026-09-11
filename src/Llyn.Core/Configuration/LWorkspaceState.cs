namespace Llyn.Core;

public sealed record LWorkspaceState(
    long LWorkspaceStateId,
    long? LWorkspaceStateLeftEntryId = null,
    long? LWorkspaceStateRightEntryId = null,
    long? LWorkspaceStateRevisionId = null,
    long LWorkspaceStateIdentityFloor = 0,
    string? LWorkspaceStateMode = null,
    bool LWorkspaceStateSplit = false,
    LCatalogOrder LWorkspaceStateOrder = LCatalogOrder.LCatalogOrderHeadword,
    LCatalogOrder LWorkspaceStateSequence = LCatalogOrder.LCatalogOrderHeadword,
    LCatalogOrder LWorkspaceStateSeries = LCatalogOrder.LCatalogOrderHeadword,
    LCatalogOrder LWorkspaceStateFunnel = LCatalogOrder.LCatalogOrderName,
    LCatalogOrder LWorkspaceStateTier = LCatalogOrder.LCatalogOrderName,
    LCatalogOrder LWorkspaceStateGrade = LCatalogOrder.LCatalogOrderName,
    LCatalogOrder LWorkspaceStateRank = LCatalogOrder.LCatalogOrderText,
    LCatalogOrder LWorkspaceStateDegree = LCatalogOrder.LCatalogOrderName);
