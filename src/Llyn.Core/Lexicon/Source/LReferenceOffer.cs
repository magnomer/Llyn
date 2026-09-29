using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LReferenceOffer(
    string LReferenceOfferText,
    IReadOnlyList<LReferenceRow> LReferenceOfferRows,
    bool LReferenceOfferShown);
