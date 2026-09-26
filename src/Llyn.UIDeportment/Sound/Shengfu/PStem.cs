using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIDeportment;

public class PStem : UserControl
{
    private LWindow _lWindow = null!;

    public PStem()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Sound/Shengfu/PStem.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));

        CommandBindings.Add(new CommandBinding(PStemCommand.PStemCommandEntry, PStemEntryHandle));
    }

    private TextBlock PStemHeadword => (TextBlock)FindName(nameof(PStemHeadword));

    private Image PStemFlag => (Image)FindName(nameof(PStemFlag));

    private TextBlock PStemLanguage => (TextBlock)FindName(nameof(PStemLanguage));

    private TextBlock PStemEmpty => (TextBlock)FindName(nameof(PStemEmpty));

    private ItemsControl PStemList => (ItemsControl)FindName(nameof(PStemList));

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
