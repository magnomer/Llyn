using System;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PStem : UserControl
{
    private LWindow _lWindow = null!;

    public PStem()
    {
        InitializeComponent();
    }

    internal Action<string?>? PStemEntryNotice { get; set; }

    internal void PStemAttach(LWindow window)
    {
        _lWindow = window;
    }

    internal void PStemShow(LStemPage page)
    {
        LFontFace.LFontApply(_lWindow, page.LStemPageLanguage, PStemHeadword);
        LFontFace.LFontPlace(PStemHeadword);
        LFontFace.LFontApply(_lWindow, page.LStemPageLanguage, LFontRole.LFontRoleGlyph, PStemList);
        PStemHeadword.Text = page.LStemPageKey;
        PStemLanguage.Text = page.LStemPageLanguage;
        PStemFlag.Source = LEnsignImage.LEnsignFind(page.LStemPageLanguage);
        PStemList.ItemsSource = page.LStemPageCharacters;
        PStemEmpty.Visibility = PLook.PLookVisibleRead(page.LStemPageEmpty);
    }

    private void PStemEntryHandle(object sender, ExecutedRoutedEventArgs e)
    {
        PStemEntryNotice?.Invoke(PSender.PSenderTextRead(e));
    }
}
