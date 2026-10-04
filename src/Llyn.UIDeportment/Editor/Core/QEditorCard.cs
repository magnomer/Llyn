using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QEditorCard
{
    private readonly ObservableCollection<PCard> _qEditorCardMeaning = [];

    private readonly ObservableCollection<PCard> _qEditorCardCollocation = [];

    private readonly ObservableCollection<PLanguageItem> _qEditorCardLanguage = [];

    private readonly QImage _qImage = new();

    private readonly QVideo _qVideo = new();

    private readonly QLink _qLink;

    private readonly QSpeaker _qSpeaker;

    private readonly QContext _qContext;

    private readonly QRegister _qRegister;

    private readonly QProffer _qProffer;

    private readonly QLabel _qLabel;

    private readonly QSlate _qSlate;

    private readonly QGloss _qGloss;

    private readonly QCitation _qCitation;

    private readonly QCardDrag _qCardDrag;

    private readonly QCard _qCard;

    private readonly QMeaning _qMeaning;

    private readonly QCollocation _qCollocation;

    private readonly QEtymologyEditor _qEtymologyEditor;

    internal QEditorCard(FrameworkElement surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        QEditorCardSentence = new QSentence(_qEditorCardMeaning, _qEditorCardCollocation);
        QEditorCardProspect = new QProspect(surface, QEditorCardSentence);
        _qLink = new QLink(surface, _qEditorCardMeaning, _qEditorCardCollocation, QEditorCardProspect);
        _qSpeaker = new QSpeaker(surface, _qEditorCardLanguage, _qLink);
        surface.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(QEditorTextObserve));
        _qContext = new QContext(surface, _qEditorCardMeaning, _qEditorCardCollocation);
        _qRegister = new QRegister(surface, _qEditorCardMeaning, _qEditorCardCollocation);
        _qProffer = new QProffer(
            surface, _qContext, _qRegister, QEditorCardSentence);
        _qLabel = new QLabel(surface, _qEditorCardMeaning, _qEditorCardCollocation);
        _qSlate = new QSlate(surface, _qLabel);
        _qGloss = new QGloss(_qEditorCardMeaning, _qEditorCardCollocation, QEditorCardSentence);
        _qCitation = new QCitation(
            _qEditorCardMeaning,
            _qEditorCardCollocation,
            QEditorCardSentence,
            _qProffer);
        _qCardDrag = new QCardDrag(surface);
        _qCard = new QCard(
            _qCardDrag,
            _qContext,
            _qRegister,
            _qLink,
            _qLabel,
            QEditorCardSentence,
            _qCitation,
            _qImage,
            _qVideo,
            _qEditorCardLanguage,
            _qEditorCardMeaning,
            _qEditorCardCollocation);
        _qMeaning = new QMeaning(surface, _qEditorCardMeaning, _qCard);
        _qCollocation = new QCollocation(surface, _qEditorCardCollocation, _qCard);
        _qEtymologyEditor = new QEtymologyEditor(surface, QEditorCardProspect);
    }

    internal QSentence QEditorCardSentence { get; }

    internal QProspect QEditorCardProspect { get; }

    internal void QEditorCardIntroduce(QWindow host, CEditor editor)
    {
        _qImage.QImageIntroduce(editor.CEditorImage);
        _qVideo.QVideoIntroduce(editor.CEditorVideo);
        _qContext.QContextIntroduce(editor, _qProffer);
        _qRegister.QRegisterIntroduce(editor, _qProffer);
        _qProffer.QProfferIntroduce(editor);
        QEditorCardProspect.QProspectIntroduce(editor, _qLink);
        _qLink.QLinkIntroduce(editor);
        _qSpeaker.QSpeakerIntroduce(editor, host.QWindowAtelier, host.QWindowEnvoy);
        _qGloss.QGlossIntroduce(editor);
        QEditorCardSentence.QSentenceIntroduce(
            editor, host, QEditorCardProspect, _qGloss, _qCitation);
        _qCitation.QCitationIntroduce(editor, host.QWindowAtelier);
        _qLabel.QLabelIntroduce(editor, _qSlate);
        _qSlate.QSlateIntroduce(editor);
        _qEtymologyEditor.QEtymologyIntroduce(editor, host.QWindowAtelier);
        _qCardDrag.QCardDragIntroduce(editor);
        _qCard.QCardIntroduce(editor);
        _qMeaning.QMeaningIntroduce(editor);
        _qCollocation.QCollocationIntroduce(editor);
    }

    internal void QEditorStartRefine()
    {
        _qEditorCardMeaning.Clear();
        _qEditorCardCollocation.Clear();
    }

    internal void QEditorMeaningRefine(CEntryDraft draft)
    {
        _qCard.QCardRefine(_qEditorCardMeaning, "Meaning", draft.CEntryDraftMeanings);
    }

    internal void QEditorCollocationRefine(CEntryDraft draft)
    {
        _qCard.QCardRefine(_qEditorCardCollocation, "Collocation", draft.CEntryDraftCollocations);
    }

    private void QEditorTextObserve(object sender, TextChangedEventArgs e)
    {
        switch (e.OriginalSource)
        {
            case TextBox { DataContext: PGloss or QImageItem } box:
                QEditorFieldObserve(box);
                break;
            case TextBox { DataContext: PContextCaret caret } box:
                _qContext.QContextTextObserve(caret, box.Text);
                break;
            case TextBox { DataContext: PRegisterCaret caret } box:
                _qRegister.QRegisterTextObserve(caret, box.Text);
                break;
            case TextBox { DataContext: PLinkCaret caret } box:
                _qLink.QLinkTextObserve(caret, box.Text);
                break;
            case TextBox { DataContext: PLabelCaret caret } box:
                _qLabel.QLabelTextObserve(caret, box.Text);
                break;
        }
    }

    private void QEditorFieldObserve(TextBox box)
    {
        if (!box.IsKeyboardFocusWithin)
        {
            return;
        }

        switch (box.DataContext)
        {
            case PGloss gloss:
                _qGloss.QGlossTextObserve(gloss, box.Text);
                break;
            case QImageItem row:
                _qImage.QImageFieldObserve(row, box.Text);
                break;
        }
    }
}
