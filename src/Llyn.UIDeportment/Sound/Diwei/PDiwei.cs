using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIDeportment;

public class PDiwei : UserControl
{
    private LWindow _lWindow = null!;

    public PDiwei()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Sound/Diwei/PDiwei.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));

        CommandBindings.Add(new CommandBinding(PDiweiCommand.PDiweiCommandEntry, PDiweiEntryHandle));
        CommandBindings.Add(new CommandBinding(PDiweiCommand.PDiweiCommandSwitch, PDiweiSwitchHandle));

        PLookItem.PLookItemAttach(PDiweiList, PDiweiItem.PDiweiItemApply);
    }

    private TextBlock PDiweiHeadword => (TextBlock)FindName(nameof(PDiweiHeadword));

    private TextBlock PDiweiKind => (TextBlock)FindName(nameof(PDiweiKind));

    private Image PDiweiFlag => (Image)FindName(nameof(PDiweiFlag));

    private TextBlock PDiweiLanguage => (TextBlock)FindName(nameof(PDiweiLanguage));

    private TextBlock PDiweiEmpty => (TextBlock)FindName(nameof(PDiweiEmpty));

    private ItemsControl PDiweiList => (ItemsControl)FindName(nameof(PDiweiList));

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
