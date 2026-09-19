using System;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PDiwei : UserControl
{
    private LEngine _lEngine = null!;

    public PDiwei()
    {
        InitializeComponent();
    }

    internal Action<string?>? PDiweiEntryNotice { get; set; }

    internal Action<bool?>? PDiweiSwitchNotice { get; set; }

    internal void PDiweiAttach(LEngine engine)
    {
        _lEngine = engine;
    }

    internal void PDiweiShow(LDiweiPage page, string kind)
    {
        PFont.PFontApply(_lEngine, page.LDiweiPageLanguage, PDiweiHeadword);
        PFont.PFontPlace(PDiweiHeadword);
        PFont.PFontApply(_lEngine, page.LDiweiPageLanguage, LFontRole.LFontRoleGlyph, PDiweiList);
        PDiweiHeadword.Text = page.LDiweiPageKey;
        PDiweiKind.SetResourceReference(TextBlock.TextProperty, kind);
        PDiweiLanguage.Text = page.LDiweiPageLanguage;
        PDiweiFlag.Source = PEnsign.PEnsignFind(page.LDiweiPageLanguage);
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
