using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QImprint : QChronicleHost
{
    private readonly UserControl _qImprintSurface;

    private readonly QAuthor _qImprintAuthor;

    private CImprint _cImprint = null!;

    internal QImprint(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qImprintSurface = surface;
        _qImprintAuthor = new QAuthor(QContract.QContractFind<Grid>(surface, "PAuthor"));
        QChronicle.QChronicleIntroduce(surface, this);

        QImprintKindIcon.QIconSource = QIcon.QIconResolve("expand", 12);

        QImprintTitle.TextChanged += QImprintTitleObserve;
        QImprintYear.TextChanged += QImprintYearObserve;
        QImprintUrl.TextChanged += QImprintUrlObserve;
        QImprintNote.TextChanged += QImprintNoteObserve;

        QChoice.QChoiceDropperAttach(QImprintKind, QImprintKindMenu, QImprintKind);
    }

    private TextBox QImprintTitle => QContract.QContractFind<TextBox>(_qImprintSurface, "PImprintTitle");

    private ToggleButton QImprintKind => QContract.QContractFind<ToggleButton>(_qImprintSurface, "PImprintKind");

    private TextBlock QImprintKindName => QContract.QContractFind<TextBlock>(_qImprintSurface, "PImprintKindName");

    private QIconImage QImprintKindIcon => QContract.QContractFind<QIconImage>(_qImprintSurface, "PImprintKindIcon");

    private Popup QImprintKindMenu => QContract.QContractFind<Popup>(_qImprintSurface, "PImprintKindMenu");

    private StackPanel QImprintKindList => QContract.QContractFind<StackPanel>(_qImprintSurface, "PImprintKindList");

    private TextBlock QImprintTally => QContract.QContractFind<TextBlock>(_qImprintSurface, "PImprintTally");

    private TextBox QImprintYear => QContract.QContractFind<TextBox>(_qImprintSurface, "PImprintYear");

    private TextBox QImprintUrl => QContract.QContractFind<TextBox>(_qImprintSurface, "PImprintUrl");

    private TextBox QImprintNote => QContract.QContractFind<TextBox>(_qImprintSurface, "PImprintNote");

    internal void QImprintIntroduce(CImprint imprint)
    {
        _cImprint = imprint;
        QChoice.QChoiceMenuRefine(QImprintKindList, QImprintKindObserve, _cImprint.CImprintKindRead());
        _qImprintAuthor.QAuthorIntroduce(_cImprint);
        _cImprint.CImprintReferenceChanged += QImprintDraftRefine;
    }

    internal void QImprintClearRefine()
    {
        QImprintDraftRefine(_cImprint.CImprintEmptyRead());
    }

    internal void QImprintCloseRefine()
    {
        QImprintKindMenu.IsOpen = false;
    }

    internal void QImprintTallyRefine()
    {
        QImprintTally.Text = _cImprint.CImprintTallyRead();
    }

    public void QChronicleUndoObserve()
    {
        QChronicle.QChronicleCaretRefine(_cImprint.CImprintDesk.CDeskChronicle.CDeskChronicleUndo);
    }

    public void QChronicleRedoObserve()
    {
        QChronicle.QChronicleCaretRefine(_cImprint.CImprintDesk.CDeskChronicle.CDeskChronicleRedo);
    }

    private void QImprintDraftRefine(CReference reference)
    {
        QImprintTitle.Text = reference.CReferenceTitle;
        QImprintTitle.SetResourceReference(QField.QFieldHintProperty, reference.CReferenceTitleHint);
        QImprintYear.Text = reference.CReferenceYear;
        QImprintYear.SetResourceReference(QField.QFieldHintProperty, reference.CReferenceYearHint);
        QImprintUrl.Text = reference.CReferenceUrl;
        QImprintUrl.SetResourceReference(QField.QFieldHintProperty, reference.CReferenceUrlHint);
        QImprintNote.Text = reference.CReferenceNote;
        QImprintNote.SetResourceReference(QField.QFieldHintProperty, reference.CReferenceNoteHint);
        QImprintKindName.SetResourceReference(TextBlock.TextProperty, reference.CReferenceKindKey);
        QChoice.QChoiceMenuApply(QImprintKindList, reference.CReferenceKindTag);
        QImprintTallyRefine();
    }

    private void QImprintTitleObserve(object sender, TextChangedEventArgs e)
    {
        _cImprint.CImprintTitleSet(QImprintTitle.Text);
    }

    private void QImprintYearObserve(object sender, TextChangedEventArgs e)
    {
        _cImprint.CImprintYearSet(QImprintYear.Text);
    }

    private void QImprintUrlObserve(object sender, TextChangedEventArgs e)
    {
        _cImprint.CImprintUrlSet(QImprintUrl.Text);
    }

    private void QImprintNoteObserve(object sender, TextChangedEventArgs e)
    {
        _cImprint.CImprintNoteSet(QImprintNote.Text);
    }

    private void QImprintKindObserve(object sender, RoutedEventArgs e)
    {
        _cImprint.CImprintKindSet(QSender.QSenderTagRead(sender));
        QImprintKindRefine();
    }

    private void QImprintKindRefine()
    {
        QImprintKind.IsChecked = false;
    }
}
