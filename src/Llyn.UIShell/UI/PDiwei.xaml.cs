using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PDiwei : UserControl
{
    private const string PDiweiKindInitial = "Yunjing.Shengmu";

    private const string PDiweiKindRime = "Yunjing.Yunmu";

    private LEngine _lEngine = null!;

    public PDiwei()
    {
        InitializeComponent();
    }

    internal Action<string>? PDiweiEntryNotice { get; set; }

    internal Action<bool>? PDiweiSwitchNotice { get; set; }

    internal void PDiweiAttach(LEngine engine)
    {
        _lEngine = engine;
    }

    internal void PDiweiApply(LDiwei diwei, IReadOnlyList<PDiweiItem> items)
    {
        ArgumentNullException.ThrowIfNull(diwei);
        ArgumentNullException.ThrowIfNull(items);


        PFont.PFontApply(_lEngine, diwei.LDiweiLanguage, PDiweiHeadword);
        PFont.PFontPlace(PDiweiHeadword);
        PFont.PFontApply(_lEngine, diwei.LDiweiLanguage, LFontRole.LFontRoleGlyph, PDiweiList);
        PDiweiHeadword.Text = diwei.LDiweiKey;
        PDiweiKind.SetResourceReference(
            TextBlock.TextProperty,
            diwei.LDiweiKind == LDiwei.LDiweiRime ? PDiweiKindRime : PDiweiKindInitial);
        PDiweiLanguage.Text = diwei.LDiweiLanguage;
        PDiweiFlag.Source = PEnsign.PEnsignFind(diwei.LDiweiLanguage);
        PDiweiList.ItemsSource = items;
        PDiweiEmpty.Visibility = items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    internal void PDiweiClear()
    {
        PDiweiList.ItemsSource = null;
        PDiweiEmpty.Visibility = Visibility.Collapsed;
    }

    private void PDiweiSwitchHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is bool respelled)
        {
            PDiweiSwitchNotice?.Invoke(respelled);
        }
    }

    private void PDiweiEntryHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is string character && character.Length > 0)
        {
            PDiweiEntryNotice?.Invoke(character);
        }
    }
}
