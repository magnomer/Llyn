using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PGloss : INotifyPropertyChanged
{
    private readonly long _pGlossId;
    private string _pGlossLanguage;
    private ImageSource? _pGlossFlag;
    private LStateValue _pGlossText;
    private bool _pGlossLanguageVisible;

    internal PGloss(ObservableCollection<PLanguageItem> catalog, LGlossDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        PGlossLanguageCatalog = catalog;
        _pGlossId = draft.LGlossDraftId;
        _pGlossLanguage = draft.LGlossDraftLanguage;
        _pGlossFlag = LEnsignImage.LEnsignFind(_pGlossLanguage);
        _pGlossText = draft.LGlossDraftText;
    }

    public ObservableCollection<PLanguageItem> PGlossLanguageCatalog { get; }

    public long PGlossId => _pGlossId;

    public string PGlossLanguage
    {
        get => _pGlossLanguage;
        set
        {
            if (value is null)
            {
                return;
            }

            PGlossLanguageVisible = false;
            if (string.Equals(_pGlossLanguage, value, StringComparison.Ordinal))
            {
                return;
            }

            _pGlossLanguage = value;
            PGlossFlag = LEnsignImage.LEnsignFind(value);
            PGlossRaise(nameof(PGlossLanguage));
        }
    }

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

    public LStateValue PGlossText
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

    internal void PGlossShow(LGlossDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        if (!string.Equals(_pGlossLanguage, draft.LGlossDraftLanguage, StringComparison.Ordinal))
        {
            _pGlossLanguage = draft.LGlossDraftLanguage;
            PGlossFlag = LEnsignImage.LEnsignFind(_pGlossLanguage);
            PGlossRaise(nameof(PGlossLanguage));
        }

        PGlossText = draft.LGlossDraftText;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

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
            PGlossNameApply(label, gloss.PGlossLanguage);
        }

        ToggleButton? speaker = QLook.QLookPartFind<ToggleButton>(container, "PGlossSpeaker");
        if (speaker is not null)
        {
            speaker.IsChecked = gloss.PGlossLanguageVisible;
            speaker.Click -= PGlossSpeakerHandle;
            speaker.Click += PGlossSpeakerHandle;
        }

        if (QLook.QLookPartFind<Popup>(container, "PGlossPopup") is Popup popup)
        {
            popup.PlacementTarget = speaker;
            popup.IsOpen = gloss.PGlossLanguageVisible;
            popup.Closed -= PGlossPopupHandle;
            popup.Closed += PGlossPopupHandle;
        }

        if (QLook.QLookPartFind<ListBox>(container, "PGlossList") is ListBox list)
        {
            list.SelectionChanged -= PGlossListHandle;
            list.SelectedValuePath = nameof(PLanguageItem.PLanguageItemName);
            list.ItemsSource = gloss.PGlossLanguageCatalog;
            list.SelectedValue = gloss.PGlossLanguage;
            list.SelectionChanged += PGlossListHandle;
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

    private static void PGlossNameApply(TextBlock label, string language)
    {
        if (language.Length == 0)
        {
            label.SetResourceReference(TextBlock.TextProperty, "Example.Language");
            label.SetResourceReference(TextBlock.ForegroundProperty, "Theme.Muted");
            return;
        }

        label.Text = language;
        label.ClearValue(TextBlock.ForegroundProperty);
    }

    private static void PGlossTextApply(FrameworkElement container, LStateValue text)
    {
        QStateConverter state = new();
        CultureInfo culture = CultureInfo.CurrentCulture;
        string unknown = QLocalizationCatalog.QLocalizationTextRead("Display.Unknown");
        if (QLook.QLookPartFind<TextBox>(container, "PGlossText") is TextBox field)
        {
            string hint = QLocalizationCatalog.QLocalizationTextRead("Example.Translation");
            field.SetValue(TextBox.TextProperty, state.Convert(text, typeof(string), string.Empty, culture));
            field.SetValue(
                QField.QFieldHintProperty, state.Convert([text, unknown, hint], typeof(string), string.Empty, culture));
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PGlossText") is TextBlock line)
        {
            line.SetValue(
                TextBlock.TextProperty, state.Convert([text, unknown], typeof(string), string.Empty, culture));
        }
    }

    private static void PGlossSpeakerHandle(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton { DataContext: PGloss gloss } speaker)
        {
            gloss.PGlossLanguageVisible = QLook.QLookCheckedRead(speaker.IsChecked);
        }
    }

    private static void PGlossPopupHandle(object? sender, EventArgs e)
    {
        if (sender is Popup { DataContext: PGloss gloss })
        {
            gloss.PGlossLanguageVisible = false;
        }
    }

    private static void PGlossListHandle(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox { DataContext: PGloss gloss, SelectedValue: string language })
        {
            gloss.PGlossLanguage = language;
        }
    }

    private void PGlossRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
