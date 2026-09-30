using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class PSentence : INotifyPropertyChanged
{
    private CStateWording _pSentenceText;
    private long? _pSentenceCitation;
    private CStateWording _pSentenceParticle;
    private CStateWording _pSentenceDependence;
    private int _pSentenceParticleColumn;
    private int _pSentenceDependenceColumn = 2;
    private bool _pSentenceFrameVisible;
    private long _pSentenceRow;

    internal PSentence(
        ObservableCollection<QCitationItem> catalog,
        ObservableCollection<string> particles,
        ObservableCollection<string> dependences,
        ObservableCollection<PLanguageItem> languages,
        CSentenceDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        CExampleDraft? example = draft.CSentenceDraftExample;

        PSentenceCitationCatalog = catalog;
        PSentenceParticleCatalog = particles;
        PSentenceDependenceCatalog = dependences;
        PSentenceLanguageCatalog = languages;
        _pSentenceRow = draft.CSentenceDraftId;
        _pSentenceText = draft.CSentenceDraftText;
        PSentenceCitation = example?.CExampleDraftReference;
        _pSentenceParticle = draft.CSentenceDraftParticle;
        _pSentenceDependence = draft.CSentenceDraftDependence;
        PSentenceGlossShow(example?.CExampleDraftGloss ?? []);
    }

    public ObservableCollection<QCitationItem> PSentenceCitationCatalog { get; }

    public ObservableCollection<PLanguageItem> PSentenceLanguageCatalog { get; }

    public ObservableCollection<string> PSentenceParticleCatalog { get; }

    public ObservableCollection<string> PSentenceDependenceCatalog { get; }

    internal long PSentenceRow => _pSentenceRow;

    public CStateWording PSentenceText
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

    public long? PSentenceCitation
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

    public CStateWording PSentenceParticle
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

    public CStateWording PSentenceDependence
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
        !_pSentenceParticle.CStateWordingMuted || !_pSentenceDependence.CStateWordingMuted;

    public string PSentenceFrameGap =>
        !_pSentenceParticle.CStateWordingMuted && !_pSentenceDependence.CStateWordingMuted ? " " : string.Empty;

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

    internal void PSentenceOrderApply(CSentenceOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);

        PSentenceParticleColumn = order.CSentenceOrderParticle * 2;
        PSentenceDependenceColumn = order.CSentenceOrderDependence * 2;
    }

    internal void PSentenceShow(CSentenceDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        CExampleDraft? example = draft.CSentenceDraftExample;

        _pSentenceRow = draft.CSentenceDraftId;
        PSentenceGlossShow(example?.CExampleDraftGloss ?? []);
        PSentenceText = draft.CSentenceDraftText;
        PSentenceCitation = example?.CExampleDraftReference;
        PSentenceParticle = draft.CSentenceDraftParticle;
        PSentenceDependence = draft.CSentenceDraftDependence;
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
            container, "PSentenceParticle", row.PSentenceParticle, row.PSentenceParticleCatalog);
        PSentenceChoiceApply(
            container,
            "PSentenceDependence",
            row.PSentenceDependence,
            row.PSentenceDependenceCatalog);
        if (QLook.QLookPartFind<Grid>(container, "PSentenceParticleField") is Grid particle)
        {
            Grid.SetColumn(particle, row.PSentenceParticleColumn);
        }

        if (QLook.QLookPartFind<Grid>(container, "PSentenceDependenceField") is Grid dependence)
        {
            Grid.SetColumn(dependence, row.PSentenceDependenceColumn);
        }

        if (QLook.QLookPartFind<TextBox>(container, "PSentenceText") is TextBox text)
        {
            text.Text = row.PSentenceText.CStateWordingText;
            QStateConverter.QStateHintRefine(text, QField.QFieldHintProperty, row.PSentenceText);
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
        FrameworkElement container, string name, CStateWording value, ObservableCollection<string> catalog)
    {
        string shown = value.CStateWordingText;
        if (QLook.QLookPartFind<ComboBox>(container, name) is ComboBox choice)
        {
            choice.ItemsSource = catalog;
            choice.Text = shown;
            QStateConverter.QStateHintRefine(choice, QField.QFieldHintProperty, value);
        }

        if (QLook.QLookPartFind<TextBlock>(container, name + "Ghost") is TextBlock ghost)
        {
            ghost.Text = shown;
        }

        if (QLook.QLookPartFind<TextBlock>(container, name + "Hint") is TextBlock prompt)
        {
            QStateConverter.QStateHintRefine(prompt, TextBlock.TextProperty, value);
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

    internal static string PSentenceCitationFind(ObservableCollection<QCitationItem> catalog, long? anchor)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        if (anchor is not long id)
        {
            return string.Empty;
        }

        foreach (QCitationItem row in catalog)
        {
            if (row.QCitationItemId == id)
            {
                return row.QCitationItemName;
            }
        }

        return string.Empty;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

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
