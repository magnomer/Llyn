using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LRegisterOffer(
    string LRegisterOfferText,
    IReadOnlyList<LRegisterRow> LRegisterOfferRows,
    bool LRegisterOfferShown);
