namespace Llyn.Core;

public sealed record LWorkspaceState(
    string LWorkspaceStateId,
    long? LWorkspaceStateLeft = null,
    long? LWorkspaceStateRight = null,
    string? LWorkspaceStateMode = null,
    bool LWorkspaceStateSplit = false,
    long? LWorkspaceStateRevision = null,
    LCatalogOrder LWorkspaceStateOrder = LCatalogOrder.LCatalogOrderHeadword,
    LCatalogOrder LWorkspaceStateSequence = LCatalogOrder.LCatalogOrderHeadword,
    LCatalogOrder LWorkspaceStateSeries = LCatalogOrder.LCatalogOrderHeadword,
    LCatalogOrder LWorkspaceStateFunnel = LCatalogOrder.LCatalogOrderName,
    LCatalogOrder LWorkspaceStateTier = LCatalogOrder.LCatalogOrderName,
    LCatalogOrder LWorkspaceStateGrade = LCatalogOrder.LCatalogOrderName,
    LCatalogOrder LWorkspaceStateRank = LCatalogOrder.LCatalogOrderText,
    LCatalogOrder LWorkspaceStateDegree = LCatalogOrder.LCatalogOrderName);
