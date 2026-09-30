using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class PEtymology : ContentControl
{
    public static readonly DependencyProperty PEtymologyEditableProperty = DependencyProperty.Register(
        nameof(PEtymologyEditable),
        typeof(bool),
        typeof(PEtymology),
        new FrameworkPropertyMetadata(false, PEtymologyStateRefine));

    public static readonly DependencyProperty PEtymologyTextProperty = DependencyProperty.Register(
        nameof(PEtymologyText),
        typeof(string),
        typeof(PEtymology),
        new FrameworkPropertyMetadata(string.Empty, PEtymologyStateRefine));

    public static readonly DependencyProperty PEtymologyLanguageProperty = DependencyProperty.Register(
        nameof(PEtymologyLanguage),
        typeof(string),
        typeof(PEtymology),
        new FrameworkPropertyMetadata(string.Empty, PEtymologyStateRefine));

    private readonly StackPanel _pEtymologyBody = new();
    private readonly ItemsControl _pEtymologyField = new();
    private readonly ItemsControl _pEtymologyStrip = new();
    private readonly PMention _pEtymologyProse = new();
    private readonly TextBox _pEtymologyWrite = new();
    private readonly PMentionLine _pEtymologyLine = new();
    private readonly PEtymon _pEtymologyCaret = new();

    public PEtymology()
    {
        Focusable = false;
        IsTabStop = false;

        _pEtymologyField.ItemsSource = new List<PEtymon> { _pEtymologyCaret };
        _pEtymologyField.SetResourceReference(StyleProperty, "Theme.Etymology.Field");
        _pEtymologyField.SetResourceReference(
            ItemsControl.ItemTemplateProperty, "Theme.Etymology.Item");
        QLookItem.QLookItemAttach(_pEtymologyField, PEtymologyItemApply);

        _pEtymologyProse.SetResourceReference(StyleProperty, "Theme.Etymology.Text");
        _pEtymologyWrite.SetResourceReference(StyleProperty, "Theme.Etymology.Prose");
        _pEtymologyWrite.SetResourceReference(QField.QFieldHintProperty, "Input.EtymologyHint");
        _pEtymologyWrite.SetResourceReference(ContextMenuProperty, "Theme.Etymology.Menu");

        _pEtymologyStrip.ItemsSource = _pEtymologyLine.PMentionLineChip;
        _pEtymologyStrip.SetResourceReference(StyleProperty, "Theme.Mention.Line");
        _pEtymologyStrip.SetResourceReference(ItemsControl.ItemTemplateProperty, "Theme.Mention.Chip");
        QLookItem.QLookItemAttach(_pEtymologyStrip, PMentionChip.PMentionChipApply);

        _pEtymologyBody.Children.Add(_pEtymologyField);
        _pEtymologyBody.Children.Add(_pEtymologyProse);
        _pEtymologyBody.Children.Add(_pEtymologyWrite);
        _pEtymologyBody.Children.Add(_pEtymologyStrip);
        Content = _pEtymologyBody;

        PEtymologyStateApply();
    }

    public bool PEtymologyEditable
    {
        get => (bool)GetValue(PEtymologyEditableProperty);
        set => SetValue(PEtymologyEditableProperty, value);
    }

    public string PEtymologyText
    {
        get => (string)GetValue(PEtymologyTextProperty);
        set => SetValue(PEtymologyTextProperty, value);
    }

    public string PEtymologyLanguage
    {
        get => (string)GetValue(PEtymologyLanguageProperty);
        set => SetValue(PEtymologyLanguageProperty, value);
    }

    internal TextBox PEtymologyBox => _pEtymologyWrite;

    internal void PEtymologySourceShow(IReadOnlyList<PEtymon> etymons)
    {
        ArgumentNullException.ThrowIfNull(etymons);

        _pEtymologyField.ItemsSource = new List<PEtymon>(etymons) { _pEtymologyCaret };
        _pEtymologyField.Visibility = QLook.QLookVisibleRead(
            CDisplay.CDisplayEtymonCheck(PEtymologyEditable, etymons.Count));
    }

    internal void PEtymologyMentionShow(
        PWindow host, string text, IReadOnlyList<CMentionDraft> mentions, string silent)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(mentions);
        ArgumentNullException.ThrowIfNull(silent);

        _pEtymologyLine.PMentionLineShow(host.PWindowAtelier, text, mentions, silent);
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new QSurfacePeer(this);
    }

    private static void PEtymologyStateRefine(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        ((PEtymology)sender).PEtymologyStateApply();
    }

    private void PEtymologyStateApply()
    {
        bool editable = PEtymologyEditable;
        string text = PEtymologyText;

        _pEtymologyProse.PMentionText = text;
        _pEtymologyProse.PMentionLanguage = PEtymologyLanguage;

        QField.QFieldTextShow(_pEtymologyWrite, text);

        _pEtymologyCaret.PEtymonShown = editable;
        _pEtymologyProse.Visibility = QLook.QLookVisibleRead(CDisplay.CDisplayNarrativeCheck(editable, text));
        _pEtymologyWrite.Visibility = QLook.QLookVisibleRead(editable);
        _pEtymologyStrip.Visibility = QLook.QLookVisibleRead(editable);
        QLookItem.QLookItemApply(_pEtymologyField);
    }

    private void PEtymologyItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PEtymon etymon)
        {
            return;
        }

        foreach (DependencyProperty watched in new[] { PEtymon.PEtymonTextProperty, PEtymon.PEtymonShownProperty })
        {
            DependencyPropertyDescriptor descriptor =
                DependencyPropertyDescriptor.FromProperty(watched, typeof(PEtymon));
            descriptor.RemoveValueChanged(etymon, PEtymologyItemRefine);
            descriptor.AddValueChanged(etymon, PEtymologyItemRefine);
        }

        if (QLook.QLookPartFind<Border>(container, "PEtymonChip") is Border chip)
        {
            chip.Visibility = QLook.QLookVisibleRead(!etymon.PEtymonCaret);
        }

        if (QLook.QLookPartFind<Image>(container, "PEtymonFlag") is Image flag)
        {
            flag.Source = etymon.PEtymonFlag;
        }

        if (QLook.QLookPartFind<Button>(container, "PEtymonEntry") is Button entry)
        {
            entry.Command = PEtymologyCommand.PEtymologyCommandEntry;
            entry.CommandParameter = etymon.PEtymonId;
            if (QLook.QLookPartFind<TextBlock>(entry, "PEtymonHeadword") is TextBlock headword)
            {
                headword.Text = etymon.PEtymonHeadword;
            }

            if (QLook.QLookPartFind<TextBlock>(entry, "PEtymonLanguage") is TextBlock language)
            {
                language.Text = etymon.PEtymonLanguage;
            }
        }

        if (QLook.QLookPartFind<Button>(container, "PEtymonRemoval") is Button removal)
        {
            removal.Command = PEtymologyCommand.PEtymologyCommandRemoval;
            removal.CommandParameter = etymon;
            removal.Visibility = QLook.QLookVisibleRead(PEtymologyEditable);
            if (QLook.QLookPartFind<QIconImage>(removal, "PEtymonMark") is QIconImage mark)
            {
                mark.QIconSource = QIcon.QIconResolve("unlink", 12);
            }
        }

        if (QLook.QLookPartFind<TextBox>(container, "PEtymonBox") is not TextBox box)
        {
            return;
        }

        box.Visibility = QLook.QLookVisibleRead(etymon.PEtymonShown);
        box.Text = etymon.PEtymonText;
        box.SetResourceReference(QField.QFieldHintProperty, "Input.EtymonHint");

        box.TextChanged -= PEtymologyBoxRefine;
        box.TextChanged += PEtymologyBoxRefine;
        if (box.InputBindings.Count == 0)
        {
            box.InputBindings.Add(new KeyBinding(
                PEtymologyCommand.PEtymologyCommandAddition, Key.Return, ModifierKeys.None)
            {
                CommandParameter = etymon,
            });
        }
    }

    private void PEtymologyItemRefine(object? sender, EventArgs e)
    {
        QLookItem.QLookItemApply(_pEtymologyField);
    }

    private static void PEtymologyBoxRefine(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { DataContext: PEtymon etymon } box)
        {
            etymon.PEtymonText = box.Text;
        }
    }
}
