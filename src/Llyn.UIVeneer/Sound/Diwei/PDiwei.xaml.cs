using System;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PDiwei : UserControl
{
    private LWindow _lWindow = null!;

    public PDiwei()
    {
        InitializeComponent();
    }

    internal Action<string?>? PDiweiEntryNotice { get; set; }

    internal Action<bool?>? PDiweiSwitchNotice { get; set; }

    internal void PDiweiAttach(LWindow window)
    {
        _lWindow = window;
    }

    internal void PDiweiShow(LDiweiPage page, string kind)
    {
        LFontFace.LFontApply(_lWindow, page.LDiweiPageLanguage, PDiweiHeadword);
        LFontFace.LFontPlace(PDiweiHeadword);
        LFontFace.LFontApply(_lWindow, page.LDiweiPageLanguage, LFontRole.LFontRoleGlyph, PDiweiList);
        PDiweiHeadword.Text = page.LDiweiPageKey;
        PDiweiKind.SetResourceReference(TextBlock.TextProperty, kind);
        PDiweiLanguage.Text = page.LDiweiPageLanguage;
        PDiweiFlag.Source = LEnsignImage.LEnsignFind(page.LDiweiPageLanguage);
        PDiweiList.ItemsSource = PDiweiItem.PDiweiItemBuild(page.LDiweiPageSections);
        PDiweiEmpty.Visibility = PLook.PLookVisibleRead(page.LDiweiPageEmpty);
    }

    private void PDiweiSwitchHandle(object sender, ExecutedRoutedEventArgs e)
    {
        PDiweiSwitchNotice?.Invoke(PSender.PSenderFlagRead(e));
    }

    private void PDiweiEntryHandle(object sender, ExecutedRoutedEventArgs e)
    {
        PDiweiEntryNotice?.Invoke(PSender.PSenderTextRead(e));
    }
}
