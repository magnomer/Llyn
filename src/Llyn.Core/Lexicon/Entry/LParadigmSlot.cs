using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Llyn.Core;

public sealed record LParadigmSlot(
    LSpeechValue LParadigmSlotSpeech,
    LMorphology LParadigmSlotMorphology,
    LInflection? LParadigmSlotInflection,
    LState LParadigmSlotState,
    LParadigm LParadigmSlotParadigm,
    IReadOnlyList<LMorphology>? LParadigmSlotMorphologies = null)
{
    public IReadOnlyList<LMorphology> LParadigmSlotMorphologies { get; init; } =
        LParadigmSlotMorphologies ?? [LParadigmSlotMorphology];

    public bool LParadigmSlotUncertain => LParadigmSlotState == LState.LStateUnknown;

    public IReadOnlyList<long> LParadigmSlotCodes =>
        LParadigmSlotMorphologies.Select(static morphology => morphology.LMorphologyCode).Order().ToList();

    public string LParadigmSlotKey =>
        string.Join("+", LParadigmSlotCodes.Select(static code => code.ToString(CultureInfo.InvariantCulture)));

    public string LParadigmSlotName =>
        string.Join(" ", LParadigmSlotMorphologies.Select(static morphology => morphology.LMorphologyName));

    public LParadigmStatus LParadigmSlotCheck(bool pending, bool enabled)
    {
        if (LParadigmSlotInflection is LInflection inflection)
        {
            return inflection.LInflectionText.Length == 0
                ? LParadigmStatus.LParadigmStatusAbsent
                : LParadigmStatus.LParadigmStatusText;
        }

        if (LParadigmSlotUncertain)
        {
            return LParadigmStatus.LParadigmStatusUnknown;
        }

        if (pending)
        {
            return LParadigmStatus.LParadigmStatusPending;
        }

        return enabled ? LParadigmStatus.LParadigmStatusLost : LParadigmStatus.LParadigmStatusAbsent;
    }

    public bool LParadigmSlotMatch(LParadigmSlot other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return LParadigmSlotSpeech.LSpeechValueId == other.LParadigmSlotSpeech.LSpeechValueId
            && LParadigmSlotInflection is LInflection first
            && other.LParadigmSlotInflection is LInflection second
            && string.Equals(first.LInflectionText, second.LInflectionText, StringComparison.Ordinal);
    }
}
