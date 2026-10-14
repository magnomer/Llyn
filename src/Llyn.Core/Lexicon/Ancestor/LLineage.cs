using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LLineage(
    long LLineageId,
    long? LLineageEntryId,
    string LLineageText,
    string LLineageLanguage,
    string LLineageGloss,
    long? LLineageRelationId,
    bool LLineageBorrowed,
    IReadOnlyList<LLineage> LLineageSources);
