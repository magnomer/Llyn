using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CMentionOffer(
    int? CMentionOfferUnit, IReadOnlyList<CTranslationTarget> CMentionOfferEntry, string CMentionOfferKey);
