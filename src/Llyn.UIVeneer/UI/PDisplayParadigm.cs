using System;
using System.Collections.Generic;
using Llyn.Core;

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
            slots = _lDisplay.LDisplayParadigmShow(id);
            enabled = _lDisplay.LDisplayMorphologyRead();
            if (enabled)
            {
                _lDisplay.LDisplayInflectionStart(id);
            }

            pending = _lDisplay.LDisplayInflectionCheck(id);
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

        PFont.PFontApply(
            _pDisplayHost.PWindowDeportment, slots[0].LParadigmSlotSpeech.LSpeechValueLanguage, PDisplayParadigm);
        PDisplayParadigm.PParadigmItems = PParadigmItem.PParadigmItemScan(slots, pending, enabled, false);
    }
}
