using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private void PDisplayParadigmShow(long id)
    {
        IReadOnlyList<LParadigmSlot> slots;
        bool enabled;
        bool pending;
        try
        {
            slots = _lLectern.LLecternParadigmShow(id);
            enabled = _lLectern.LLecternMorphologyRead();
            if (enabled)
            {
                _lLectern.LLecternInflectionStart(id);
            }

            pending = _lLectern.LLecternInflectionCheck(id);
        }
        catch (Exception)
        {
            slots = [];
            enabled = false;
            pending = false;
        }

        if (slots.Count == 0)
        {
            PDisplayParadigm.PParadigmItems = null;
            return;
        }

        LFontFace.LFontApply(
            _pDisplayHost.PWindowDeportment, slots[0].LParadigmSlotSpeech.LSpeechValueLanguage, PDisplayParadigm);
        PDisplayParadigm.PParadigmItems = PParadigmItem.PParadigmItemScan(slots, pending, enabled, false);
    }
}
