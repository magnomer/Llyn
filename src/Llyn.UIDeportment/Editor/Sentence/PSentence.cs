using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed partial class PSentence : INotifyPropertyChanged
{
    private LStateValue _pSentenceText;
    private LStateAnchor _pSentenceCitation;
    private LStateValue _pSentenceParticle;
    private LStateValue _pSentenceDependence;
    private int _pSentenceParticleColumn;
    private int _pSentenceDependenceColumn = 2;
    private bool _pSentenceFrameVisible;
    private long _pSentenceRow;

    internal PSentence(
        ObservableCollection<PCitationItem> catalog,
        ObservableCollection<string> particles,
        ObservableCollection<string> dependences,
        ObservableCollection<PLanguageItem> languages,
        LSentenceDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        LExampleDraft example = draft.LSentenceDraftExample
            ?? LExampleDraft.LExampleDraftCreate(string.Empty);

        PSentenceCitationCatalog = catalog;
        PSentenceParticleCatalog = particles;
        PSentenceDependenceCatalog = dependences;
        PSentenceLanguageCatalog = languages;
        _pSentenceRow = draft.LSentenceDraftId;
        _pSentenceText = example.LExampleDraftText;
        _pSentenceCitation = example.LExampleDraftReference;
        _pSentenceParticle = draft.LSentenceDraftParticle;
        _pSentenceDependence = draft.LSentenceDraftDependence;
        _pSentenceMention = example.LExampleDraftMention;
        PSentenceGlossShow(example.LExampleDraftGloss);
    }

    public ObservableCollection<PCitationItem> PSentenceCitationCatalog { get; }

    public ObservableCollection<PLanguageItem> PSentenceLanguageCatalog { get; }

    public ObservableCollection<string> PSentenceParticleCatalog { get; }

    public ObservableCollection<string> PSentenceDependenceCatalog { get; }

    internal long PSentenceRow => _pSentenceRow;

    public LStateValue PSentenceText
    {
        get => _pSentenceText;
        private set
        {
            if (_pSentenceText == value)
            {
                return;
            }

            _pSentenceText = value;
            PSentenceRaise(nameof(PSentenceText));
        }
    }

    public LStateAnchor PSentenceCitation
    {
        get => _pSentenceCitation;
        private set
        {
            if (_pSentenceCitation == value)
            {
                return;
            }

            _pSentenceCitation = value;
            PSentenceRaise(nameof(PSentenceCitation));
        }
    }

    public LStateValue PSentenceParticle
    {
        get => _pSentenceParticle;
        private set
        {
            if (_pSentenceParticle == value)
            {
                return;
            }

            _pSentenceParticle = value;
            PSentenceRaise(nameof(PSentenceParticle));
            PSentenceFrameRaise();
        }
    }

    public LStateValue PSentenceDependence
    {
        get => _pSentenceDependence;
        private set
        {
            if (_pSentenceDependence == value)
            {
                return;
            }

            _pSentenceDependence = value;
            PSentenceRaise(nameof(PSentenceDependence));
            PSentenceFrameRaise();
        }
    }

    public bool PSentenceFrameVisible
    {
        get => _pSentenceFrameVisible || PSentenceFrameWritten;
        set
        {
            _pSentenceFrameVisible = value;
            PSentenceRaise(nameof(PSentenceFrameVisible));
        }
    }

    public bool PSentenceFrameWritten =>
        PSentenceFrameCheck(_pSentenceParticle) || PSentenceFrameCheck(_pSentenceDependence);

    public string PSentenceFrameGap =>
        PSentenceFrameCheck(_pSentenceParticle) && PSentenceFrameCheck(_pSentenceDependence) ? " " : string.Empty;

    public int PSentenceParticleColumn
    {
        get => _pSentenceParticleColumn;
        private set
        {
            if (_pSentenceParticleColumn == value)
            {
                return;
            }

            _pSentenceParticleColumn = value;
            PSentenceRaise(nameof(PSentenceParticleColumn));
        }
    }

    public int PSentenceDependenceColumn
    {
        get => _pSentenceDependenceColumn;
        private set
        {
            if (_pSentenceDependenceColumn == value)
            {
                return;
            }

            _pSentenceDependenceColumn = value;
            PSentenceRaise(nameof(PSentenceDependenceColumn));
        }
    }

    internal void PSentenceOrderApply(LSentenceOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);

        PSentenceParticleColumn = order.LSentenceOrderParticle * 2;
        PSentenceDependenceColumn = order.LSentenceOrderDependence * 2;
    }

    internal void PSentenceShow(LSentenceDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        LExampleDraft example = draft.LSentenceDraftExample
            ?? LExampleDraft.LExampleDraftCreate(string.Empty);

        _pSentenceRow = draft.LSentenceDraftId;
        _pSentenceMention = example.LExampleDraftMention;
        PSentenceGlossShow(example.LExampleDraftGloss);
        PSentenceText = example.LExampleDraftText;
        PSentenceCitation = example.LExampleDraftReference;
        PSentenceParticle = draft.LSentenceDraftParticle;
        PSentenceDependence = draft.LSentenceDraftDependence;
    }

    internal void PSentenceCitationShow()
    {
        PSentenceRaise(nameof(PSentenceCitation));
    }

    internal static void PSentenceRowApply(FrameworkElement container, PSentence row)
    {
        ArgumentNullException.ThrowIfNull(row);

        if (QLook.QLookPartFind<ToggleButton>(container, "PSentenceOpening") is ToggleButton opening)
        {
            opening.IsChecked = row.PSentenceFrameVisible;
            opening.Visibility = QLook.QLookVisibleRead(!row.PSentenceFrameWritten);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PSentenceSign") is TextBlock sign)
        {
            sign.Text = QLook.QLookFirstRead(row.PSentenceFrameVisible, "\u2212", "+");
        }

        if (QLook.QLookPartFind<StackPanel>(container, "PSentenceFrame") is StackPanel frame)
        {
            frame.Visibility = QLook.QLookVisibleRead(row.PSentenceFrameVisible);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PSentenceGap") is TextBlock gap)
        {
            gap.Text = row.PSentenceFrameGap;
        }

        PSentenceChoiceApply(
            container, "PSentenceParticle", row.PSentenceParticle, row.PSentenceParticleCatalog, "Card.ParticleHint");
        PSentenceChoiceApply(
            container,
            "PSentenceDependence",
            row.PSentenceDependence,
            row.PSentenceDependenceCatalog,
            "Card.DependenceHint");
        if (QLook.QLookPartFind<Grid>(container, "PSentenceParticleField") is Grid particle)
        {
            Grid.SetColumn(particle, row.PSentenceParticleColumn);
        }

        if (QLook.QLookPartFind<Grid>(container, "PSentenceDependenceField") is Grid dependence)
        {
            Grid.SetColumn(dependence, row.PSentenceDependenceColumn);
        }

        QStateConverter state = new();
        CultureInfo culture = CultureInfo.CurrentCulture;
        if (QLook.QLookPartFind<TextBox>(container, "PSentenceText") is TextBox text)
        {
            text.Text = (string)state.Convert(row.PSentenceText, typeof(string), string.Empty, culture);
            text.SetValue(QField.QFieldHintProperty, state.Convert(
                [
                    row.PSentenceText,
                    QLocalizationCatalog.QLocalizationTextRead("Display.Unknown"),
                    QLocalizationCatalog.QLocalizationTextRead("Card.ExampleHint"),
                ],
                typeof(string),
                string.Empty,
                culture));
            PSentenceLayoutApply(container, text);
        }

        if (QLook.QLookPartFind<TextBox>(container, "PSentenceCitation") is TextBox citation)
        {
            citation.Text = PSentenceCitationFind(row.PSentenceCitationCatalog, row.PSentenceCitation);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PSentenceGlossIcon") is QIconImage gloss)
        {
            gloss.QIconSource = QIcon.QIconResolve("gloss", 12);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PSentenceAddIcon") is QIconImage add)
        {
            add.QIconSource = QIcon.QIconResolve("add", 12);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PSentenceRemoveIcon") is QIconImage remove)
        {
            remove.QIconSource = QIcon.QIconResolve("remove", 12);
        }
    }

    private static void PSentenceChoiceApply(
        FrameworkElement container, string name, LStateValue value, ObservableCollection<string> catalog, string hint)
    {
        QStateConverter state = new();
        CultureInfo culture = CultureInfo.CurrentCulture;
        string shown = (string)state.Convert(value, typeof(string), string.Empty, culture);
        object tag = state.Convert(
            [
                value,
                QLocalizationCatalog.QLocalizationTextRead("Display.Unknown"),
                QLocalizationCatalog.QLocalizationTextRead(hint),
            ],
            typeof(string),
            string.Empty,
            culture);
        if (QLook.QLookPartFind<ComboBox>(container, name) is ComboBox choice)
        {
            choice.ItemsSource = catalog;
            choice.Text = shown;
            choice.SetValue(QField.QFieldHintProperty, tag);
        }

        if (QLook.QLookPartFind<TextBlock>(container, name + "Ghost") is TextBlock ghost)
        {
            ghost.Text = shown;
        }

        if (QLook.QLookPartFind<TextBlock>(container, name + "Hint") is TextBlock prompt)
        {
            prompt.Text = (string)tag;
            prompt.Visibility = QLook.QLookVisibleRead(shown.Length == 0);
        }
    }

    private static void PSentenceLayoutApply(FrameworkElement container, TextBox text)
    {
        if (QLook.QLookPartFind<TextBlock>(container, "PSentenceLead") is not TextBlock lead
            || QLook.QLookPartFind<Grid>(container, "PSentenceBody") is not Grid body
            || QLook.QLookPartFind<TextBox>(container, "PSentenceCitation") is not TextBox citation
            || BindingOperations.IsDataBound(body, FrameworkElement.MarginProperty))
        {
            return;
        }

        foreach (FrameworkElement target in new FrameworkElement[] { body, citation })
        {
            MultiBinding inset = new() { Converter = new QFontConverter() };
            inset.Bindings.Add(new Binding(nameof(TextBlock.FontFamily)) { Source = lead });
            inset.Bindings.Add(new Binding(nameof(TextBlock.FontSize)) { Source = lead });
            inset.Bindings.Add(new Binding(nameof(TextBox.FontFamily)) { Source = text });
            inset.Bindings.Add(new Binding(nameof(TextBox.FontSize)) { Source = text });
            target.SetBinding(FrameworkElement.MarginProperty, inset);
        }

        citation.SetBinding(Control.FontFamilyProperty, new Binding(nameof(TextBox.FontFamily)) { Source = text });
        citation.SetBinding(Control.FontSizeProperty, new Binding(nameof(TextBox.FontSize)) { Source = text });
        foreach (string name in new[] { "PSentenceParticle", "PSentenceDependence" })
        {
            if (QLook.QLookPartFind<ComboBox>(container, name) is ComboBox choice
                && QLook.QLookPartFind<Grid>(container, name + "Field") is Grid field)
            {
                choice.SetBinding(
                    FrameworkElement.WidthProperty, new Binding(nameof(Grid.ActualWidth)) { Source = field });
                choice.SetBinding(
                    FrameworkElement.HeightProperty, new Binding(nameof(Grid.ActualHeight)) { Source = field });
            }
        }
    }

    internal static string PSentenceCitationFind(ObservableCollection<PCitationItem> catalog, LStateAnchor anchor)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(anchor);

        if (!anchor.LStateAnchorLinked)
        {
            return string.Empty;
        }

        foreach (PCitationItem row in catalog)
        {
            if (anchor.LStateAnchorMatch(row.PCitationItemId))
            {
                return row.PCitationItemName;
            }
        }

        return string.Empty;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private static bool PSentenceFrameCheck(LStateValue value)
    {
        return value.LStateValueUncertain || value.LStateValueShown is not null;
    }

    private void PSentenceFrameRaise()
    {
        PSentenceRaise(nameof(PSentenceFrameVisible));
        PSentenceRaise(nameof(PSentenceFrameWritten));
        PSentenceRaise(nameof(PSentenceFrameGap));
    }

    private void PSentenceRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
