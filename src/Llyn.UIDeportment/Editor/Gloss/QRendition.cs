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

internal sealed class QRendition : INotifyPropertyChanged
{
    private readonly long _qRenditionId;
    private string _qRenditionLanguage;
    private string? _qRenditionHint;
    private ImageSource? _qRenditionFlag;
    private CStateWording _qRenditionText;
    private bool _qRenditionLanguageVisible;

    internal QRendition(ObservableCollection<QLanguageItem> catalog, CGlossDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        QRenditionLanguageCatalog = catalog;
        _qRenditionId = draft.CGlossDraftId;
        _qRenditionLanguage = draft.CGlossDraftLanguage;
        _qRenditionHint = draft.CGlossDraftHint;
        _qRenditionFlag = QEnsignImage.QEnsignRead(_qRenditionLanguage);
        _qRenditionText = draft.CGlossDraftWording;
    }

    public ObservableCollection<QLanguageItem> QRenditionLanguageCatalog { get; }

    public long QRenditionId => _qRenditionId;

    public string QRenditionLanguage => _qRenditionLanguage;

    public string? QRenditionHint => _qRenditionHint;

    public ImageSource? QRenditionFlag
    {
        get => _qRenditionFlag;
        private set
        {
            if (ReferenceEquals(_qRenditionFlag, value))
            {
                return;
            }

            _qRenditionFlag = value;
            QRenditionRaise(nameof(QRenditionFlag));
        }
    }

    public CStateWording QRenditionText
    {
        get => _qRenditionText;
        private set
        {
            if (_qRenditionText == value)
            {
                return;
            }

            _qRenditionText = value;
            QRenditionRaise(nameof(QRenditionText));
        }
    }

    public bool QRenditionLanguageVisible
    {
        get => _qRenditionLanguageVisible;
        set
        {
            if (_qRenditionLanguageVisible == value)
            {
                return;
            }

            _qRenditionLanguageVisible = value;
            QRenditionRaise(nameof(QRenditionLanguageVisible));
        }
    }

    internal void QRenditionShow(CGlossDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        if (!string.Equals(_qRenditionLanguage, draft.CGlossDraftLanguage, StringComparison.Ordinal))
        {
            _qRenditionLanguage = draft.CGlossDraftLanguage;
            _qRenditionHint = draft.CGlossDraftHint;
            QRenditionFlag = QEnsignImage.QEnsignRead(_qRenditionLanguage);
            QRenditionRaise(nameof(QRenditionLanguage));
        }

        QRenditionText = draft.CGlossDraftWording;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    internal event Action<QRendition, string>? QRenditionPicked;

    internal static void QRenditionRowApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QRendition gloss)
        {
            return;
        }

        bool flagged = gloss.QRenditionFlag is not null;
        if (QLook.QLookPartFind<Image>(container, "PGlossFlag") is Image flag)
        {
            flag.Source = gloss.QRenditionFlag;
            flag.Visibility = QLook.QLookVisibleRead(flagged);
        }

        if (QLook.QLookPartFind<Ellipse>(container, "PGlossGlobe") is Ellipse globe)
        {
            globe.Visibility = QLook.QLookVisibleRead(!flagged);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PGlossName") is TextBlock label)
        {
            QRenditionNameApply(label, gloss);
        }

        ToggleButton? speaker = QLook.QLookPartFind<ToggleButton>(container, "PGlossSpeaker");
        if (speaker is not null)
        {
            speaker.IsChecked = gloss.QRenditionLanguageVisible;
            speaker.Click -= QRenditionSpeakerRefine;
            speaker.Click += QRenditionSpeakerRefine;
        }

        if (QLook.QLookPartFind<Popup>(container, "PGlossPopup") is Popup popup)
        {
            popup.PlacementTarget = speaker;
            popup.IsOpen = gloss.QRenditionLanguageVisible;
            popup.Closed -= QRenditionPopupRefine;
            popup.Closed += QRenditionPopupRefine;
        }

        if (QLook.QLookPartFind<ListBox>(container, "PGlossList") is ListBox list)
        {
            list.SelectionChanged -= QRenditionPickRaise;
            list.SelectionChanged -= QRenditionListRefine;
            list.SelectedValuePath = nameof(QLanguageItem.QLanguageItemName);
            list.ItemsSource = gloss.QRenditionLanguageCatalog;
            list.SelectedValue = gloss.QRenditionLanguage;
            list.SelectionChanged += QRenditionPickRaise;
            list.SelectionChanged += QRenditionListRefine;
            QLookItem.QLookItemAttach(list, QLanguageItem.QLanguageItemApply);
        }

        QRenditionTextApply(container, gloss.QRenditionText);

        if (QLook.QLookPartFind<Button>(container, "PGlossBin") is Button bin)
        {
            bin.CommandParameter = gloss;
            if (bin.Content is QIconImage mark)
            {
                mark.QIconSource = QIcon.QIconResolve("close", 12);
            }
        }
    }

    private static void QRenditionNameApply(TextBlock label, QRendition gloss)
    {
        if (gloss.QRenditionHint is string hint)
        {
            label.SetResourceReference(TextBlock.TextProperty, hint);
            label.SetResourceReference(TextBlock.ForegroundProperty, "Theme.Muted");
            return;
        }

        label.Text = gloss.QRenditionLanguage;
        label.ClearValue(TextBlock.ForegroundProperty);
    }

    private static void QRenditionTextApply(FrameworkElement container, CStateWording text)
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

    private static void QRenditionSpeakerRefine(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton { DataContext: QRendition gloss } speaker)
        {
            gloss.QRenditionLanguageVisible = QLook.QLookCheckedRead(speaker.IsChecked);
        }
    }

    private static void QRenditionPopupRefine(object? sender, EventArgs e)
    {
        if (sender is Popup { DataContext: QRendition gloss })
        {
            gloss.QRenditionLanguageVisible = false;
        }
    }

    private static void QRenditionPickRaise(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox { DataContext: QRendition gloss, SelectedValue: string language })
        {
            gloss.QRenditionPicked?.Invoke(gloss, language);
        }
    }

    private static void QRenditionListRefine(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox { DataContext: QRendition gloss } list)
        {
            list.Dispatcher.BeginInvoke(() => gloss.QRenditionLanguageVisible = false);
        }
    }

    private void QRenditionRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
