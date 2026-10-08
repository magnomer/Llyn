using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QTranscriptGloss
{
    private readonly UserControl _qTranscriptGlossScope;

    private readonly ObservableCollection<PGloss> _qTranscriptGloss = [];

    private ObservableCollection<PLanguageItem> _qLanguageItem = [];

    private CCorpus _cCorpus = null!;

    internal QTranscriptGloss(UserControl scope)
    {
        ArgumentNullException.ThrowIfNull(scope);

        _qTranscriptGlossScope = scope;

        QTranscriptAdditionIcon.QIconSource = QIcon.QIconResolve("add", 12);
        QTranscriptSeedField.SetResourceReference(QField.QFieldHintProperty, "Example.Translation");

        QTranscriptSeedField.GotKeyboardFocus += QTranscriptSeedObserve;
        QTranscriptAddition.Click += QGlossAddObserve;
    }

    private ItemsControl QTranscriptGlossLine =>
        QContract.QContractFind<ItemsControl>(_qTranscriptGlossScope, "PTranscriptGlossLine");

    private Border QTranscriptSeed => QContract.QContractFind<Border>(_qTranscriptGlossScope, "PTranscriptSeed");

    private TextBox QTranscriptSeedField =>
        QContract.QContractFind<TextBox>(_qTranscriptGlossScope, "PTranscriptSeedField");

    private Button QTranscriptAddition =>
        QContract.QContractFind<Button>(_qTranscriptGlossScope, "PTranscriptAddition");

    private QIconImage QTranscriptAdditionIcon =>
        QContract.QContractFind<QIconImage>(_qTranscriptGlossScope, "PTranscriptAdditionIcon");

    internal void QTranscriptGlossIntroduce(CCorpus corpus, ObservableCollection<PLanguageItem> language)
    {
        ArgumentNullException.ThrowIfNull(corpus);
        ArgumentNullException.ThrowIfNull(language);

        _cCorpus = corpus;
        _qLanguageItem = language;
        _cCorpus.CCorpusTranscriptChanged += QTranscriptGlossShow;
        _cCorpus.CCorpusTranscript.CTranscriptDraftChanged += QTranscriptGlossShow;

        QTranscriptGlossLine.ItemsSource = _qTranscriptGloss;
        QLookItem.QLookItemAttach(QTranscriptGlossLine, QTranscriptGlossApply);
    }

    private void QTranscriptGlossShow(CExample? example)
    {
        QLookItem.QLookItemShow(
            _qTranscriptGloss,
            example?.CExampleGloss ?? [],
            static row => row.PGlossId,
            static draft => draft.CGlossDraftId,
            QTranscriptGlossCreate,
            (row, draft) =>
            {
                row.PGlossShow(draft);
                return row;
            });

        QTranscriptSeedShow();
    }

    private PGloss QTranscriptGlossCreate(CGlossDraft draft)
    {
        PGloss row = new(_qLanguageItem, draft);
        row.PGlossPicked += QGlossSpeakerObserve;
        return row;
    }

    private void QGlossTextObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { DataContext: PGloss gloss } box)
        {
            _cCorpus.CCorpusAnthology.CAnthologyGlossSet(gloss.PGlossId, box.Text);
        }
    }

    private void QGlossSpeakerObserve(PGloss gloss, string language)
    {
        _cCorpus.CCorpusAnthology.CAnthologyLanguageSet(gloss.PGlossId, language);
    }

    private void QGlossAddObserve(object sender, RoutedEventArgs e)
    {
        int below = QSender.QSenderItemRead<PGloss>(sender) is PGloss row
            ? _qTranscriptGloss.IndexOf(row)
            : _qTranscriptGloss.Count - 1;

        _cCorpus.CCorpusAnthology.CAnthologyGlossAdd(below);
    }

    private void QGlossRemoveObserve(object sender, RoutedEventArgs e)
    {
        if (QSender.QSenderItemRead<PGloss>(sender) is PGloss gloss)
        {
            _cCorpus.CCorpusAnthology.CAnthologyGlossRemove(gloss.PGlossId);
        }
    }

    private void QTranscriptGlossApply(FrameworkElement container, object item, string? change)
    {
        TextBox? field = QLook.QLookPartFind<TextBox>(container, "PGlossText");
        if (field is not null)
        {
            field.TextChanged -= QGlossTextObserve;
        }

        PGloss.PGlossRowApply(container, item, change);

        if (field is not null)
        {
            field.TextChanged += QGlossTextObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PGlossAddition") is Button addition)
        {
            addition.Click -= QGlossAddObserve;
            addition.Click += QGlossAddObserve;
            if (addition.Content is QIconImage mark)
            {
                mark.QIconSource = QIcon.QIconResolve("add", 12);
            }
        }

        if (QLook.QLookPartFind<Button>(container, "PGlossRemoval") is Button removal)
        {
            removal.Click -= QGlossRemoveObserve;
            removal.Click += QGlossRemoveObserve;
            if (removal.Content is QIconImage mark)
            {
                mark.QIconSource = QIcon.QIconResolve("remove", 12);
            }
        }
    }

    private void QTranscriptSeedShow()
    {
        QTranscriptSeed.Visibility = _qTranscriptGloss.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QTranscriptSeedObserve(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (_cCorpus.CCorpusAnthology.CAnthologyGlossPrepare())
        {
            QTranscriptSeedRefine();
        }
    }

    private void QTranscriptSeedRefine()
    {
        QTranscriptGlossLine.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, () =>
        {
            if (QTranscriptGlossLine.ItemContainerGenerator.ContainerFromIndex(0) is DependencyObject container
                && QField.QFieldCaretFind(container) is TextBox box)
            {
                box.Focus();
            }
        });
    }
}
