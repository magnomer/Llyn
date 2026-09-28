using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QAutograph
{
    private readonly UserControl _qAutographSurface;

    private CGuild _cGuild = null!;

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

    internal void QAutographAttach(CGuild guild)
    {
        _cGuild = guild;
        _cGuild.CGuildAutograph.CDeskStarted += QAutographStartUpdate;
        _cGuild.CGuildAutograph.CDeskObserverAttach(LObserver.LObserverCreate<Action>(static run => run()));
        _cGuild.CGuildAutograph.CDeskDraftChanged += QAutographDraftUpdate;
    }

    internal void QAutographTallyShow(CVita vita)
    {
        ArgumentNullException.ThrowIfNull(vita);

        QAutographWork.Text = vita.CVitaWork;
        QAutographTally.Text = vita.CVitaTally;
    }

    internal void QAutographModeUpdate()
    {
        QAutographUnionBody.Visibility = QLook.QLookVisibleRead(_cGuild.CGuildUnionShown);
        QAutographUnionNotice.Visibility = QLook.QLookVisibleRead(!_cGuild.CGuildUnionShown);
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
        _cGuild.CGuildAutograph.CDeskQuill?.LQuillAuthorSet(QAutographName.Text);
    }

    private void QAutographUnionHandle(object sender, TextChangedEventArgs e)
    {
        QAutographUnionList.ItemsSource =
            QRollItem.QRollItemBuild(_cGuild.CGuildUnionRead(QAutographUnion.Text));
    }

    private void QAutographUnionSelect(object sender, RoutedEventArgs e)
    {
        _cGuild.CGuildUnionSelect(QSender.QSenderSourceRead<QRollItem>(e)?.QRollItemId);
    }
}
