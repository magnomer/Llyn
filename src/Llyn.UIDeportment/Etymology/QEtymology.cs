using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public sealed class QEtymology : Decorator
{
    public static readonly DependencyProperty QEtymologyEditableProperty = DependencyProperty.Register(
        nameof(QEtymologyEditable),
        typeof(bool),
        typeof(QEtymology),
        new FrameworkPropertyMetadata(false, QEtymologyStateRefine));

    public static readonly DependencyProperty QEtymologyTextProperty = DependencyProperty.Register(
        nameof(QEtymologyText),
        typeof(string),
        typeof(QEtymology),
        new FrameworkPropertyMetadata(string.Empty, QEtymologyStateRefine));

    public static readonly DependencyProperty QEtymologyNarratedProperty = DependencyProperty.Register(
        nameof(QEtymologyNarrated),
        typeof(bool),
        typeof(QEtymology),
        new FrameworkPropertyMetadata(false, QEtymologyStateRefine));

    public static readonly DependencyProperty QEtymologyCardProperty = DependencyProperty.Register(
        nameof(QEtymologyCard),
        typeof(bool),
        typeof(QEtymology),
        new FrameworkPropertyMetadata(false, QEtymologyStateRefine));

    private readonly Border _qEtymologyFrame = new();
    private readonly Border _qEtymologyHead = new();
    private readonly StackPanel _qEtymologyBody = new();
    private readonly ItemsControl _qEtymologyField = new();
    private readonly ItemsControl _qEtymologyStrip = new();
    private readonly PMention _qEtymologyProse = new();
    private readonly TextBox _qEtymologyWrite = new();
    private readonly PMentionLine _qEtymologyLine = new();
    private readonly PEtymon _qEtymologyCaret = new();

    public QEtymology()
    {
        _qEtymologyField.ItemsSource = new List<PEtymon> { _qEtymologyCaret };
        _qEtymologyField.SetResourceReference(StyleProperty, "Theme.Etymology.Field");
        _qEtymologyField.SetResourceReference(
            ItemsControl.ItemTemplateProperty, "Theme.Etymology.Item");
        QLookItem.QLookItemAttach(_qEtymologyField, QEtymologyItemApply);

        _qEtymologyProse.SetResourceReference(StyleProperty, "Theme.Etymology.Text");
        _qEtymologyWrite.SetResourceReference(StyleProperty, "Theme.Etymology.Prose");
        _qEtymologyWrite.SetResourceReference(QField.QFieldHintProperty, "Input.EtymologyHint");
        _qEtymologyWrite.SetResourceReference(ContextMenuProperty, "Theme.Etymology.Menu");

        _qEtymologyStrip.ItemsSource = _qEtymologyLine.PMentionLineChip;
        _qEtymologyStrip.SetResourceReference(StyleProperty, "Theme.Mention.Line");
        _qEtymologyStrip.SetResourceReference(ItemsControl.ItemTemplateProperty, "Theme.Mention.Chip");
        QLookItem.QLookItemAttach(_qEtymologyStrip, PMentionChip.PMentionChipRefine);

        _qEtymologyBody.Children.Add(_qEtymologyField);
        _qEtymologyBody.Children.Add(_qEtymologyProse);
        _qEtymologyBody.Children.Add(_qEtymologyWrite);
        _qEtymologyBody.Children.Add(_qEtymologyStrip);
        TextBlock title = new();
        title.SetResourceReference(StyleProperty, "Theme.Etymology.Title");
        title.SetResourceReference(TextBlock.TextProperty, "Display.Etymology");
        _qEtymologyHead.Child = title;
        _qEtymologyHead.SetResourceReference(StyleProperty, "Theme.Etymology.Head");

        Grid.SetRow(_qEtymologyBody, 1);
        Grid frame = new();
        frame.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        frame.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        frame.Children.Add(_qEtymologyHead);
        frame.Children.Add(_qEtymologyBody);
        _qEtymologyFrame.Child = frame;
        Child = _qEtymologyFrame;

        QEtymologyStateApply();
    }

    public bool QEtymologyEditable
    {
        get => (bool)GetValue(QEtymologyEditableProperty);
        set => SetValue(QEtymologyEditableProperty, value);
    }

    public string QEtymologyText
    {
        get => (string)GetValue(QEtymologyTextProperty);
        set => SetValue(QEtymologyTextProperty, value);
    }

    public bool QEtymologyCard
    {
        get => (bool)GetValue(QEtymologyCardProperty);
        set => SetValue(QEtymologyCardProperty, value);
    }

    public bool QEtymologyNarrated
    {
        get => (bool)GetValue(QEtymologyNarratedProperty);
        set => SetValue(QEtymologyNarratedProperty, value);
    }

    internal TextBox QEtymologyBox => _qEtymologyWrite;

    internal PMentionLine QEtymologyLine => _qEtymologyLine;

    internal void QEtymologySourceShow(IReadOnlyList<PEtymon> etymons, bool linked)
    {
        ArgumentNullException.ThrowIfNull(etymons);

        _qEtymologyField.ItemsSource = new List<PEtymon>(etymons) { _qEtymologyCaret };
        _qEtymologyField.Visibility = QLook.QLookVisibleRead(linked);
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new QVeilPeer(this);
    }

    private static void QEtymologyStateRefine(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        ((QEtymology)sender).QEtymologyStateApply();
    }

    private void QEtymologyStateApply()
    {
        bool editable = QEtymologyEditable;
        string text = QEtymologyText;

        _qEtymologyFrame.SetResourceReference(
            StyleProperty, QEtymologyCard ? "Theme.Etymology.Card" : "Theme.Etymology.Read");
        _qEtymologyHead.Visibility = QLook.QLookVisibleRead(QEtymologyCard);
        if (QEtymologyCard)
        {
            _qEtymologyBody.SetResourceReference(StyleProperty, "Theme.Etymology.Body");
        }
        else
        {
            _qEtymologyBody.ClearValue(StyleProperty);
        }

        _qEtymologyProse.PMentionPiece = QMentionPiece.QMentionPieceCreate(text);

        QField.QFieldTextShow(_qEtymologyWrite, text);

        _qEtymologyCaret.PEtymonShown = editable;
        _qEtymologyProse.Visibility = QLook.QLookVisibleRead(QEtymologyNarrated);
        _qEtymologyWrite.Visibility = QLook.QLookVisibleRead(editable);
        _qEtymologyStrip.Visibility = QLook.QLookVisibleRead(editable);
        QLookItem.QLookItemApply(_qEtymologyField);
    }

    private void QEtymologyItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PEtymon etymon)
        {
            return;
        }

        foreach (DependencyProperty watched in new[] { PEtymon.PEtymonTextProperty, PEtymon.PEtymonShownProperty })
        {
            DependencyPropertyDescriptor descriptor =
                DependencyPropertyDescriptor.FromProperty(watched, typeof(PEtymon));
            descriptor.RemoveValueChanged(etymon, QEtymologyItemRefine);
            descriptor.AddValueChanged(etymon, QEtymologyItemRefine);
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
            removal.Visibility = QLook.QLookVisibleRead(QEtymologyEditable);
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

        box.TextChanged -= QEtymologyBoxRefine;
        box.TextChanged += QEtymologyBoxRefine;
        if (box.InputBindings.Count == 0)
        {
            box.InputBindings.Add(new KeyBinding(
                PEtymologyCommand.PEtymologyCommandAddition, Key.Return, ModifierKeys.None)
            {
                CommandParameter = etymon,
            });
        }
    }

    private void QEtymologyItemRefine(object? sender, EventArgs e)
    {
        QLookItem.QLookItemApply(_qEtymologyField);
    }

    private static void QEtymologyBoxRefine(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { DataContext: PEtymon etymon } box)
        {
            etymon.PEtymonText = box.Text;
        }
    }
}
