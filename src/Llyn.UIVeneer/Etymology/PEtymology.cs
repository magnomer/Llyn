using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

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

        _pEtymologyProse.SetResourceReference(StyleProperty, "Theme.Etymology.Text");
        _pEtymologyWrite.SetResourceReference(StyleProperty, "Theme.Etymology.Prose");
        _pEtymologyWrite.SetResourceReference(TagProperty, "Input.EtymologyHint");
        _pEtymologyWrite.SetResourceReference(ContextMenuProperty, "Theme.Etymology.Menu");

        _pEtymologyStrip.ItemsSource = _pEtymologyLine.PMentionLineChip;
        _pEtymologyStrip.SetResourceReference(StyleProperty, "Theme.Mention.Line");
        _pEtymologyStrip.SetResourceReference(ItemsControl.ItemTemplateProperty, "Theme.Mention.Chip");

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
    }
}
