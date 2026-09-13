using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    internal void PEditorParadigmShow()
    {
        long? entry = PEditorEntryRead();
        if (entry is null)
        {
            PEditorParadigm.PParadigmItems = null;
            return;
        }

        IReadOnlyList<LParadigmSlot> slots;
        bool enabled;
        bool pending;
        try
        {
            slots = _lEngine.LEngineParadigmShow(entry.Value);
            enabled = _lEngine.LEngineSettingsRead().LSettingsMorphology;
            pending = _lEngine.LEngineInflectionCheck(entry.Value);
        }
        catch (Exception)
        {
            slots = [];
            enabled = false;
            pending = false;
        }

        if (slots.Count == 0)
        {
            PEditorParadigm.PParadigmItems = null;
            return;
        }

        PFont.PFontApply(_lEngine, slots[0].LParadigmSlotSpeech.LSpeechValueLanguage, PEditorParadigm);
        PEditorParadigm.PParadigmItems = PParadigmItem.PParadigmItemScan(slots, pending, enabled, true);
    }
}
