using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LSituationOffer(
    string LSituationOfferText,
    IReadOnlyList<LSituationRow> LSituationOfferRows,
    bool LSituationOfferShown);
