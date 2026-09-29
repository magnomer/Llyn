using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CMentionOffer(int CMentionOfferOffset, IReadOnlyList<CTranslationTarget> CMentionOfferEntry);
