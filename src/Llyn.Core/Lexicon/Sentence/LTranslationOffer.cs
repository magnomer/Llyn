using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LTranslationOffer(
    string LTranslationOfferText,
    string LTranslationOfferWord,
    IReadOnlyList<LVistaRow> LTranslationOfferRows,
    bool LTranslationOfferShown,
    bool LTranslationOfferChosen,
    IReadOnlyList<string> LTranslationOfferLanguages);
