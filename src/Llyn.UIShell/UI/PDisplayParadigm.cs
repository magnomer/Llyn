using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PDisplay
{
    private void PDisplayParadigmShow(long id)
    {
        IReadOnlyList<LParadigmSlot> slots;
        bool enabled;
        bool pending;
        try
        {
            slots = _lEngine.LEngineParadigmShow(id);
            enabled = _lEngine.LEngineSettingsRead().LSettingsMorphology;
            if (enabled)
            {
                _lEngine.LEngineInflectionStart(id);
            }

            pending = _lEngine.LEngineInflectionCheck(id);
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

        PFont.PFontApply(_lEngine, slots[0].LParadigmSlotSpeech.LSpeechValueLanguage, PDisplayParadigm);
        PDisplayParadigm.PParadigmItems = PParadigmItem.PParadigmItemScan(slots, pending, enabled, false);
    }
}
