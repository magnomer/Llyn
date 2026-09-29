using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LSpeechOffer(
    IReadOnlyList<LSpeechRow> LSpeechOfferRows,
    bool LSpeechOfferDeclared,
    bool LSpeechOfferMatched,
    bool LSpeechOfferShown);
