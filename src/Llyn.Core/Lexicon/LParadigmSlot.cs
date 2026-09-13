namespace Llyn.Core;

public sealed record LParadigmSlot(
    LSpeechValue LParadigmSlotSpeech,
    LMorphology LParadigmSlotMorphology,
    LInflection? LParadigmSlotInflection,
    LState LParadigmSlotState,
    LParadigm LParadigmSlotParadigm);
