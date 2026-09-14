using System;
using System.Collections.Generic;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PDisplay
{
    private void PDisplayScriptShow(long id, string language)
    {
        IReadOnlyList<LScriptImage> images;
        IReadOnlyList<LScriptStyle> styles;
        try
        {
            styles = _lEngine.LEngineStyleRead(language);
            images = styles.Count == 0 ? [] : _lEngine.LEngineScriptRead(id);
        }
        catch (Exception)
        {
            styles = [];
            images = [];
        }

        IReadOnlyList<PScriptItem> items = PScriptItem.PScriptItemScan(images, styles);
        if (items.Count == 0)
        {
            PDisplayScriptClear();
            return;
        }

        PFont.PFontApply(_lEngine, language, LFontRole.LFontRoleGlyph, PDisplayScript);
        PDisplayScript.ItemsSource = items;
        PDisplayScriptSection.Visibility = Visibility.Visible;
    }

    private void PDisplayScriptClear()
    {
        PDisplayScript.ItemsSource = null;
        PDisplayScriptSection.Visibility = Visibility.Collapsed;
    }
}
