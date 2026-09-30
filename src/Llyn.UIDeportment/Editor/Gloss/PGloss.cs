using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PGloss : INotifyPropertyChanged
{
    private readonly long _pGlossId;
    private string _pGlossLanguage;
    private string? _pGlossHint;
    private ImageSource? _pGlossFlag;
    private CStateWording _pGlossText;
    private bool _pGlossLanguageVisible;

    internal PGloss(ObservableCollection<PLanguageItem> catalog, CGlossDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        PGlossLanguageCatalog = catalog;
        _pGlossId = draft.CGlossDraftId;
        _pGlossLanguage = draft.CGlossDraftLanguage;
        _pGlossHint = draft.CGlossDraftHint;
        _pGlossFlag = QEnsignImage.QEnsignRead(_pGlossLanguage);
        _pGlossText = draft.CGlossDraftWording;
    }

    public ObservableCollection<PLanguageItem> PGlossLanguageCatalog { get; }

    public long PGlossId => _pGlossId;

    public string PGlossLanguage => _pGlossLanguage;

    public string? PGlossHint => _pGlossHint;

    public ImageSource? PGlossFlag
    {
        get => _pGlossFlag;
        private set
        {
            if (ReferenceEquals(_pGlossFlag, value))
            {
                return;
            }

            _pGlossFlag = value;
            PGlossRaise(nameof(PGlossFlag));
        }
    }

    public CStateWording PGlossText
    {
        get => _pGlossText;
        private set
        {
            if (_pGlossText == value)
            {
                return;
            }

            _pGlossText = value;
            PGlossRaise(nameof(PGlossText));
        }
    }

    public bool PGlossLanguageVisible
    {
        get => _pGlossLanguageVisible;
        set
        {
            if (_pGlossLanguageVisible == value)
            {
                return;
            }

            _pGlossLanguageVisible = value;
            PGlossRaise(nameof(PGlossLanguageVisible));
        }
    }

    internal void PGlossShow(CGlossDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        if (!string.Equals(_pGlossLanguage, draft.CGlossDraftLanguage, StringComparison.Ordinal))
        {
            _pGlossLanguage = draft.CGlossDraftLanguage;
            _pGlossHint = draft.CGlossDraftHint;
            PGlossFlag = QEnsignImage.QEnsignRead(_pGlossLanguage);
            PGlossRaise(nameof(PGlossLanguage));
        }

        PGlossText = draft.CGlossDraftWording;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    internal event Action<PGloss, string>? PGlossPicked;

    internal static void PGlossRowApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PGloss gloss)
        {
            return;
        }

        bool flagged = gloss.PGlossFlag is not null;
        if (QLook.QLookPartFind<Image>(container, "PGlossFlag") is Image flag)
        {
            flag.Source = gloss.PGlossFlag;
            flag.Visibility = QLook.QLookVisibleRead(flagged);
        }

        if (QLook.QLookPartFind<Ellipse>(container, "PGlossGlobe") is Ellipse globe)
        {
            globe.Visibility = QLook.QLookVisibleRead(!flagged);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PGlossName") is TextBlock label)
        {
            PGlossNameApply(label, gloss);
        }

        ToggleButton? speaker = QLook.QLookPartFind<ToggleButton>(container, "PGlossSpeaker");
        if (speaker is not null)
        {
            speaker.IsChecked = gloss.PGlossLanguageVisible;
            speaker.Click -= PGlossSpeakerRefine;
            speaker.Click += PGlossSpeakerRefine;
        }

        if (QLook.QLookPartFind<Popup>(container, "PGlossPopup") is Popup popup)
        {
            popup.PlacementTarget = speaker;
            popup.IsOpen = gloss.PGlossLanguageVisible;
            popup.Closed -= PGlossPopupRefine;
            popup.Closed += PGlossPopupRefine;
        }

        if (QLook.QLookPartFind<ListBox>(container, "PGlossList") is ListBox list)
        {
            list.SelectionChanged -= PGlossPickRaise;
            list.SelectionChanged -= PGlossListRefine;
            list.SelectedValuePath = nameof(PLanguageItem.PLanguageItemName);
            list.ItemsSource = gloss.PGlossLanguageCatalog;
            list.SelectedValue = gloss.PGlossLanguage;
            list.SelectionChanged += PGlossPickRaise;
            list.SelectionChanged += PGlossListRefine;
            QLookItem.QLookItemAttach(list, PLanguageItem.PLanguageItemApply);
        }

        PGlossTextApply(container, gloss.PGlossText);

        if (QLook.QLookPartFind<Button>(container, "PGlossBin") is Button bin)
        {
            bin.CommandParameter = gloss;
            if (bin.Content is QIconImage mark)
            {
                mark.QIconSource = QIcon.QIconResolve("close", 12);
            }
        }
    }

    private static void PGlossNameApply(TextBlock label, PGloss gloss)
    {
        if (gloss.PGlossHint is string hint)
        {
            label.SetResourceReference(TextBlock.TextProperty, hint);
            label.SetResourceReference(TextBlock.ForegroundProperty, "Theme.Muted");
            return;
        }

        label.Text = gloss.PGlossLanguage;
        label.ClearValue(TextBlock.ForegroundProperty);
    }

    private static void PGlossTextApply(FrameworkElement container, CStateWording text)
    {
        if (QLook.QLookPartFind<TextBox>(container, "PGlossText") is TextBox field)
        {
            field.SetValue(TextBox.TextProperty, text.CStateWordingText);
            QStateConverter.QStateHintRefine(field, QField.QFieldHintProperty, text);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PGlossText") is TextBlock line)
        {
            QStateConverter.QStateTextRefine(line, TextBlock.TextProperty, text);
        }
    }

    private static void PGlossSpeakerRefine(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton { DataContext: PGloss gloss } speaker)
        {
            gloss.PGlossLanguageVisible = QLook.QLookCheckedRead(speaker.IsChecked);
        }
    }

    private static void PGlossPopupRefine(object? sender, EventArgs e)
    {
        if (sender is Popup { DataContext: PGloss gloss })
        {
            gloss.PGlossLanguageVisible = false;
        }
    }

    private static void PGlossPickRaise(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox { DataContext: PGloss gloss, SelectedValue: string language })
        {
            gloss.PGlossPicked?.Invoke(gloss, language);
        }
    }

    private static void PGlossListRefine(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox { DataContext: PGloss gloss } list)
        {
            list.Dispatcher.BeginInvoke(() => gloss.PGlossLanguageVisible = false);
        }
    }

    private void PGlossRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
