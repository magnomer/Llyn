using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIDeportment;

public sealed class PEtymology : ContentControl
{
    public static readonly DependencyProperty PEtymologyEditableProperty = DependencyProperty.Register(
        nameof(PEtymologyEditable),
        typeof(bool),
        typeof(PEtymology),
        new FrameworkPropertyMetadata(false, PEtymologyStateHandle));

    public static readonly DependencyProperty PEtymologyTextProperty = DependencyProperty.Register(
        nameof(PEtymologyText),
        typeof(string),
        typeof(PEtymology),
        new FrameworkPropertyMetadata(string.Empty, PEtymologyStateHandle));

    public static readonly DependencyProperty PEtymologyLanguageProperty = DependencyProperty.Register(
        nameof(PEtymologyLanguage),
        typeof(string),
        typeof(PEtymology),
        new FrameworkPropertyMetadata(string.Empty, PEtymologyStateHandle));

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
        PLookItem.PLookItemAttach(_pEtymologyField, PEtymologyItemApply);

        _pEtymologyProse.SetResourceReference(StyleProperty, "Theme.Etymology.Text");
        _pEtymologyWrite.SetResourceReference(StyleProperty, "Theme.Etymology.Prose");
        _pEtymologyWrite.SetResourceReference(TagProperty, "Input.EtymologyHint");
        _pEtymologyWrite.SetResourceReference(ContextMenuProperty, "Theme.Etymology.Menu");

        _pEtymologyStrip.ItemsSource = _pEtymologyLine.PMentionLineChip;
        _pEtymologyStrip.SetResourceReference(StyleProperty, "Theme.Mention.Line");
        _pEtymologyStrip.SetResourceReference(ItemsControl.ItemTemplateProperty, "Theme.Mention.Chip");
        PLookItem.PLookItemAttach(_pEtymologyStrip, PMentionChip.PMentionChipApply);

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

    internal void PEtymologyShow(string language, string text, IReadOnlyList<LTranslationTarget> etymons)
    {
        PEtymologyLanguage = language;
        PEtymologyText = text;
        PEtymologySourceShow(etymons);
    }

    internal void PEtymologySourceShow(IReadOnlyList<LTranslationTarget> etymons)
    {
        ArgumentNullException.ThrowIfNull(etymons);

        _pEtymologyField.ItemsSource = PEtymon.PEtymonBuild(etymons, _pEtymologyCaret);
        _pEtymologyField.Visibility = PLook.PLookVisibleRead(
            LLectern.LLecternEtymonCheck(PEtymologyEditable, etymons.Count));
    }

    internal void PEtymologyMentionShow(
        LWindow window, string text, IReadOnlyList<LMentionDraft> mentions, string silent)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(mentions);
        ArgumentNullException.ThrowIfNull(silent);

        _pEtymologyLine.PMentionLineShow(window, text, mentions, silent);
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new PSurfacePeer(this);
    }

    private static void PEtymologyStateHandle(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        ((PEtymology)sender).PEtymologyStateApply();
    }

    private void PEtymologyStateApply()
    {
        bool editable = PEtymologyEditable;
        string text = PEtymologyText;

        _pEtymologyProse.PMentionText = text;
        _pEtymologyProse.PMentionLanguage = PEtymologyLanguage;

        PField.PFieldTextShow(_pEtymologyWrite, text);

        _pEtymologyCaret.PEtymonShown = editable;
        _pEtymologyProse.Visibility = PLook.PLookVisibleRead(LLectern.LLecternNarrativeCheck(editable, text));
        _pEtymologyWrite.Visibility = PLook.PLookVisibleRead(editable);
        _pEtymologyStrip.Visibility = PLook.PLookVisibleRead(editable);
        PLookItem.PLookItemApply(_pEtymologyField);
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
            descriptor.RemoveValueChanged(etymon, PEtymologyItemHandle);
            descriptor.AddValueChanged(etymon, PEtymologyItemHandle);
        }

        if (PLook.PLookPartFind<Border>(container, "PEtymonChip") is Border chip)
        {
            chip.Visibility = PLook.PLookVisibleRead(!etymon.PEtymonCaret);
        }

        if (PLook.PLookPartFind<Image>(container, "PEtymonFlag") is Image flag)
        {
            flag.Source = etymon.PEtymonFlag;
        }

        if (PLook.PLookPartFind<Button>(container, "PEtymonEntry") is Button entry)
        {
            entry.Command = PEtymologyCommand.PEtymologyCommandEntry;
            entry.CommandParameter = etymon.PEtymonId;
            if (PLook.PLookPartFind<TextBlock>(entry, "PEtymonHeadword") is TextBlock headword)
            {
                headword.Text = etymon.PEtymonHeadword;
            }

            if (PLook.PLookPartFind<TextBlock>(entry, "PEtymonLanguage") is TextBlock language)
            {
                language.Text = etymon.PEtymonLanguage;
            }
        }

        if (PLook.PLookPartFind<Button>(container, "PEtymonRemoval") is Button removal)
        {
            removal.Command = PEtymologyCommand.PEtymologyCommandRemoval;
            removal.CommandParameter = etymon;
            removal.Visibility = PLook.PLookVisibleRead(PEtymologyEditable);
            if (PLook.PLookPartFind<PIconImage>(removal, "PEtymonMark") is PIconImage mark)
            {
                mark.PIconSource = PIcon.PIconResolve("unlink", 12);
            }
        }

        if (PLook.PLookPartFind<TextBox>(container, "PEtymonBox") is not TextBox box)
        {
            return;
        }

        box.Visibility = PLook.PLookVisibleRead(etymon.PEtymonShown);
        box.Text = etymon.PEtymonText;

        box.TextChanged -= PEtymologyBoxHandle;
        box.TextChanged += PEtymologyBoxHandle;
        if (box.InputBindings.Count == 0)
        {
            box.InputBindings.Add(new KeyBinding(
                PEtymologyCommand.PEtymologyCommandAddition, Key.Return, ModifierKeys.None)
            {
                CommandParameter = etymon,
            });
        }
    }

    private void PEtymologyItemHandle(object? sender, EventArgs e)
    {
        PLookItem.PLookItemApply(_pEtymologyField);
    }

    private static void PEtymologyBoxHandle(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { DataContext: PEtymon etymon } box)
        {
            etymon.PEtymonText = box.Text;
        }
    }
}
