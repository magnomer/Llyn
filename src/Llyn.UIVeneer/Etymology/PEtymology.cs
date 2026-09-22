using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    private readonly ObservableCollection<object> _pEtymologyItem = [];
    private readonly PMention _pEtymologyProse = new();
    private readonly TextBox _pEtymologyWrite = new();
    private readonly PMentionLine _pEtymologyLine = new();
    private readonly PEtymologyCaret _pEtymologyCaret = new();

    private IReadOnlyList<PEtymologyChip> _pEtymologySource = [];

    private IReadOnlyList<LMentionDraft> _pEtymologyMention = [];

    public PEtymology()
    {
        Focusable = false;
        IsTabStop = false;

        _pEtymologyField.ItemsSource = _pEtymologyItem;
        _pEtymologyField.SetResourceReference(StyleProperty, "Theme.Etymology.Field");
        _pEtymologyField.SetResourceReference(
            ItemsControl.ItemTemplateSelectorProperty, "Theme.Etymology.Item");

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
        get => (string?)GetValue(PEtymologyTextProperty) ?? string.Empty;
        set => SetValue(PEtymologyTextProperty, value);
    }

    public string PEtymologyLanguage
    {
        get => (string?)GetValue(PEtymologyLanguageProperty) ?? string.Empty;
        set => SetValue(PEtymologyLanguageProperty, value);
    }

    internal TextBox PEtymologyBox => _pEtymologyWrite;

    internal PEtymologyCaret PEtymologyEntry => _pEtymologyCaret;

    internal void PEtymologySourceShow(IReadOnlyList<PEtymologyChip> chips)
    {
        ArgumentNullException.ThrowIfNull(chips);

        _pEtymologySource = chips;
        PEtymologyStateApply();
    }

    internal void PEtymologyMentionShow(IReadOnlyList<LMentionDraft> mentions, string silent)
    {
        ArgumentNullException.ThrowIfNull(mentions);
        ArgumentNullException.ThrowIfNull(silent);

        _pEtymologyMention = mentions;
        if (PMention.PMentionWindowRead(this) is LWindow window)
        {
            _pEtymologyLine.PMentionLineShow(window, PEtymologyText, mentions, silent);
        }
        else
        {
            _pEtymologyLine.PMentionLineClear();
        }

        PEtymologyStateApply();
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new PSurfacePeer(this);
    }

    private static void PEtymologyStateHandle(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is PEtymology etymology)
        {
            etymology.PEtymologyStateApply();
        }
    }

    private void PEtymologyStateApply()
    {
        bool editable = PEtymologyEditable;
        string text = PEtymologyText;
        bool narrated = text.Trim().Length > 0;

        PEtymologyItemApply(editable);

        List<LMention> shown = new(_pEtymologyMention.Count);
        foreach (LMentionDraft mention in _pEtymologyMention)
        {
            shown.Add(mention.LMentionDraftResolve());
        }

        _pEtymologyProse.PMentionText = text;
        _pEtymologyProse.PMentionMention = shown;
        _pEtymologyProse.PMentionLanguage = PEtymologyLanguage;

        PField.PFieldTextShow(_pEtymologyWrite, text);

        _pEtymologyProse.Visibility = !editable && narrated ? Visibility.Visible : Visibility.Collapsed;
        _pEtymologyWrite.Visibility = editable ? Visibility.Visible : Visibility.Collapsed;
        _pEtymologyStrip.Visibility = editable ? Visibility.Visible : Visibility.Collapsed;
        _pEtymologyField.Visibility = editable || _pEtymologySource.Count > 0
            ? Visibility.Visible
            : Visibility.Collapsed;
        if (!editable)
        {
            Visibility = narrated || _pEtymologySource.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void PEtymologyItemApply(bool editable)
    {
        _pEtymologyItem.Clear();
        foreach (PEtymologyChip chip in _pEtymologySource)
        {
            _pEtymologyItem.Add(chip);
        }

        if (editable)
        {
            _pEtymologyItem.Add(_pEtymologyCaret);
        }
    }
}
