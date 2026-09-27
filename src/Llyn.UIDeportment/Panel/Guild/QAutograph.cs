using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QAutograph
{
    private readonly UserControl _qAutographSurface;

    private LGuild _lGuild = null!;

    internal QAutograph(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qAutographSurface = surface;

        QAutographName.SetResourceReference(QField.QFieldHintProperty, "Autograph.Name");
        QAutographUnion.SetResourceReference(QField.QFieldHintProperty, "Autograph.UnionSearch");
        QAutographName.TextChanged += QAutographNameHandle;
        QAutographUnion.TextChanged += QAutographUnionHandle;
        QAutographUnionList.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QAutographUnionSelect));

        QLookItem.QLookItemAttach(QAutographUnionList, QRollItem.QRollItemApply);
    }

    private TextBox QAutographName => QContract.QContractFind<TextBox>(_qAutographSurface, "PAutographName");

    private TextBlock QAutographWork => QContract.QContractFind<TextBlock>(_qAutographSurface, "PAutographWork");

    private TextBlock QAutographTally => QContract.QContractFind<TextBlock>(_qAutographSurface, "PAutographTally");

    private StackPanel QAutographUnionBody =>
        QContract.QContractFind<StackPanel>(_qAutographSurface, "PAutographUnionBody");

    private TextBox QAutographUnion => QContract.QContractFind<TextBox>(_qAutographSurface, "PAutographUnion");

    private ItemsControl QAutographUnionList =>
        QContract.QContractFind<ItemsControl>(_qAutographSurface, "PAutographUnionList");

    private TextBlock QAutographUnionNotice =>
        QContract.QContractFind<TextBlock>(_qAutographSurface, "PAutographUnionNotice");

    internal void QAutographAttach(LGuild guild)
    {
        _lGuild = guild;
        _lGuild.LGuildAutograph.LDeskStarted += QAutographStartUpdate;
        QAutographObserverAttach(_lGuild.LGuildAutograph);
        _lGuild.LGuildAutograph.LDeskDraftChanged += QAutographDraftUpdate;
    }

    internal void QAutographTallyShow(CVita vita)
    {
        ArgumentNullException.ThrowIfNull(vita);

        QAutographWork.Text = vita.CVitaWork;
        QAutographTally.Text = vita.CVitaTally;
    }

    internal void QAutographModeUpdate()
    {
        QAutographUnionBody.Visibility = QLook.QLookVisibleRead(_lGuild.LGuildUnion.QUnionShown);
        QAutographUnionNotice.Visibility = QLook.QLookVisibleRead(!_lGuild.LGuildUnion.QUnionShown);
    }

    private void QAutographObserverAttach(LDesk desk)
    {
        desk.LDeskVigil.QVigilDraftAttach(
            CSubject.CSubjectDraft, LObserver.LObserverCreate<CBulletin>(_qAutographSurface, desk.LDeskDraftUpdate));
        desk.LDeskVigil.QVigilDraftAttach(
            CSubject.CSubjectTenure, LObserver.LObserverCreate<CBulletin>(_qAutographSurface, desk.LDeskStateUpdate));
    }

    private void QAutographStartUpdate()
    {
        QAutographUnion.Text = string.Empty;
        QAutographUnionList.ItemsSource = null;
        QAutographName.Focus();
    }

    private void QAutographDraftUpdate(CDraft draft)
    {
        QAutographName.Text = draft.CDraftAuthorName;
    }

    private void QAutographNameHandle(object sender, TextChangedEventArgs e)
    {
        _lGuild.LGuildAutograph.LDeskQuill.QQuillAuthorSet(QAutographName.Text);
    }

    private void QAutographUnionHandle(object sender, TextChangedEventArgs e)
    {
        QAutographUnionList.ItemsSource =
            QRollItem.QRollItemBuild(_lGuild.LGuildUnion.QUnionRead(QAutographUnion.Text));
    }

    private void QAutographUnionSelect(object sender, RoutedEventArgs e)
    {
        _lGuild.LGuildUnion.QUnionSelect(QSender.QSenderSourceRead<QRollItem>(e)?.QRollItemId);
    }
}
