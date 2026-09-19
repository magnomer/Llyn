using System;

namespace Llyn.Core;

public sealed record LParadigmSlot(
    LSpeechValue LParadigmSlotSpeech,
    LMorphology LParadigmSlotMorphology,
    LInflection? LParadigmSlotInflection,
    LState LParadigmSlotState,
    LParadigm LParadigmSlotParadigm)
{
    public bool LParadigmSlotUncertain => LParadigmSlotState == LState.LStateUnknown;


    public bool LParadigmSlotMatch(LParadigmSlot other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return LParadigmSlotSpeech.LSpeechValueId == other.LParadigmSlotSpeech.LSpeechValueId
            && LParadigmSlotInflection is LInflection first
            && other.LParadigmSlotInflection is LInflection second
            && string.Equals(first.LInflectionText, second.LInflectionText, StringComparison.Ordinal);
    }
}
